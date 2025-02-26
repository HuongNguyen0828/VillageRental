using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VillageRental.Components.Pages
{
    public class EquipmentwithCategoryStr
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal DailyPrice { get; set; }

        [Required]
        public string CategoryStr { get; set; }

        public bool IsFavorite { get; set; }

    }
}
