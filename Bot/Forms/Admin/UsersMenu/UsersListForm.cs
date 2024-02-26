using Application.Users.Queries.GetAllUsers;

using Bot.Forms.Common.Base;

using Domain.Entities;

using MediatR;

using Telegram.Bot.Types.Enums;

namespace Bot.Forms.Admin.UsersMenu;

public class UsersListForm : ListItemsForm<User>
{
    public UsersListForm(IMediator mediator)
    {
        _mediator = mediator;
        _request = new GetAllUsersQuery();
    }

    protected override string GetButtonName(User user)
    {
        return $"{user.FirstName} {user.LastName}";
    }

    protected override async Task HandleEntity(User user)
    {
        var transferTicketStatus = user.TransferTicket ? "Присутній✅" : "Відсутній🛑";
        await Device.Send(
            "Інформація про користувача.\n"
                + $"Ім'я: {user.FirstName}\nПрізвище: {user.LastName}\n"
                + $"Номер телефону: {user.PhoneNumber}\n"
                + $"Квиток переносу: {transferTicketStatus}\n"
                + $"Дата реєстрації користувача: {user.CreatedAt.ToLocalTime()}\n"
                + $"Рівень англійської: {user.EnglishLevel}\n"
                + $"Як про нас дізнались: {user.Source.Title}\n\n"
                + $"<a href=\"tg://user?id={user.TelegramId}\">Зв'язатись з користувачем</a>",
            parseMode: ParseMode.Html
        );
    }
}
