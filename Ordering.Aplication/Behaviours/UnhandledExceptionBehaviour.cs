using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ordering.Application.Behaviours
{
    // Pipeline de MediatR para capturar excepciones no controladas
    public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<TRequest> _logger;

        public UnhandledExceptionBehaviour(ILogger<TRequest> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            try
            {
                // Ejecuta el siguiente comportamiento o handler
                return await next();
            }
            catch (Exception ex)
            {
                // Log de la excepción
                var requestName = typeof(TRequest).Name;
                _logger.LogError(
                    ex,
                    "Excepción no controlada para la petición {RequestName} {@Request}",
                    requestName,
                    request
                );

                // Lanza la excepción para que se propague
                throw;
            }
        }
    }
}
