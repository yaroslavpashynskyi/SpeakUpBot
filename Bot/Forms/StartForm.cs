using Application.Users.Queries.GetUserRole;

using Bot.Forms.Common.UserRegistration;

using Domain.Enums;

using MediatR;

using Microsoft.Extensions.Logging;

using TelegramBotBase.Base;
using TelegramBotBase.DependencyInjection;
using TelegramBotBase.Form;

namespace Bot.Forms;

public class StartForm : FormBase
{
    private readonly ILogger<StartForm> _logger;

    private readonly IMediator _mediator;

    public StartForm(ILogger<StartForm> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public override async Task Load(MessageResult message)
    {
        await Device.HideReplyKeyboard();

        Role userRole = await _mediator.Send(new GetUserRoleQuery(Device.DeviceId));
        switch (userRole)
        {
            case Role.Member:
                await Device.Send("Вітаємо вас, звичайний користувач!");
                break;
            case Role.Admin:
                await Device.Send("Вітаємо вас, адміністратор!");
                break;
            default:
                await this.NavigateTo<StartUserRegistrationForm>();
                break;
        }
    }
}
