namespace ArcihetechtCaseStudy.Models
{
    public class User
    {
        public int Id { get; set; } // Primary Key!

        public string Username { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!; 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public decimal Balance { get; set; } = 0;

    }
}
