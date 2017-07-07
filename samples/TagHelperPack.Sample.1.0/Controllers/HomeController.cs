using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TagHelperPack.Sample.Models;

namespace TagHelperPack.Sample.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var customer = new Customer
            {
                FirstName = "Elizabeth",
                LastName = "Edwards",
                BirthDate = new DateTime(2005, 8, 8)
            };
            customer.Orders.Add(new Order { Customer = customer, CustomerId = customer.Id, Id = 1, PlacedOn = new DateTime(2017, 2, 4, 15, 22, 0), Total = 342.39m });
            customer.Orders.Add(new Order { Customer = customer, CustomerId = customer.Id, Id = 2, PlacedOn = new DateTime(2017, 3, 21, 11, 04, 0), Total = 1983.44m });
            return View(customer);
        }

        public IActionResult IndexPost(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", customer);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
