using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contract.Persistence;
using Ordering.Application.Exceptions;
using Ordering.Domain.Common.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Commands.DeleteOrder
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DeleteOrderCommandHandler> _logger;

        public DeleteOrderCommandHandler(
            IOrderRepository orderRepository,
            IMapper mapper,
            ILogger<DeleteOrderCommandHandler> logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            Order? orderToDelete = null;

            if (request.Id != null)
            {
                orderToDelete = await _orderRepository.GetByAsync(request.Id.Value);
                if (orderToDelete == null)
                {
                    _logger.LogError($"No existe la orden con Id {request.Id}");
                    throw new NotFoundException(nameof(Order), request.Id);
                }
            }
            else if (!string.IsNullOrWhiteSpace(request.UserName))
            {
                var orders = await _orderRepository.GetOrdersByUserName(request.UserName);
                orderToDelete = orders.FirstOrDefault();
                if (orderToDelete == null)
                {
                    _logger.LogError($"No existe ninguna orden para el usuario '{request.UserName}'");
                    throw new NotFoundException(nameof(Order), request.UserName);
                }
            }
            else
            {
                _logger.LogError("Se requiere un Id o un UserName para eliminar la orden.");
                throw new ValidationException("Debe proporcionar un Id o un UserName válido.");
            }

            await _orderRepository.DeleteAsync(orderToDelete);
            _logger.LogInformation($"Orden '{orderToDelete.Id}' eliminada correctamente.");

            return Unit.Value;
        }
    }
}