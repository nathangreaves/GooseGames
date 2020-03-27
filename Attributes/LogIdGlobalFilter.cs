using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Attributes
{
    public class LogIdGlobalFilter : IActionFilter
    {
        [ThreadStatic]
        private static Guid _LogId;

        public static Guid LogId { get { return _LogId; } }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _LogId = Guid.NewGuid();
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

    }
}
