using Application.Common.Interfaces;
using Application.Extensions;

using Domain.Entities;
using Domain.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Persistence.Interceptors;

public class AvailableSeatNotificationInterceptor : SaveChangesInterceptor
{
    private readonly INotificationSender _notificationSender;

    public AvailableSeatNotificationInterceptor(INotificationSender notificationSender)
    {
        _notificationSender = notificationSender;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result
    )
    {
        Notify(eventData.Context).GetAwaiter().GetResult();

        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        await Notify(eventData.Context);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task Notify(DbContext? dbContext)
    {
        if (dbContext is null)
            return;

        IEnumerable<EntityEntry<Registration>> entries =
            dbContext.ChangeTracker.Entries<Registration>();

        foreach (EntityEntry<Registration> entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Modified)
            {
                int reserved = await dbContext
                    .Set<Registration>()
                    .CountAsync(
                        r =>
                            r.SpeakingId == entityEntry.Entity.SpeakingId
                            && r.PaymentStatus == PaymentStatus.InReserve
                    );
                if (reserved == 0)
                {
                    return;
                }

                int availableSeats = await dbContext
                    .Set<Registration>()
                    .CountAsync(
                        r =>
                            r.SpeakingId == entityEntry.Entity.SpeakingId
                            && r.PaymentStatus != PaymentStatus.Cancelled
                            && r.PaymentStatus != PaymentStatus.InReserve
                    );

                if (availableSeats > reserved)
                {
                    return;
                }

                PaymentStatus originalValue = entityEntry
                    .Property(r => r.PaymentStatus)
                    .OriginalValue;
                PaymentStatus currentValue = entityEntry
                    .Property(r => r.PaymentStatus)
                    .CurrentValue;

                PaymentStatus[] statusesOfInterest =
                {
                    PaymentStatus.Cancelled,
                    PaymentStatus.InReserve
                };
                if (
                    statusesOfInterest.Contains(originalValue)
                    || statusesOfInterest.Contains(currentValue)
                )
                {
                    var speaking = await dbContext
                        .Set<Speaking>()
                        .Include(s => s.Registrations)
                        .ThenInclude(r => r.User)
                        .FirstOrDefaultAsync(s => s.Id == entityEntry.Entity.Speaking.Id);

                    if (
                        (
                            currentValue == PaymentStatus.Cancelled
                            && originalValue != PaymentStatus.InReserve
                        )
                        || (
                            originalValue == PaymentStatus.InReserve
                            && currentValue == PaymentStatus.Cancelled
                        )
                    )
                    {
                        /* New seat available or Reservation cancelled, notify a user in reserve
                         whose ordinal number is how many seats are available - 1 */
                        long userToNotify = speaking!.Registrations
                            .Where(r => r.PaymentStatus == PaymentStatus.InReserve)
                            .OrderBy(r => r.RegistrationDate)
                            .Select(r => r.User.TelegramId)
                            .ElementAtOrDefault(speaking.AvailableSeats - 1);

                        if (userToNotify != 0)
                        {
                            await _notificationSender.SendToUser(
                                $"Для вас з'явилось місце на {speaking.GetName()}!\n"
                                    + $"Перейдіть в мої реєстрації та активуйте реєстрацію.",
                                userToNotify
                            );
                        }
                    }
                }
            }
        }
    }
}
