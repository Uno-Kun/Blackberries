namespace Blackberries.Models
{
    public class AuthenticationResult
    {
        public bool Success { get; set; }

        public IAppUser? AppUser { get; set; }
    }
}
