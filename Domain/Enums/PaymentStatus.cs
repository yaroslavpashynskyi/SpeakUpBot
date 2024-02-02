using System.ComponentModel;

namespace Domain.Enums;

public enum PaymentStatus
{
    [Description("Очікується оплата")]
    Pending,

    [Description("Оплата повинна бути підтверджена організатором")]
    ToBeApproved,

    [Description("Оплачено картою")]
    PaidByCard,

    [Description("Оплачено квитком переносу")]
    PaidByTransferTicket,

    [Description("Оплачено/Буде оплачено готівкою")]
    ToBePaidByCash,

    [Description("Скасовано")]
    Cancelled
}
