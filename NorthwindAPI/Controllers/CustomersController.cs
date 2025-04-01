using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorthwindAPI.Models;

namespace NorthwindAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private NorthwindOriginalContext db;


        public CustomersController(NorthwindOriginalContext dbparametri)
        {
            db = dbparametri;
        }

        [HttpGet]
        public ActionResult GetAllCustomers()
        {
            var asiakkaat = db.Customers.ToList();
            return Ok(asiakkaat);
        }
        // POST: api/Customers
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            if (customer == null || string.IsNullOrEmpty(customer.CustomerId))
            {
                return BadRequest("Invalid customer data.");
            }

            db.Customers.Add(customer);
            await db.SaveChangesAsync();

            return Ok(customer);
                      
        }
    }
}
