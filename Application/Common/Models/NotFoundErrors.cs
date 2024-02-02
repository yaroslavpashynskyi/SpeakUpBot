using Domain.Common;

namespace Application.Common.Models;

public static class NotFoundErrors<T>
{
    public static readonly Error EntityNotFound =
        new($"{nameof(T)}.NotFound", $"Сутність {nameof(T)} не знайдена");
}
