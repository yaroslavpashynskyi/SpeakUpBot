using Application.Sources.Queries;
using Application.Users.Commands.CreateUser;

using Domain.Entities;

using MediatR;

using TelegramBotBase.Args;
using TelegramBotBase.Base;
using TelegramBotBase.DependencyInjection;
using TelegramBotBase.Form;

namespace Bot.Forms.Common.UserRegistration.Steps;

public class GetSourceForm : AutoCleanForm
{
    private readonly IMediator _mediator;
    private List<Source> _defaultSources = [];
    public CreateUserCommand UserData { get; set; } = default!;

    public GetSourceForm(IMediator mediator)
    {
        Init += Source_Init;
        _mediator = mediator;
    }


    private async Task Source_Init(object sender, InitEventArgs e)
    {
        _defaultSources = await _mediator.Send(new GetDefaultSourcesCommand());
        UserData = (CreateUserCommand)e.Args[0];
    }

    public override async Task Action(MessageResult message)
    {
        await message.ConfirmAction();

        Guid.TryParse(message.RawData, out Guid selectedSource);
        if (selectedSource == Guid.Empty)
            return;

        var source = _defaultSources.FirstOrDefault(x => x.Id == selectedSource);
        if (source == null)
            return;

        UserData.Source = source;
    }

    public override async Task Render(MessageResult message)
    {
        if (UserData.Source == null)
        {
            var bf = new ButtonForm();
            foreach (var source in _defaultSources)
            {
                bf.AddButtonRow(new ButtonBase(source.Title, source.Id.ToString()));
            }

            await Device.Send("Звідки ви дізналися про нас?", bf);
            return;
        }

        await this.NavigateTo<UserConfirmationForm>(UserData);
    }
}
