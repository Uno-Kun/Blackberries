namespace Blackberries.Models
{
    public interface IAppUser
    {
        long Id { get; }
        string Email { get; }
        string Name { get; }
        UserRole Role { get; }
    }
}
