using System.Globalization;
using Application.Speakings.Commands.CreateSpeaking;
using Humanizer;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotBase.Args;
using TelegramBotBase.Base;
using TelegramBotBase.Controls.Inline;
using TelegramBotBase.Enums;
using TelegramBotBase.Form;
using TelegramBotBase.DependencyInjection;

namespace Bot.Forms.Admin.SpeakingMenu.CreateSpeakingSteps
{
    public class SelectTimeOfEventForm : AutoCleanForm
    {
        private const int minutesStep = 30;
        private CreateSpeakingCommand _speakingData = new();
        private CalendarPicker _picker = new();
        private int? _selectedDateMessage;

        private readonly ButtonForm _timeButtons = new();
        private readonly ButtonForm _durationButtons = new();

        public SelectTimeOfEventForm()
        {
            DeleteMode = EDeleteMode.OnLeavingForm;
            InitButtons();
            Init += InitializeCalendarPicker;
        }

        private void InitButtons()
        {
            AddTimeButtons();
            AddDurationButtons();
        }

        private void AddTimeButtons()
        {
            var timeSample = new TimeOnly(10, 0);
            var buttonList = new List<ButtonBase>();

            while (timeSample.Hour <= 20)
            {
                buttonList.Add(
                    new ButtonBase(timeSample.ToShortTimeString(), timeSample.ToShortTimeString())
                );
                timeSample = timeSample.AddMinutes(minutesStep);
            }

            _timeButtons.AddSplitted(buttonList);
        }

        private void AddDurationButtons()
        {
            var durationSample = TimeSpan.FromMinutes(minutesStep);
            while (durationSample.Hours <= 4)
            {
                _durationButtons.AddButtonRow(
                    new ButtonBase(
                        durationSample.Humanize(
                            2,
                            culture: CultureInfo.CreateSpecificCulture("uk-UA")
                        ),
                        durationSample.TotalMinutes.ToString()
                    )
                );
                durationSample = durationSample.Add(TimeSpan.FromMinutes(minutesStep));
            }
        }

        private Task InitializeCalendarPicker(object sender, InitEventArgs e)
        {
            _speakingData = (CreateSpeakingCommand)e.Args[0];

            _picker = new CalendarPicker { Title = "Оберіть дату проведення івенту" };

            return Task.CompletedTask;
        }

        public override async Task Load(MessageResult message)
        {
            if (IsDefaultValue(_speakingData.DateOfEvent))
            {
                if (Controls.Count == 0)
                    AddControl(_picker);
                return;
            }

            if (
                IsDefaultValue(_speakingData.TimeOfEvent)
                && TryParseTime(message.MessageText, out var time)
            )
            {
                _speakingData.TimeOfEvent = time;
                return;
            }
            else if (
                IsDefaultValue(_speakingData.DurationMinutes) && TrySetDuration(message.MessageText)
            )
                return;

            await NotifyInvalidInput();
        }

        private bool TryParseTime(string input, out TimeOnly time)
        {
            return TimeOnly.TryParse(input, out time);
        }

        private bool TrySetDuration(string input)
        {
            var button = FindButtonWithValue(_durationButtons, input);
            if (button != null)
            {
                _speakingData.DurationMinutes = int.Parse(button.Value);
                return true;
            }

            if (int.TryParse(input, out var duration) && duration > 0)
            {
                _speakingData.DurationMinutes = duration;
                return true;
            }

            return false;
        }

        private ButtonBase? FindButtonWithValue(ButtonForm durationButtons, string value)
        {
            foreach (var button in durationButtons.ToList())
            {
                if (button.Text == value)
                {
                    return button;
                }
            }
            return null;
        }

        private async Task NotifyInvalidInput()
        {
            await Device.Send("Неправильний ввід. Будь ласка, спробуйте знову.");
        }

        public override async Task Action(MessageResult message)
        {
            await message.ConfirmAction();
            if (message.RawData == "confirmDate")
                ConfirmDateSelection();
        }

        private void ConfirmDateSelection()
        {
            if (!IsDefaultValue(_speakingData.DateOfEvent))
                return;
            _speakingData.DateOfEvent = DateOnly.FromDateTime(_picker.SelectedDate);
            RemoveControl(_picker);
            if (_selectedDateMessage.HasValue)
                Device.DeleteMessage(_selectedDateMessage.Value);
        }

        public override async Task Render(MessageResult message)
        {
            if (IsDefaultValue(_speakingData.DateOfEvent))
                await RenderDateSelection();
            else if (IsDefaultValue(_speakingData.TimeOfEvent))
                await RenderTimeSelection();
            else if (IsDefaultValue(_speakingData.DurationMinutes))
                await RenderDurationSelection();
            else
                await this.NavigateTo<SelectSpeakingVenueForm>(_speakingData);
        }

        private async Task RenderDateSelection()
        {
            string dateText = $"Обрана дата: {_picker.SelectedDate.ToShortDateString()}";
            ButtonForm buttons = new ButtonForm();
            buttons.AddButtonRow(new ButtonBase("Підтвердити", "confirmDate"));

            _selectedDateMessage = await UpdateOrSendMessage(
                dateText,
                buttons,
                _selectedDateMessage
            );
        }

        private async Task RenderTimeSelection()
        {
            var keyboard = CreateKeyboardMarkup(_timeButtons);
            await Device.Send(
                "Введіть точний час проведення заходу.\n"
                    + "Оберіть готовий, або напишіть свій у 24-годинному форматі (гг:хх)",
                keyboard
            );
        }

        private async Task RenderDurationSelection()
        {
            var keyboard = CreateKeyboardMarkup(_durationButtons);
            await Device.Send(
                "Введіть тривалість проведення заходу.\n"
                    + "Можна обрати одну із запропонованих або ввести тривалість в хвилинах.",
                keyboard
            );
        }

        private static ReplyKeyboardMarkup CreateKeyboardMarkup(ButtonForm buttons)
        {
            var keyboard = (ReplyKeyboardMarkup)buttons;
            keyboard.ResizeKeyboard = true;
            return keyboard;
        }

        private async Task<int?> UpdateOrSendMessage(
            string text,
            ButtonForm buttons,
            int? messageId
        )
        {
            if (messageId.HasValue)
            {
                await Device.Edit(messageId.Value, text, buttons);
                return messageId;
            }
            else
            {
                var message = await Device.Send(text, buttons);
                return message.MessageId;
            }
        }

        public static bool IsDefaultValue<T>(T item) =>
            EqualityComparer<T>.Default.Equals(item, default);
    }
}
