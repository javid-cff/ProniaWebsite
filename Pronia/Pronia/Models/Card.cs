using System.ComponentModel.DataAnnotations;

namespace Pronia.Models
{
    public class Card
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public bool isOnline { get; set; }
    }
}
