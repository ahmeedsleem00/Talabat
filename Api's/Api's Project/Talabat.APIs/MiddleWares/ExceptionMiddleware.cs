﻿//using Microsoft.EntityFrameworkCore;
//using System.Net;
//using System.Text.Json;
//using Talabat.APIs.Errors;

//namespace Talabat.APIs.MiddleWares
//{
//	public class ExceptionMiddleware
//	{
//		private readonly RequestDelegate _next;
//		private readonly ILogger<ExceptionMiddleware> _logger;
//		private readonly IHostEnvironment _env;

//		public ExceptionMiddleware(RequestDelegate next  , ILogger<ExceptionMiddleware> logger ,IHostEnvironment env ) 
//		{
//			_next = next;
//			_logger = logger;
//			_env = env;
//		}


//		public async Task InvokeAsync(HttpContext context)
//		{

//			try

//			{
//				await _next.Invoke( context );

//			}

//			catch  ( Exception ex ) 
			
//			{
//				_logger.LogError(ex , ex.Message );	
//				// Log Exception Database  [ Production ]

//			    context.Response.ContentType = "application/json";

//				context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;


//				var response = _env.IsDevelopment () ?
//                       new ApiExceptionResponse( 500 , ex.Message , ex.StackTrace.ToString() ) 
//					 : new ApiExceptionResponse(  500 );   // Production


//				var json = JsonSerializer.Serialize(response);

//				await context.Response.WriteAsync( json );

//			}
			



//		}




//	}
//}
