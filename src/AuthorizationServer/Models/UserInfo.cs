using System.Collections.Generic;

namespace AuthorizationServer.Models
{
    public class UserInfo
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public static IList<UserInfo> GetAllUsers()
        {
            return new List<UserInfo> {
                new UserInfo { UserName = "ganagus", Password = "12345", ClientId = "100", ClientSecret = "0000" },
                new UserInfo { UserName = "chen", Password = "12345", ClientId = "200", ClientSecret = "0000" }
            };
        }
    }
}
