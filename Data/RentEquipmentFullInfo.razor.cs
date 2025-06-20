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
        public int Quantity { get; set; }
        [Required]
        public decimal TotalCost { get; set; }
        [Required]
        public string FirstName { set; get; }
        [Required]
        public string LastName { set; get; }

    }
}
