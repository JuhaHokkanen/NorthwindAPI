using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorthwindAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NorthwindAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private NorthwindOriginalContext db;

        public EmployeesController(NorthwindOriginalContext dbparametri)
        {
            db = dbparametri;
        }

        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            return await db.Employees.Select(e => new Employee
            {
                EmployeeId = e.EmployeeId,
                LastName = e.LastName,
                FirstName = e.FirstName,
                Title = e.Title,
                ReportsTo = null // Estetään looppi
            }).ToListAsync();
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await db.Employees.FindAsync(id);
            if (employee == null) return NotFound();
            employee.ReportsTo = null;
            employee.Photo = null; // Nullataan työntekijän kuva
            return employee;
        }

        // GET: api/Employees/bytitle?title=Manager
        [HttpGet("bytitle")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesByTitle(string title)
        {
            return await db.Employees.Where(e => e.Title.Contains(title)).Select(e => new Employee
            {
                EmployeeId = e.EmployeeId,
                LastName = e.LastName,
                FirstName = e.FirstName,
                Title = e.Title,
                ReportsTo = null
            }).ToListAsync();
        }

        // POST: api/Employees
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            employee.ReportsTo = null;
            db.Employees.Add(employee);
            await db.SaveChangesAsync();
            return CreatedAtAction("GetEmployee", new { id = employee.EmployeeId }, employee);
        }

        // PUT: api/Employees/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, Employee employee)
        {
            if (id != employee.EmployeeId) return BadRequest();
            employee.ReportsTo = null;
            db.Entry(employee).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await db.Employees.FindAsync(id);
            if (employee == null) return NotFound();
            db.Employees.Remove(employee);
            await db.SaveChangesAsync();
            return NoContent();
        }
    }
}
