using AutoMapper;
using MediatR;
using Ordering.Application.Contract.Persistence;
using Ordering.Application.Features.Orders.Queries.GetOrderList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Aplication.Features.Orders.Queries.GetOrderList
{
    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, List<OrderVM>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public GetAllOrdersQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<List<OrderVM>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            var orderList = await _orderRepository.GetAllOrders();
            return _mapper.Map<List<OrderVM>>(orderList);
        }
    }

}
