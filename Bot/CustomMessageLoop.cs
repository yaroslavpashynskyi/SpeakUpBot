using System.ComponentModel;
using Telegram.Bot.Types.Enums;
using TelegramBotBase.Args;
using TelegramBotBase.Base;
using TelegramBotBase.Form;
using TelegramBotBase.Interfaces;
using TelegramBotBase.Sessions;
using TelegramBotBase;
using Telegram.Bot.Types;
using TelegramBotBase.Enums;

namespace Bot;

public class CustomMessageLoop : IMessageLoopFactory
{
    private static readonly object EvUnhandledCall = new object();

    private readonly EventHandlerList _events = new EventHandlerList();
    private long _lastUpdateId;

    //
    // Summary:
    //     Will be called if no form handled this call
    public event EventHandler<UnhandledCallEventArgs> UnhandledCall
    {
        add { _events.AddHandler(EvUnhandledCall, value); }
        remove { _events.RemoveHandler(EvUnhandledCall, value); }
    }

    public async Task MessageLoop(
        BotBase bot,
        DeviceSession session,
        UpdateResult ur,
        MessageResult mr
    )
    {
        Update update = ur.RawData;
        if (
            update.Type != UpdateType.Message
            && update.Type != UpdateType.EditedMessage
            && update.Type != UpdateType.CallbackQuery
        )
        {
            return;
        }

        if (bot.GetSetting(ESettings.LogAllMessages, false) && _lastUpdateId != update.Id)
        {
            _lastUpdateId = update.Id;
            bot.OnMessage(new MessageIncomeEventArgs(session.DeviceId, session, mr));
        }

        if (mr.IsFirstHandler && mr.IsBotCommand && bot.IsKnownBotCommand(mr.BotCommand))
        {
            BotCommandEventArgs sce = new BotCommandEventArgs(
                mr.BotCommand,
                mr.BotCommandParameters,
                mr.Message,
                session.DeviceId,
                session
            );
            await bot.OnBotCommand(sce);
            if (sce.Handled)
            {
                return;
            }
        }

        mr.Device = session;
        ur.Device = session;
        FormBase activeForm = session.ActiveForm;
        await activeForm.PreLoad(mr);
        await activeForm.LoadControls(mr);
        await activeForm.Load(mr);
        if (
            update.Type == UpdateType.Message
            && (
                (mr.MessageType == MessageType.Contact)
                | (mr.MessageType == MessageType.Document)
                | (mr.MessageType == MessageType.Location)
                | (mr.MessageType == MessageType.Photo)
                | (mr.MessageType == MessageType.Video)
                | (mr.MessageType == MessageType.Audio)
            )
        )
        {
            await activeForm.SentData(new DataResult(ur));
        }

        if (update.Type == UpdateType.EditedMessage)
        {
            await activeForm.Edited(mr);
        }

        if (!session.FormSwitched && mr.IsAction)
        {
            await activeForm.ActionControls(mr);
            await activeForm.Action(mr);
            if (!mr.Handled)
            {
                UnhandledCallEventArgs unhandledCallEventArgs = new UnhandledCallEventArgs(
                    ur.Message.Text,
                    mr.RawData,
                    session.DeviceId,
                    mr.MessageId,
                    ur.Message,
                    session
                );
                OnUnhandledCall(unhandledCallEventArgs);
                if (unhandledCallEventArgs.Handled)
                {
                    mr.Handled = true;
                    if (!session.FormSwitched)
                    {
                        return;
                    }
                }
            }
        }

        if (!session.FormSwitched)
        {
            await activeForm.RenderControls(mr);
            await activeForm.Render(mr);
        }
    }

    public void OnUnhandledCall(UnhandledCallEventArgs e)
    {
        (_events[EvUnhandledCall] as EventHandler<UnhandledCallEventArgs>)?.Invoke(this, e);
    }
}
