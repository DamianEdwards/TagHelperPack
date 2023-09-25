using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TagHelperPack.Sample2.Models;

public class Customer
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "First name", Prompt = "Enter customer's first name", Description = "The customer's first name.")]
    public string FirstName { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Last name", Prompt = "Enter customer's last name", Description = "The customer's last name.")]
    public string LastName { get; set; }

    [Display(Name = "Birth date", Description = "The customer's date of birth.")]
    [DataType(DataType.Date)]
    public DateTime? BirthDate { get; set; }

    [Display(Name = "Country", Description = "The customer's country.")]
    [DataType(DataType.Text)]
    public string Country { get; set; }

    public IList<Order> Orders { get; set; } = new List<Order>();
}
