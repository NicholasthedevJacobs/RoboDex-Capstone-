using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoboDex__Capstone_
{
    public class ViewBagActionFilter : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            // for razor pages
            //if (context.Controller is PageModel)
            //{
            //    var controller = context.Controller as PageModel;
            //    controller.ViewData.Add("Avatar", $"~/avatar/empty.png");
            //    // or
            //    controller.ViewBag.Avatar = $"~/avatar/empty.png";

            //    //also you have access to the httpcontext & route in controller.HttpContext & controller.RouteData
            //}

            // for Razor Views
            if (context.Controller is Controller)
            {
                var controller = context.Controller as Controller;
               
                
                controller.ViewBag.Avatar = $"~/avatar/empty.png";

                //also you have access to the httpcontext & route in controller.HttpContext & controller.RouteData
            }

            base.OnResultExecuting(context);
        }
    }
}
