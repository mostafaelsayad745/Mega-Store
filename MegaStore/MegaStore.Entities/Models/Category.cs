﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
