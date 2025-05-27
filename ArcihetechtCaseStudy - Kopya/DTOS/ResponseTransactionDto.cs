namespace ArcihetechtCaseStudy.DTOS
{
    public class ResponseTransactionDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
