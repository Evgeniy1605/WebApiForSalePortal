using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalePortal.Data;
using SalePortal.Entities;

namespace WebApiForSalePortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommoditiesController : ControllerBase
    {
        private readonly SalePortalDbConnection _context;

        public CommoditiesController(SalePortalDbConnection context)
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommodityEntity>>> Getcommodities()
        {
            var result = _context.commodities.Include(o => o.Owner).Include(t => t.Type);
            return await result.ToListAsync();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<CommodityEntity>> GetCommodityEntity(int id)
        {
            var result = await _context.commodities.Include(x => x.Owner).Include(x => x.Type).SingleOrDefaultAsync(x => x.Id == id);

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutCommodityEntity(int id, CommodityEntity commodityEntity)
        {
            if (id != commodityEntity.Id)
            {
                return BadRequest();
            }
            var commotity = await _context.commodities.SingleOrDefaultAsync(x => x.Id == id);
            if (commotity == null)
            {
                return NotFound();
            }
            commotity.Name = commodityEntity.Name;
            commotity.Price = commodityEntity.Price;
            commotity.Description = commodityEntity.Description;
            _context.commodities.Update(commotity);


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommodityEntityExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<CommodityEntity>> PostCommodityEntity(CommodityEntity commodityEntity)
        {
            var type = await _context.Categories.SingleOrDefaultAsync(x => x.Id == commodityEntity.TypeId);
            if (type == null) { return BadRequest(); };
            var owner = await _context.Users.SingleOrDefaultAsync(_ => _.Id == commodityEntity.OwnerId);
            if (owner == null) { return BadRequest(); };
            commodityEntity.Owner= owner;
            commodityEntity.Type=type;

            _context.commodities.Add(commodityEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCommodityEntity", new { id = commodityEntity.Id }, commodityEntity);
        }

      
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommodityEntity(int id)
        {
            var commodityEntity = await _context.commodities.FindAsync(id);
            if (commodityEntity == null)
            {
                return NotFound();
            }

            _context.commodities.Remove(commodityEntity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CommodityEntityExists(int id)
        {
            return _context.commodities.Any(e => e.Id == id);
        }
    }
}
