using System;
using System.ComponentModel.DataAnnotations;
namespace ArcihetechtCaseStudy.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; } 

        [Required]
        public decimal Amount { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
