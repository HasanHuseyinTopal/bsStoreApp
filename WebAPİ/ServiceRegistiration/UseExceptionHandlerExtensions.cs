using EntityLayer.ErrorModel;
using EntityLayer.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Services.Abstract;

namespace WebAPİ.ServiceRegistiration
{
    public static class UseExceptionHandlerExtensions
    {
        public static void UseCustomExceptionHandler(this WebApplication app, ILoggerService loggerService)
        {
            app.UseExceptionHandler(conifg =>
            {
                conifg.Run(async context =>
                {
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        loggerService.Error(contextFeature.Error.Message);
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = contextFeature.Error switch 
                        {
                            NotFoundException => StatusCodes.Status404NotFound,
                            _=> StatusCodes.Status500InternalServerError
                        };
                        await context.Response.WriteAsync(new ErrorModel()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = contextFeature.Error.Message
                        }.ToString());
                    }
                });
            });
        }
    }
}
