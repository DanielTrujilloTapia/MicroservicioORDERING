using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Aplication.Features.Orders.Queries.GetOrderList;
using Ordering.Application.Exceptions;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder;
using Ordering.Application.Features.Orders.Commands.DeleteOrder;
using Ordering.Application.Features.Orders.Commands.UpdateOrder;
using Ordering.Application.Features.Orders.Queries.GetOrderList;
using System.Net;

namespace Ordering.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class OrderController : Controller
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
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

        [HttpPost(Name ="CheckoutOrder")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<int>> CheckoutOrder([FromBody] CheckoutOrderCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
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
