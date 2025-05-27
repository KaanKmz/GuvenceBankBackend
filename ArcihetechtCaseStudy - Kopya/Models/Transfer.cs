namespace ArcihetechtCaseStudy.Models
{
    public class Transfer
    {
        public int Id { get; set; }

        public int SenderId { get; set; }
        public int ReceiverId { get; set; }

        public decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
