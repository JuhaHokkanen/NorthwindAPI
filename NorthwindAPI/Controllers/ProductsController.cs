﻿using Microsoft.AspNetCore.Mvc;
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
                var product = await db.Products.FindAsync(id);
                if (product == null)
                    return NotFound();
                return product;
            }
            catch (Exception ex)
            {
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

                return await db.Products.Where(p => p.ProductName.Contains(name)).ToListAsync();
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
        // Päivittää olemassa olevan tuotteen tiedot
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
                if (!ProductExists(id))
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
                //mutta vastaukseen ei liity sisältöä. Tämä on yleinen käytäntö RESTful-rajapinnoissa,
                //erityisesti DELETE-operaatioissa, koska kun resurssi on poistettu, sen palauttaminen ei ole tarpeen.
                //Tämä selkeyttää API:n käytöstä ja vähentää tarpeettomien tietojen siirtoa asiakkaan ja palvelimen välillä.
            }
            catch (Exception ex)
            {
                // Jos virhe tapahtuu, palautetaan 500 Internal Server Error -vastaus virheviestin kanssa.
                return StatusCode(500, $"Sisäinen palvelinvirhe: {ex.Message}");
            }
        }

        // Apumetodi, jolla tarkistetaan, onko tuote olemassa
        private bool ProductExists(int id)
        {
            // Tarkistetaan LINQ:n Any-metodilla, löytyykö tuotteita, joiden ProductId vastaa annettua id:tä.
            // Palauttaa true, jos ainakin yksi vastaava tuote löytyy, muuten false.
            return db.Products.Any(e => e.ProductId == id);
        }
    }
}
