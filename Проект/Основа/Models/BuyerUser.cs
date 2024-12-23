namespace Blackberries.Models
{
    public class BuyerUser : IAppUser
    {
        private string email;
        private long id;
        private string name;
        private string telephone;

        public BuyerUser(string email, long id, string name, string telephone)
        {
            this.email = email;
            this.id = id;
            this.name = name;
            this.telephone = telephone;
        }
        public string Email => this.email;
        public long Id => this.id;
        public string Name => this.name;
        public string Telephone => this.telephone;

        public UserRole Role => UserRole.Buyer;
    }
}
