using dp.api.Authorization;
using dp.api.Helpers;
using dp.business.Enums;
using dp.business.Models;
using dp.data;
using dp.data.Interfaces;
using Microsoft.Extensions.Options;

namespace dp.api.Services
{
    public interface IUserService
    {
        Task<AccessToken> Authenticate(string email, string password, Role userType);
        Task<User> GetById(int id);

    }

    public class UserService : IUserService
    {
        private IJwtUtils _jwtUtils;
        private readonly AppSettings _appSettings;
        private readonly ConnectionStrings _connectionStrings;

        private IDaoFactory AdoDao => DaoFactories.GetFactory(DataProvider.AdoNet, _connectionStrings.DpDbConnectionString);


        public UserService(IOptions<AppSettings> appSettings, IOptions<ConnectionStrings> connectionStrings, IJwtUtils jwtUtils)
        {
            _appSettings = appSettings.Value;
            _connectionStrings = connectionStrings.Value;
            _jwtUtils = jwtUtils;
        }

        public async Task<AccessToken> Authenticate(string email, string password, Role userType)
        {

            User user = await AdoDao.UserDao.ValidateUser(email, password, userType);

            // return null if user not found
            if (user == null)
                return null;
            if (user.IsActive == false)
                throw new AppException("Username or password is incorrect");
            var jwtToken = _jwtUtils.GenerateJwtToken(new User() { UserId = user.UserId, Role = user.Role });
            return new AccessToken() { access_token = jwtToken };


        }

        public async Task<User> GetById(int id)
        {
            return await AdoDao.UserDao.GetUserInfo(id);
        }
    }
}