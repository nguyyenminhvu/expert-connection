using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace Service.ThrowException
{
    public class CustomerExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case ArgumentException argumentException: context.Result = new BadRequestObjectResult(argumentException.Message); break;
                case Exception ex: context.Result = new BadRequestObjectResult(ex.Message); break;
                default: context.Result = new BadRequestResult(); break;
            }
        }
    }
}
