using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Interfaces;

namespace Talabat.APIs.Controllers
{
	public class BasketController : BaseApiController
	{
		private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository basketRepository , IMapper mapper)
		{
			_basketRepository = basketRepository;
            _mapper = mapper;
        }



		[HttpGet]     //  GET : /api/basket?id=11
		public async Task<ActionResult<CustomerBasket>> GetBasket(string id)
		{
			var basket = await _basketRepository.GetBasketAsync(id);

            //return Ok( basket.Id);
            return Ok( basket ?? new CustomerBasket(id));
          //  return basket is null ? new CustomerBasket(id) : basket;


        }


        [HttpPost]   // POST : /api/basket
		public async Task<ActionResult<CustomerBasket>> CreateOrUpdateBasket(CustomerBasketDto model)
		{
		var mappedBasket = 	_mapper.Map<CustomerBasket>(model);

			var  createdOrUpdatedBasket = await _basketRepository.UpdateBasketAsync(mappedBasket);

			if (createdOrUpdatedBasket is null)
				return BadRequest( new ApiResponse(400));  

			return Ok(createdOrUpdatedBasket);

		}




		[HttpDelete]   // DELETE : /api/basket
		public async Task DeleteBasket (string id)
		{
			 await _basketRepository.DeletBasketAsync(id);
		}






	}
}
