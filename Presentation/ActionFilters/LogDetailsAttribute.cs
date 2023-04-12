using EntityLayer.LogModel;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Services.Abstract;


namespace Presentation.ActionFilters
{
    public class LogDetailsAttribute : ActionFilterAttribute
    {
        readonly ILoggerService loggerService;
        public LogDetailsAttribute(ILoggerService loggerService)
        {
            this.loggerService = loggerService;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            loggerService.Info(Log("Onexecuting", context.RouteData));
        }
        private string Log(string ModelName, RouteData routeData)
        {
            var logDetails = new LogDetails()
            {
                Controller = routeData.Values["controller"],
                Action = routeData.Values["action"],
                ModelName = ModelName
            };
            if (routeData.Values.Count >= 3)
            {
                logDetails.Id = routeData.Values["id"];
            }
            return logDetails.ToString();
        }
    }
}
