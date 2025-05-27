namespace ArcihetechtCaseStudy.DTOS
{
    public class CreateTransactionDto
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
    }
}
