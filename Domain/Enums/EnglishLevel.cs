using System.ComponentModel;

namespace Domain.Enums;

public enum EnglishLevel
{
    [Description("Впевнений Pre-Intermediate + бокал вина :)")]
    PreIntermediate = 1,
    Intermediate,

    [Description("Upper Intermediate")]
    UpperIntermediate,
    Advanced
}
