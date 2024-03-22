namespace Domain.Common;

public interface IOrderable
{
    object? GetOrderKey();
}
