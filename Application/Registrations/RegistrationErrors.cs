using Domain.Common;

namespace Application.Registrations;

public static class RegistrationErrors
{
    public static readonly Error RegistrationDuplication =
        new(
            "Registration.RegistrationDuplication",
            "Реєстрація на даний спікінг від користувача вже існує"
        );
}
