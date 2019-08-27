using dp.business.Enums;
using dp.business.Models;
using dp.data;
using dp.data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace dp.api.Filters
{
    [AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAuthAtrribute : Attribute, IAsyncActionFilter
    {

        private const string ApiKeyHeaderName = "x-api-key";
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var requestApiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            if (!Guid.TryParse(requestApiKey, out var guidOutput))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            //check cache first
            string _dpDbConnectionString = Environment.GetEnvironmentVariable("dpDbConnectionString");
            IDaoFactory AdoNetDao = DaoFactories.GetFactory(DataProvider.AdoNet, _dpDbConnectionString);
           // int? userId = await AdoNetDao.UserDao.CheckUserAPIKey(potentialApiKey); //If we just want to get the userId for auth
            User user = await AdoNetDao.UserDao.GetUserFromAPIKey(requestApiKey);

            if (user == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            context.HttpContext.Items.Add("user", user);
            await next();
        }


    }
}
