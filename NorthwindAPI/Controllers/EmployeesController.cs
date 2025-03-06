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

        // Konstruktori, joka injektoi NorthwindOriginalContextin
        public EmployeesController(NorthwindOriginalContext dbparametri)
        {
            db = dbparametri;
        }

        // GET: api/Employees
        // Tämä metodi palauttaa kaikki työntekijät tietokannasta
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
        // Tämä metodi hakee yhden työntekijän ID:n perusteella
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await db.Employees.FindAsync(id);
            if (employee == null) return NotFound("Työntekijää ei löydy");

            // Nullataan 'ReportsTo' ja 'Photo' ennen palautusta

            employee.ReportsTo = null;
            employee.Photo = null; // Nullataan työntekijän kuva
            return employee;
        }

        // GET: api/Employees/bytitle?title=Manager
        // Tämä metodi hakee työntekijöitä tietyn aseman perusteella

        [HttpGet("bytitle")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesByTitle(string title)
        {
            // Haetaan työntekijöitä, joiden 'Title' kenttä sisältää annetun arvon
            return await db.Employees.Where(e => e.Title.Contains(title)).Select(e => new Employee
            {
                EmployeeId = e.EmployeeId,
                LastName = e.LastName,
                FirstName = e.FirstName,
                Title = e.Title,
                ReportsTo = null // Estetään mahdolline looppi
            }).ToListAsync();
        }

        // POST: api/Employees
        // Tämä metodi lisää uuden työntekijän tietokantaan

        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            employee.ReportsTo = null;
            db.Employees.Add(employee);
            await db.SaveChangesAsync();

            // Palautetaan luotu työntekijä ja HTTP 201 statuskoodi

            return CreatedAtAction("GetEmployee", new { id = employee.EmployeeId }, employee);
        }

        // PUT: api/Employees/5
        // Tämä metodi päivittää olemassa olevan työntekijän tietoja

        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, Employee employee)
        {
            // Tarkistetaan, että ID:t täsmäävät

            if (id != employee.EmployeeId) return BadRequest("Väärä employee ID");
            employee.ReportsTo = null;

            // Merkitään työntekijä muokattavaksi ja tallennetaan muutokset

            db.Entry(employee).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return NoContent(); // Ei palauteta sisältöä, mutta ilmoitetaan onnistumisesta
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await db.Employees.FindAsync(id);
            if (employee == null) return NotFound("Työntekijää ei löydy");

            // Poistetaan työntekijä tietokannasta

            db.Employees.Remove(employee);
            await db.SaveChangesAsync();
            return NoContent();
        }
    }
}
