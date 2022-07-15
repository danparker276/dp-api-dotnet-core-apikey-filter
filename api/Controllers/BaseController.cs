using dp.business.Models;
using Microsoft.AspNetCore.Mvc;

namespace dp.api.Controllers
{
    public class BaseController : ControllerBase
    {
        protected User GetClaimedUser()
        {
            return (User)HttpContext.Items["User"];

        }
    }
}