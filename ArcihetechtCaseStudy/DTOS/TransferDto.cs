public class TransferDto
{
    public int Id { get; set; }
    public string SenderUsername { get; set; }
    public string ReceiverUsername { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}
