using dp.api.Models;
using dp.api.Services;
using dp.api.Authorization;
using dp.business.Enums;
using dp.business.Helpers;
using dp.business.Models;
using dp.data;
using dp.data.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace dp.api.Controllers
{

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private string _dpDbConnectionString;
        private IDaoFactory AdoNetDao => DaoFactories.GetFactory(DataProvider.AdoNet, _dpDbConnectionString);

        public UsersController(IUserService userService)
        {
            _userService = userService;
            _dpDbConnectionString = Environment.GetEnvironmentVariable("dpDbConnectionString");
        }
        /// <summary>
        /// This main login call to get a token
        /// </summary>
        /// <param name="userParam"></param>
        /// <returns>UserResponse</returns>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<AccessToken>> Authenticate([FromBody]TokenRequest userParam)
        {
            AccessToken user = await _userService.Authenticate(userParam.Email, userParam.Password, (UserType)userParam.UserTypeId);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }

        [Authorize(Role.Admin)]
        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            //add skip/take... later on
            List<User> users = await AdoNetDao.UserDao.GetUserList();
            return Ok(users);
        }


        //Below are tasks for later on when we will have our users create with the new API and not service stack

        /// <summary>
        /// Create a new user from email
        /// </summary>
        /// <param name="user"></param>
        /// <returns>The New UserId or nothing if user exists</returns>

        [Authorize(Role.Admin)]
        [HttpPost("createuser")]
        public async Task<IActionResult> CreateUser([FromBody]UserCreateRequest user)
        {
            
            if (!Utils.IsValidEmail(user.Email))
                return BadRequest("Valid Email is requried");
            if (String.IsNullOrEmpty(user.Password) || user.Password.Length < 5)
                return BadRequest("Valid Password is requried"); 
            int? userId= await AdoNetDao.UserDao.CreateUser(user);
 
            return Ok(userId);
        }
        

        /// <summary>
        /// Get current information about a userId v2
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // only admins can access other user records
            var currentUser = (ClaimedUser)HttpContext.Items["User"];
            if (id != currentUser.Id && currentUser.Role != Role.Admin)
                return Unauthorized(new { message = "Unauthorized" });

            var user = await _userService.GetById(id);
            return Ok(user);
        }


    }
}