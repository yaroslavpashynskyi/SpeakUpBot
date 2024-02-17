using System.ComponentModel;

namespace Domain.Enums;

public enum SpeakingStatus
{
    [Description("Ще не розпочався")]
    NotStarted,

    [Description("Триває")]
    InProgress,

    [Description("Відбувся")]
    Completed
}
