using System.Security.Principal;

namespace Blackberries.Models
{
    public class UserIdentity : IIdentity
    {
        private string email;
        private long id;
        private string name;
        private UserRole role;

        public UserIdentity(long id, string email, string name, UserRole role) 
        {
            this.id = id;
            this.email = email;
            this.name = name;
            this.role = role;
        }

        public string? AuthenticationType => "Blackberries";

        public bool IsAuthenticated => true;

        public string? Name => this.email;

        public long Id => this.id;

        public string Email => this.email;

        public string DisplayName => this.name;

        public UserRole Role => this.role;
    }
}
