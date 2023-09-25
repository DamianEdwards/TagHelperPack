using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TagHelperPack.Sample2.Models;

namespace TagHelperPack.Sample2.Pages;

public class IndexModel : PageModel
{
    [BindProperty,
     Display(Name = "Customer", Description = "The Customer Name")]
    public Customer Customer { get; set; }

    public IReadOnlyList<string> Countries { get; } = new List<string> { "DE", "FR", "US" };

    public void OnGet()
    {
        Customer = new Customer
        {
            Id = 123,
            FirstName = "Elizabeth",
            LastName = "Edwards",
            BirthDate = new DateTime(2005, 8, 8),
            Country = "US",
        };
        Customer.Orders.Add(new Order { Customer = Customer, CustomerId = Customer.Id, Id = 1, PlacedOn = new DateTime(2017, 2, 4, 15, 22, 0), Total = 342.39m });
        Customer.Orders.Add(new Order { Customer = Customer, CustomerId = Customer.Id, Id = 2, PlacedOn = new DateTime(2017, 3, 21, 11, 04, 0), Total = 1983.44m });
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        return RedirectToPage();
    }
}
