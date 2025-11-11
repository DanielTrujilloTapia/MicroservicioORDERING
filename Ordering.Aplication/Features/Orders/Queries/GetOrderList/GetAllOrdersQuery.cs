using MediatR;
using Ordering.Application.Features.Orders.Queries.GetOrderList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Aplication.Features.Orders.Queries.GetOrderList
{
    public class GetAllOrdersQuery : IRequest<List<OrderVM>>
    {
        
    }

}
