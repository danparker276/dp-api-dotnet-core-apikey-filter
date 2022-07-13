using dp.api.Models;
using Microsoft.AspNetCore.Mvc;

namespace dp.api.Controllers
{
    public class BaseController : ControllerBase
    {
        protected ClaimedUser GetClaimedUser()
        {
            return (ClaimedUser)HttpContext.Items["User"];

        }
    }
}