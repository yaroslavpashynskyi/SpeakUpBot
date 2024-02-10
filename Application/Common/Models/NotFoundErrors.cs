using Domain.Common;

namespace Application.Common.Models;

public static class NotFoundErrors<T>
{
    public static readonly Error EntityNotFound =
        new($"{typeof(T).Name}.NotFound", $"Сутність {typeof(T).Name} не знайдена");
}
