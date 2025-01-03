using System.ComponentModel.DataAnnotations;

namespace BulkyWeb.Models
{
    public class Category11
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string DisplayOrder { get; set; }
    }
}
