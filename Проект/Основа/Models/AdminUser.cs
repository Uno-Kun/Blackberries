namespace Blackberries.Models
{
    public class AdminUser : IAppUser
    {
        private string email;

        private string name;

        public AdminUser(string email, string name)
        {
            this.email = email;
            this.name = name;
        }

        public long Id => 1;

        public string Email => this.email;

        public string Name => this.name;

        public UserRole Role => UserRole.Admin;
    }
}
