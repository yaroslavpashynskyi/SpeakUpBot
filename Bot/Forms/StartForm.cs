using Application.Users.Queries.GetUserRole;

using Bot.Forms.Admin;
using Bot.Forms.Common.UserRegistration;
using Bot.Forms.Member;

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
        if (Device.IsGroup || Device.IsChannel)
            return;

        Role userRole = await _mediator.Send(new GetUserRoleQuery(Device.DeviceId));
        switch (userRole)
        {
            case Role.Member:
                await this.NavigateTo<MemberMenuForm>();
                break;
            case Role.Admin:
                await this.NavigateTo<AdminMenuForm>();
                break;
            default:
                await this.NavigateTo<StartUserRegistrationForm>();
                break;
        }
    }
}
