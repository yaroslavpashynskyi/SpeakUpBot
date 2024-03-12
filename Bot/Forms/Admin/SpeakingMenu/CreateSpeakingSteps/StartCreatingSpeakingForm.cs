using System.Reflection;

using Application.Speakings.Commands.CreateSpeaking;

using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

using TelegramBotBase.Base;
using TelegramBotBase.DependencyInjection;
using TelegramBotBase.Form;

namespace Bot.Forms.Admin.SpeakingMenu.CreateSpeakingSteps;

public class StartCreatingSpeakingForm : AutoCleanForm
{
    private readonly CreateSpeakingCommand _speakingData = new();

    private int _currentStep = 0;
    private int _photoOrderStep = 0;

    private List<PhotoDto> _photos = new();
    private readonly List<string> _propertyOrder =
        new()
        {
            nameof(CreateSpeakingCommand.Title),
            nameof(CreateSpeakingCommand.Intro),
            nameof(CreateSpeakingCommand.Description),
            nameof(CreateSpeakingCommand.Price),
            nameof(CreateSpeakingCommand.Seats),
            nameof(CreateSpeakingCommand.Photos)
        };
    private readonly List<string> _optionalProperties =
        new() { nameof(CreateSpeakingCommand.Intro) };

    private readonly KeyboardButton _confirmPhotoButton = new("Закінчити надсилання фото");
    private readonly KeyboardButton _resetPhotoButton = new("Очистити фото");

    public override async Task Load(MessageResult message)
    {
        if (string.IsNullOrWhiteSpace(message.MessageText) || message.Handled)
        {
            return;
        }

        var property = GetNextUnsetSpeakingProperty();
        if (property == null)
            return;
        bool success = false;
        if (property.PropertyType == typeof(string))
        {
            success = await SetStringProperty(message, property);
        }
        else if (property.PropertyType == typeof(int))
        {
            success = await SetIntProperty(message, property);
        }
        else if (property.PropertyType == typeof(List<PhotoDto>))
        {
            if (message.MessageText == _confirmPhotoButton.Text)
            {
                _speakingData.Photos = _photos;
                success = true;
            }
            else if (message.MessageText == _resetPhotoButton.Text)
                ResetPhotos();
        }
        if (success)
            _currentStep++;
    }

    private PropertyInfo? GetNextUnsetSpeakingProperty()
    {
        return _propertyOrder
            .Select(p => _speakingData.GetType().GetProperty(p))
            .FirstOrDefault(p => IsPropertyDefaultValue(p!, _speakingData));
    }

    public static bool IsPropertyDefaultValue(PropertyInfo propertyInfo, object obj)
    {
        var propertyType = propertyInfo.PropertyType;
        var defaultValue = propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null;
        var actualValue = propertyInfo.GetValue(obj);
        return Equals(actualValue, defaultValue);
    }

    private async Task<bool> SetStringProperty(MessageResult message, PropertyInfo property)
    {
        if (message.MessageText.Length > 1024)
        {
            await Device.Send(
                "Занадто багато символів! В пості повинно бути не більше 1024 символів!"
            );
            return false;
        }
        property.SetValue(_speakingData, message.MessageText);
        return true;
    }

    private async Task<bool> SetIntProperty(MessageResult message, PropertyInfo property)
    {
        if (!int.TryParse(message.MessageText, out var result) || result <= 0)
        {
            await Device.Send("Вам потрібно ввести число більше за 0");
            return false;
        }
        property.SetValue(_speakingData, result);
        return true;
    }

    public override Task SentData(DataResult message)
    {
        if (message.Type == MessageType.Photo && _speakingData.Photos == null)
        {
            if (_photoOrderStep == 10)
            {
                Device.Send("Максимум 10 фото! Відправте ще раз");
                ResetPhotos();
                return Task.CompletedTask;
            }
            _photoOrderStep++;
            _photos.Add(new PhotoDto { FileId = message.FileId, OrdinalNumber = _photoOrderStep });
        }
        return Task.CompletedTask;
    }

    private void ResetPhotos()
    {
        _photoOrderStep = 0;
        _photos = new();
    }

    public override async Task Action(MessageResult message)
    {
        await message.ConfirmAction();

        switch (message.RawData)
        {
            case "skip":
                var nextProperty = GetNextUnsetSpeakingProperty();
                if (_optionalProperties.Contains(nextProperty?.Name ?? ""))
                {
                    GetNextUnsetSpeakingProperty()!.SetValue(_speakingData, "");
                    _currentStep++;
                }
                break;
        }
    }

    public override async Task Render(MessageResult message)
    {
        if (_currentStep < _propertyOrder.Count)
        {
            ButtonForm? bf = null;

            if (_photoOrderStep > 0)
            {
                var keyboard = new ReplyKeyboardMarkup(
                    new[] { _confirmPhotoButton, _resetPhotoButton }
                );
                keyboard.ResizeKeyboard = true;
                await Device.Send($"Ви відправили {_photoOrderStep} фото", keyboard);
                return;
            }

            var propertyName = _propertyOrder[_currentStep];
            var prompt = GetPromptForProperty(propertyName);
            if (_optionalProperties.Contains(propertyName))
            {
                bf = new ButtonForm();
                bf.AddButtonRow(new ButtonBase("Пропустити", "skip"));
            }

            await Device.Send(prompt, bf);
        }
        else
        {
            await this.NavigateTo<SelectTimeOfEventForm>(_speakingData);
        }
    }

    private string GetPromptForProperty(string propertyName)
    {
        return propertyName switch
        {
            nameof(CreateSpeakingCommand.Title) => "Дайте назву івенту",
            nameof(CreateSpeakingCommand.Intro) => "Напишить вступ (за бажанням)",
            nameof(CreateSpeakingCommand.Description) => "Надайте опис до івенту",
            nameof(CreateSpeakingCommand.Price) => "Ціна запису в ₴ (ціле число більше за 0)",
            nameof(CreateSpeakingCommand.Seats) => "Кількість місць (ціле число більше за 0)",
            nameof(CreateSpeakingCommand.Photos) => "Надішліть до 10 фото для поста в телеграм",
            _ => "Помилка",
        };
    }
}
