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
    public class ProductsController : ControllerBase
    {
        private NorthwindOriginalContext db;

        public ProductsController(NorthwindOriginalContext dbparametri)
        {
            db = dbparametri;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            try
            {
                return await db.Products.ToListAsync();
            }
            catch (Exception ex)
            {
                // Palautetaan 500 Internal Server Error, jos tapahtuu palvelimen päässä ongelmia
                return StatusCode(500, $"Sisäinen palvelinvirhe: {ex.Message}");
            }
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            try
            {
                // Yritetään löytää tuote tietokannasta annetulla id:llä
                var product = await db.Products.FindAsync(id);

                // Jos tuotetta ei löydy, palautetaan 404 Not Found -vastaus
                if (product == null)
                {
                    return NotFound("Tuotetta ei löydy");
                }

                // Jos tuote löytyy, palautetaan se 200 OK -vastauksena
                return Ok(product);
            }
            catch (Exception ex)
            {
                // Jos tulee jokin muu virhe, palautetaan 500 Internal Server Error -vastaus
                return StatusCode(500, $"Sisäinen palvelinvirhe: {ex.Message}");
            }
        }



        // GET: api/Products/byname?name=Chai
        // Hakee tuotteita, joiden nimi sisältää annetun hakusanan
        [HttpGet("byname")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByName(string name)
        {
            try
            {

                // Suoritetaan tietokantakysely, jossa etsitään tuotteita, joiden
                // 'ProductName' sisältää annetun 'name'-merkkijonon.
                // ToListAsync() kerää tulokset asynkronisesti listaksi.

                return await db.Products.Where(p => p.ProductName.ToLower().Contains(name.ToLower())).ToListAsync();
            }
            catch (Exception ex)
            {
                // Jos tapahtuu virhe, palautetaan HTTP 500 Internal Server Error -vastaus
                // ja virheilmoitus, joka sisältää poikkeuksen viestin.

                return StatusCode(500, $"Sisäinen palvelinvirhe: {ex.Message}");
            }
        }

        // POST: api/Products
        // Lisää uuden tuotteen tietokantaan
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            try
            {
                db.Products.Add(product);
                await db.SaveChangesAsync();
                return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Sisäinen palvelinvirhe: {ex.Message}");
            }
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.ProductId)
                return NotFound($"Tuotetta ei id:llä {id} löytynyt");

            try
            {
                db.Entry(product).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Tarkistetaan suoraan, onko tuotetta tietokannassa
                if (!db.Products.Any(e => e.ProductId == id))
                    return NotFound();
                else
                    throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Sisäinen palvelinvirhe: {ex.Message}");
            }
        }


        // DELETE: api/Products/5
        // Poistaa tuotteen tietokannasta
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                // Etsitään tietokannasta tuote, jonka id vastaa annettua id:tä.

                var product = await db.Products.FindAsync(id);

                // Jos tuotetta ei löydy, palautetaan 404 Not Found -vastaus ja ilmoitus "Tuotetta ei löydy".

                if (product == null)
                    return NotFound("Tuotetta ei löydy");

                // Jos tuote löytyy, poistetaan se tietokannasta.

                db.Products.Remove(product);

                // Tallennetaan muutokset tietokantaan asynkronisesti.
                await db.SaveChangesAsync();

                return NoContent();
                //palauttaa HTTP 204 No Content -statuskoodin, mikä tarkoittaa, että pyyntö on onnistuneesti suoritettu,
                
            }
            catch (Exception ex)
            {
                // Jos virhe tapahtuu, palautetaan 500 Internal Server Error -vastaus virheviestin kanssa.
                return StatusCode(500, $"Sisäinen palvelinvirhe: {ex.Message}");
            }
        }
    }
}
