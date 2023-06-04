using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ThrowException
{
    public class CustomerExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case ArgumentException argumentException: context.Result = new BadRequestObjectResult(argumentException.Message); break;
         //       case InvalidOperationException : context.Result = new StatusCodeResult(500); break;
                case Exception ex: context.Result = new BadRequestObjectResult(ex.Message); break;
                default: context.Result = new BadRequestResult(); break;
            }
        }
    }
}
