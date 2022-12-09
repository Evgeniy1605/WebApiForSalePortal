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
    public class OrdersController : ControllerBase
    {
        private readonly SalePortalDbConnection _context;

        public OrdersController(SalePortalDbConnection context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommodityOrderEntity>>> GetCommodityOrders()
        {
            return await _context.CommodityOrders
                .Include(x => x.Commodity)
                .Include(x => x.Customer)
                .Include(x => x.CommodityOwner).ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CommodityOrderEntity>> GetCommodityOrderEntity(int id)
        {
            if (id == 0 || id < 0 )
            {
                return BadRequest();
            }
            var commodityOrderEntity = await _context.CommodityOrders
                .Include(x => x.Commodity)
                .Include(x => x.Customer)
                .Include(x => x.CommodityOwner).SingleOrDefaultAsync(x => x.Id == id);

            if (commodityOrderEntity == null)
            {
                return NotFound();
            }

            return commodityOrderEntity;
        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCommodityOrderEntity(int id, CommodityOrderEntity commodityOrderEntity)
        {
            if (id != commodityOrderEntity.Id || commodityOrderEntity.CommodityId == 0 || commodityOrderEntity.CommodityOwnerId == 0 || commodityOrderEntity.CustomerId == 0 || commodityOrderEntity.CommodityOwnerId == 0)
            {
                return BadRequest();
            }
            var customer = await _context.Users.SingleOrDefaultAsync(x => x.Id == commodityOrderEntity.CustomerId);
            var commodity = await _context.commodities
                .Include(x => x.Owner)
                .Include(x => x.Type).SingleOrDefaultAsync(x => x.Id == commodityOrderEntity.CommodityId);
            var commodityOwner = await _context.Users.SingleOrDefaultAsync(x => x.Id == commodityOrderEntity.CommodityOwnerId);
            if (customer == null || commodity == null || commodityOwner == null)
            {
                return BadRequest();
            }
            commodityOrderEntity.Customer = customer;
            commodityOrderEntity.Commodity = commodity;
            commodityOrderEntity.CommodityOwner= commodityOwner;
            _context.CommodityOrders.Update(commodityOrderEntity);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommodityOrderEntityExists(id))
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

        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<CommodityOrderEntity>> PostCommodityOrderEntity(CommodityOrderEntity commodityOrderEntity)
        {
            var customer = await _context.Users.SingleOrDefaultAsync(x => x.Id == commodityOrderEntity.CustomerId);
            var commodity = await _context.commodities
                .Include(x => x.Owner)
                .Include(x => x.Type).SingleOrDefaultAsync(x => x.Id == commodityOrderEntity.CommodityId);
            var commodityOwner = await _context.Users.SingleOrDefaultAsync(x => x.Id == commodityOrderEntity.CommodityOwnerId);
            if (customer == null || commodity == null || commodityOwner == null)
            {
                return BadRequest();
            }
            commodityOrderEntity.Customer = customer;
            commodityOrderEntity.Commodity= commodity;
            commodityOrderEntity.CommodityOwner= commodityOwner;

            await _context.CommodityOrders.AddAsync(commodityOrderEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCommodityOrderEntity", new { id = commodityOrderEntity.Id }, commodityOrderEntity);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommodityOrderEntity(int id)
        {
            var commodityOrderEntity = await _context.CommodityOrders.FindAsync(id);
            if (commodityOrderEntity == null)
            {
                return NotFound();
            }

            _context.CommodityOrders.Remove(commodityOrderEntity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CommodityOrderEntityExists(int id)
        {
            return _context.CommodityOrders.Any(e => e.Id == id);
        }
    }
}
