using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TagHelperPack.Sample.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "First name", Prompt = "Enter customer's first name",
            Description = "The customer's first name.")]
        public string FirstName { get; set; } = default!;

        [Required]
        [StringLength(100)]
        [Display(Name = "Last name", Prompt = "Enter customer's last name", Description = "The customer's last name.")]
        public string LastName { get; set; } = default!;

        [Display(Name = "Birth date", Description = "The customer's date of birth.")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        public IList<Order> Orders { get; set; } = new List<Order>();
    }
}
