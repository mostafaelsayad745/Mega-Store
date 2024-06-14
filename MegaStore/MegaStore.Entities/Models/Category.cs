using System.ComponentModel.DataAnnotations;

namespace MegaStore.Entities.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }=string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }=DateTime.Now;
    }
}
