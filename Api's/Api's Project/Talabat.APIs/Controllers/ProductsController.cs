using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Windows.Markup;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Interfaces;
using Talabat.Core.Specifications.ProductSpecs;

namespace Talabat.APIs.Controllers
{

    public class ProductsController : BaseApiController

    {
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<ProductType> _categoriesRepo;
        private readonly IGenericRepository<ProductBrand> _brandsRepo;

        //GetAll
        //GetById

        public ProductsController(IGenericRepository<Product> productRepo,
                                                              IMapper mapper,
                                                              IGenericRepository<ProductBrand> brandsRepo,
                                                              IGenericRepository<ProductType> categoriesRepo)
        {
            _productRepo = productRepo;
            _mapper = mapper;
            _categoriesRepo = categoriesRepo;
            _brandsRepo = brandsRepo;
        }






        // GET : /api/products
        [HttpGet] 
		public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts([FromQuery] ProductSpecParams productSpec)
        {
                
              //  var products = await _productRepo.GetAllAsync();
            var spec = new ProductWithBrandAndCategorySpecifications( productSpec );
            var products = await _productRepo.GetAllWithSpecAsync(spec);
            var data = _mapper.Map< IReadOnlyList<Product> , IReadOnlyList<ProductToReturnDto>>( products );
            var countSpec = new ProductWithFilterationForCountSpecifications( productSpec );
            var count = await _productRepo.GetCountAsync(countSpec);
            return Ok( new Pagination<ProductToReturnDto>( productSpec.PageSize , productSpec.PageIndex , count, data) );


        }





        [ ProducesResponseType(typeof(ProductToReturnDto) , StatusCodes.Status200OK ) ]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
		[HttpGet(template:"{id}")]    // GET : /api/products/3
		public async Task<ActionResult<ProductToReturnDto>> GetProductById(int id)
        {

			// var product = await _productRepo.GetAsync(id);


			var specs = new ProductWithBrandAndCategorySpecifications(id);

            var product =await _productRepo.GetWithSpecAsync(specs);


			if (product is null) 
                return NotFound(new ApiResponse( 404 ) );


            var result = _mapper.Map<Product, ProductToReturnDto>(product);


            return Ok(result);   //200

            
        }







        [HttpGet("Brands")]  // GET : /api/products/Brands
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
        {
            var brands = await _brandsRepo.GetAllAsync();
            
            return Ok(brands);

        }




		[HttpGet("types")]  // GET : /api/products/categories
		public async Task<ActionResult<IReadOnlyList<ProductType>>> GetCategories()
		{
			var categories = await _categoriesRepo.GetAllAsync();

			return Ok(categories);

		}







	}
}
