using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorthwindAPI.Models;
using System;
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
            try
            {
                return await db.Employees.Select(e => new Employee
                {
                    EmployeeId = e.EmployeeId,
                    LastName = e.LastName,
                    FirstName = e.FirstName,
                    Title = e.Title,
                    ReportsTo = null // Estetään mahdollinen looppi JSON-serialisoinnissa
                }).ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Sisäinen palvelinvirhe: {ex.Message}");
            }
        }

        // GET: api/Employees/5
        // Tämä metodi hakee yhden työntekijän ID:n perusteella
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            try
            {
                var employee = await db.Employees.FindAsync(id);
                if (employee == null)
                    return NotFound("Työntekijää ei löydy");

                // Nullataan 'ReportsTo' ja 'Photo' ennen palautusta
                employee.ReportsTo = null;
                employee.Photo = null; // Nullataan työntekijän kuva
                return employee;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Sisäinen palvelinvirhe: {ex.Message}");
            }
        }

        // GET: api/Employees/bytitle?title=Manager
        // Tämä metodi hakee työntekijöitä tietyn aseman perusteella
        [HttpGet("bytitle")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesByTitle(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return BadRequest("Parametri 'title' on pakollinen ja sen tulee sisältää hakusana.");
            }

            try
            {
                // Suoritetaan tietokantakysely

                return await db.Employees
                    .Where(e => e.Title != null && e.Title.Contains(title))
                    .Select(e => new Employee
                    {
                        EmployeeId = e.EmployeeId,
                        LastName = e.LastName,
                        FirstName = e.FirstName,
                        Title = e.Title,
                        ReportsTo = null // Estetään mahdollinen looppi
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Sisäinen palvelinvirhe: {ex.Message}");
            }
        }

        // POST: api/Employees
        // Tämä metodi lisää uuden työntekijän tietokantaan
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            try
            {
                employee.ReportsTo = null;
                employee.Photo = null; // Nullataan kuva
                db.Employees.Add(employee);
                await db.SaveChangesAsync();

                // Palautetaan luotu työntekijä ja HTTP 201 statuskoodi
                return CreatedAtAction("GetEmployee", new { id = employee.EmployeeId }, employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Sisäinen palvelinvirhe: {ex.Message}");
            }
        }

        // PUT: api/Employees/5
        // Tämä metodi päivittää olemassa olevan työntekijän tietoja
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, Employee employee)
        {
            // Tarkistetaan, että ID:t täsmäävät
            if (id != employee.EmployeeId)
                return BadRequest("Väärä employee ID");

            try
            {
                employee.ReportsTo = null;
                // Merkitään työntekijä muokattavaksi ja tallennetaan muutokset
                db.Entry(employee).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return NoContent(); // Ei palauteta sisältöä, mutta ilmoitetaan onnistumisesta
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Sisäinen palvelinvirhe: {ex.Message}");
            }
        }

        // DELETE: api/Employees/5
        // Tämä metodi poistaa työntekijän tietokannasta
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                // Etsitään työntekijä tietokannasta käyttäen annettua id:tä.

                var employee = await db.Employees.FindAsync(id);

                // Jos työntekijää ei löydy, palautetaan 404 Not Found -vastaus ja viesti "Työntekijää ei löydy".

                if (employee == null)
                    return NotFound("Työntekijää ei löydy");

                // Poistetaan työntekijä tietokannasta
                // Jos työntekijä löytyy, poistetaan se tietokannasta.

                db.Employees.Remove(employee);
                // Tallennetaan muutokset tietokantaan asynkronisesti.

                await db.SaveChangesAsync();
                // Palautetaan 204 No Content -vastaus, mikä tarkoittaa, että pyyntö onnistui mutta sisältöä ei palauteta.

                return NoContent();
            }
            catch (Exception ex)
            {
                // Jos jossain vaiheessa tapahtuu virhe, palautetaan 500 Internal Server Error -vastaus
                // ja lisätietoa virheestä sisällytettynä viestissä.
                return StatusCode(500, $"Sisäinen palvelinvirhe: {ex.Message}");
            }
        }
    }
}
