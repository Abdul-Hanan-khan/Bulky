using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BulkyWeb.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required, DisplayName("Category Name"), MaxLength(50), MinLength(3,ErrorMessage ="Minimum Length For Category Name is 3")]
        
        public string Name { get; set; }
        [Required, DisplayName("Display Order"), Range(1,200, ErrorMessage ="Please Enter a Number Between 1 and 200")]
        public int  DisplayOrder { get; set; }
        public bool IsVerified { get; set; } = false;
    }
}
