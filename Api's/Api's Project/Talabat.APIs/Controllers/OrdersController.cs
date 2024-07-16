using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.Core;
using Talabat.Core.Entities.Order;
using Talabat.Core.Services.Interfaces;

namespace Talabat.APIs.Controllers
{
   
    public class OrdersController : BaseApiController
    {
        private readonly IOrderServices _orderServices;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OrdersController(IOrderServices orderServices , IMapper mapper , IUnitOfWork unitOfWork)
        {
            _orderServices = orderServices;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [ProducesResponseType(typeof(OrderToRetunDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest  )]
        [HttpPost] // POST : /api/Orders
        [Authorize]
        public async Task<ActionResult<OrderToRetunDto>> CreateOrder(OrderDto model)
        {
          var bueyrEmail = User.FindFirstValue(ClaimTypes.Email);
          var address = _mapper.Map<AddressDto, Address>(model.ShippingAddress);
          var order = await  _orderServices.CreateOrderAsync(bueyrEmail, model.BasketId, model.DeliveryMethodId, address);
          
            if (order is null) return BadRequest(new ApiResponse(statusCode: 400, message: "There is a Problem With Your Order!"));
           var result =  _mapper.Map<Order, OrderToRetunDto>(order);
            return Ok(order);
        }


        [HttpGet] // GET : /api/Orders
        [Authorize]
        public async Task<ActionResult<IReadOnlyList<OrderToRetunDto>>> GetOrdersForUser()
        {

          var buyerEmail = User.FindFirstValue(ClaimTypes.Email);

          var orders =  _orderServices.GetOrdersForSpecificUserAsync(buyerEmail);
            if (orders is null) return NotFound(value: new ApiResponse(statusCode: 400, message: "There os no Order For You!"));


            return Ok(_mapper.Map<IReadOnlyList<OrderToRetunDto>>(orders));
        }

        [HttpGet(template:"{id}")] // GET : /api/Orders/1
        [Authorize]
        public async  Task<ActionResult<OrderToRetunDto>> GetOrderByIdForUser(int id)
        {
           var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
           var order = await _orderServices.GetOrderByIdForSpecificUserAsync(buyerEmail , id);
            if (order is null) return NotFound(new ApiResponse(statusCode: 400, message: $"There is No Order With id {id} For U!"));
              
            return Ok( _mapper.Map< OrderToRetunDto>(order));
        }

        [HttpGet(template:"DeliveryMethods")] // GET : /api/Orders/DeliveryMethods
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
           var deliveryMethods = await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
            
            return Ok(deliveryMethods);
        }


    }
}
