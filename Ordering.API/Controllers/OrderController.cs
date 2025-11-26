using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Aplication.Features.Orders.Queries.GetOrderList;
using Ordering.Aplication.Messaging;
using Ordering.Application.Exceptions;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder;
using Ordering.Application.Features.Orders.Commands.DeleteOrder;
using Ordering.Application.Features.Orders.Commands.UpdateOrder;
using Ordering.Application.Features.Orders.Queries.GetOrderList;
using Ordering.Domain.Message;
using System.Net;

namespace Ordering.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class OrderController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IEventBus _eventBus; // 👈 inyectamos el bus de eventos

        public OrderController(IMediator mediator, IEventBus eventBus)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        [HttpGet(Name = "GetAllOrders")]
        [ProducesResponseType(typeof(IEnumerable<OrderVM>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<OrderVM>>> GetAllOrders()
        {
            var query = new GetAllOrdersQuery();
            var orders = await _mediator.Send(query);
            return Ok(orders);
        }


        [HttpGet("{username}", Name = "GetOrder")]
        [ProducesResponseType(typeof(IEnumerable<OrderVM>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<OrderVM>>> GetOrdersByUsername(string username)
        {
            var query = new GetOrderListQuery(username);
            var orders = await _mediator.Send(query);
            return Ok(orders);
        }

        /*[HttpPost(Name ="CheckoutOrder")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<int>> CheckoutOrder([FromBody] CheckoutOrderCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }*/

        [HttpPost(Name = "CheckoutOrder")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<int>> CheckoutOrder([FromBody] CheckoutOrderCommand command)
        {
            // 1️⃣ Ejecutas el comando normal con MediatR
            var orderId = await _mediator.Send(command);

            // 2️⃣ Construyes el mensaje para RabbitMQ con los datos del pedido
            var message = new OrderMessage
            {
                Id = orderId,
                UserName = command.UserName,
                TotalPrice = command.TotalPrice,
                FirstName = command.FirstName,
                LastName = command.LastName,
                EmailAddress = command.EmailAddress,
                AddressLine = command.AddressLine,
                Country = command.Country,
                State = command.State,
                ZipCode = command.ZipCode,
                CardName = command.CardName,
                CardNumber = command.CardNumber,
                Expiration = command.Expiration,
                CVV = command.CVV,
                PaymentMethod = command.PaymentMethod
            };

            // 3️⃣ Envías el evento a RabbitMQ
            await _eventBus.PublishAsync(message);

            // 4️⃣ Devuelves la respuesta normal
            return Ok(orderId);
        }

        [HttpPut(Name = "UpdateOrder")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> UpdateOrder([FromBody] UpdateOrderCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }
            [HttpDelete("delete", Name = "DeleteOrder")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            [ProducesDefaultResponseType]
            public async Task<ActionResult> DeleteOrder([FromQuery] int? id, [FromQuery] string? userName)
            {
                if (id == null && string.IsNullOrWhiteSpace(userName))
                    return BadRequest("Debe proporcionar un Id o un UserName.");

                try
                {
                    await _mediator.Send(new DeleteOrderCommand { Id = id, UserName = userName });
                    return Ok("Orden eliminada correctamente");
                }
                catch (NotFoundException ex)
                {
                    return NotFound(ex.Message);
                }
            }
    }
}
