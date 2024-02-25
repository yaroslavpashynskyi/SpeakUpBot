using MediatR;

using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors;

public sealed class RequestLoggingPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<IPipelineBehavior<TRequest, TResponse>> _logger;

    public RequestLoggingPipelineBehavior(ILogger<IPipelineBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        string requestName = typeof(TRequest).Name;
        _logger.LogInformation("Processing request {RequestName} {@Request}", requestName, request);
        TResponse result = await next();

        _logger.LogInformation("Completed request {RequestName}", requestName);

        return result;
    }
}
