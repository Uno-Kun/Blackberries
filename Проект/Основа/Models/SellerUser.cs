namespace Blackberries.Models
{
    public class SellerUser : IAppUser
    {
        private string email;
        private long id;
        private string telephone;
        private string name;
        public SellerUser(string email, long id, string telephone, string name) 
        {
            this.email = email;
            this.id = id;
            this.telephone = telephone;
            this.name = name;
        }
        public string Email => this.email;
        public long Id => this.id;
        public string Name => this.name;
        public string Telephone => this.telephone;
        
        public UserRole Role => UserRole.Seller;
    }
}
