using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contract.Infrestructure;
using Ordering.Application.Contract.Persistence;
using Ordering.Application.Models;
using Ordering.Domain.Common.Entities;
using System.Threading;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder
{
    public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, int>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<CheckoutOrderCommandHandler> _logger;

        public CheckoutOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper, IEmailService emailService, ILogger<CheckoutOrderCommandHandler> logger)
        {
            _orderRepository = orderRepository;
            this.mapper = mapper;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<int> Handle(CheckoutOrderCommand request, CancellationToken cancellationtoken)
        {
            var orderEntity = this.mapper.Map<Order>(request);
            var nuevaOrden = await _orderRepository.AddAsync(orderEntity);
            _logger.LogInformation($"Order {nuevaOrden.Id} se creo con exito");
            await SendMail(nuevaOrden);
            return nuevaOrden.Id;
        }

        private async Task SendMail(Order order)
        {
            var email = new Email() { To = "jbriantguerrero@gmail.com", Body = $"La Orden Fue Creada.", Subject = "La Orden Fue Creada" };
            try
            {
                await _emailService.SendEmail(email);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Orden {order.Id}" + $"fallo un error ocurrio a; enviarse el email :" + $"{ex.Message}");
            }
        }
    }
}
