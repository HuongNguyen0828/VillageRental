using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VillageRental.Components.Pages
{
    public class RentAEquipmentFullInfo
    {
        [Required]
        public int ID { get; set; }
        [Required]

        public int EquipmentId { get; set; }
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int Quantity { get; set; }
        [Required]
        public DateTime RentalDate { get; set; }
        [Required]
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "Date must be in the format YYYY-MM-DD.")]
        public DateOnly StartDate { get; set; }
        [Required]
        public int Duration { get; set; }
        [Required]
        public decimal TotalCost { get; set; }
        public string FirstName { set; get; }
        public string LastName { set; get; }
        public string EquipmentName { get; set; }
        [Required]
        public string CategoryName { get; set; }

    }
}
