﻿using Domain.Common;

namespace Application.Registrations;

public static class RegistrationErrors
{
    public static readonly Error RegistrationDuplication =
        new(
            "Registration.RegistrationDuplication",
            "Реєстрація на даний івент від користувача вже існує"
        );
    public static readonly Error RegistrationTimeout =
        new(
            "Registration.RegistrationTimeout",
            "На жаль, на даний івент реєстрації та скасування вже не приймаються😔"
        );
    public static readonly Error RegistrationNeedToBeApproved =
        new(
            "Registration.RegistrationNeedToBeApproved",
            "Поки організатор не підтвердить статус платежу, скасувати реєстрацію неможливо"
        );
    public static readonly Error RegistrationAlreadyCancelled =
        new("Registration.RegistrationAlreadyCancelled", "Реєстрація вже скасована");
    public static readonly Error RegistrationAlreadyActive =
        new("Registration.RegistrationAlreadyActive", "Реєстрація вже активна");
    public static readonly Error PaymentNotPending =
        new("Registration.PaymentNotPending", "Реєстрація не очікує оплату");

    public static readonly Error RegistrationInReserve =
        new("Registration.RegistrationInReserve", "Для вас ще не звільнось місце на івенті");
}
