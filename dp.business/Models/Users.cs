using dp.business.Enums;

namespace dp.business.Models
  
{
    public class User
    {
        public int UserId { get; set; }
        public Role Role { get; set; }
        public bool IsActive { get; set; }
        public string Email { get; set; }
    }

    public class UserResponse : User
    {
        public string Token { get; set; }
    }
    public class UserCreateRequest { 
        public string Email { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
    }
}