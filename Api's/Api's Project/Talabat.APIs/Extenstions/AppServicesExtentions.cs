using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Repositories.Interfaces;
using Talabat.Core.Services.Interfaces;
using Talabat.Repository;
using Talabat.Repository.Repositories;
using Talabat.Service;

namespace Talabat.APIs.Extenstions
{
	public static class AppServicesExtentions
	{

		public static IServiceCollection AddAppLicationsServices(this IServiceCollection services)
		{


			services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
			services.AddScoped<IUnitOfWork, UnitOfWork>();
			services.AddScoped<IOrderServices,OrderServices>();
			services.AddScoped<IPaymentServices, PaymentService>();


			//  webApplicationbuilder.Services.AddAutoMapper( M => M.AddProfile( new MappingProfile()  ) );
			services.AddAutoMapper(typeof(MappingProfile));







			 services.Configure<ApiBehaviorOptions>(options =>
			 { 

				options.InvalidModelStateResponseFactory = (actionContext) =>
				{
					var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count() > 0)
														 .SelectMany(P => P.Value.Errors)
														 .Select(E => E.ErrorMessage).ToArray();


					var validationErrorResponse = new ApiValidationErrorResponse()
					{
						Errors = errors
					};

					return new BadRequestObjectResult(validationErrorResponse);
				};

			});





			return services;
		}



	}
}
