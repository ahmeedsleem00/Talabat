using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.Repository.Data;

namespace Talabat.APIs.Controllers
{

	public class BuggyController : BaseApiController
	{
		private readonly StoreDbContext _context;

		public BuggyController(StoreDbContext context)
		{
			_context = context;
		}



		 [HttpGet("notFound")]    // Get : / aPi/ Buggy/ notfound
        public  ActionResult GetNotfoundRequest()
		{
			var Product = _context.Products.Find(1000);

			if (Product is null)
				return NotFound( new ApiResponse( 404 ) );

			return Ok(Product); 

        }



		[HttpGet("servererror")]      // Get : / aPi/ Buggy/ servererror
		public ActionResult GetServerError() 
		{
			var product = _context.Products.Find(1000);

			var result = product.ToString();  // Will Throw Exception [ Null ReferenceException ]

			return Ok(result);

		}



		[HttpGet("badrequest")]      // Get : / aPi/ Buggy/ badrequest
		public ActionResult GetBadRequest()
		{

			return BadRequest( new ApiResponse( 400 ) );
		}



		[HttpGet("badrequest/{id}")]               // Get : / aPi/ Buggy/ badrequest / 5
		public ActionResult GetBadRequest(int? id)                   // Validation Error
		{

			return Ok();

		}




		[HttpGet("unauthorized")]                 // Get : / aPi/ Buggy/ unauthorized 
		public ActionResult GetUnauthorizedError()              // validation Error
		{

			return Unauthorized( new ApiResponse( 401 ) );
		}





	}
}
