using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalePortal.Data;
using SalePortal.Entities;

namespace WebApiForSalePortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly SalePortalDbConnection _context;

        public ChatsController(SalePortalDbConnection context)
        {
            _context = context;
        }

        // GET: api/Chats
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChatEntity>>> GetChats()
        {
            return await _context.Chats
                .Include(x => x.Commodity)
                .Include(x => x.Seller)
                .Include(x => x.Customer).ToListAsync();
        }

        // GET: api/Chats/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ChatEntity>> GetChatEntity(int id)
        {
            var chatEntity = await _context.Chats.Include(x => x.Commodity)
                .Include(x => x.Seller)
                .Include(x => x.Customer).SingleOrDefaultAsync(x => x.Id == id);
            if (chatEntity == null)
            {
                return NotFound();
            }

            return chatEntity;
        }

        
        // POST: api/Chats

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ChatEntity>> PostChatEntity(ChatEntity chatEntity)
        {
            if (chatEntity == null || chatEntity.CommodityId == 0 || chatEntity.SellerId == 0 || chatEntity.CustomerId == 0) { return BadRequest(); }

            var commodity = await _context.commodities.Include(x => x.Owner)
                .Include(x => x.Type)
                .SingleOrDefaultAsync(x => x.Id== chatEntity.CommodityId);

            var seller = await _context.Users.SingleOrDefaultAsync(x => x.Id== chatEntity.SellerId);
            var customer = await _context.Users.SingleOrDefaultAsync(x => x.Id== chatEntity.CustomerId);

            if (commodity == null || seller == null || customer == null || seller.Id != commodity.OwnerId)
            {
                return BadRequest();
            }
            chatEntity.Commodity = commodity;
            chatEntity.Seller = seller;
            chatEntity.Customer = customer;
            _context.Chats.Add(chatEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetChatEntity", new { id = chatEntity.Id }, chatEntity);
        }

        
        // DELETE: api/Chats/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteChatEntity(int id)
        {
            var chatEntity = await _context.Chats.FindAsync(id);
            if (chatEntity == null)
            {
                return NotFound();
            }

            var messeges = await _context.Messages.Where(x => x.ChatId == id).ToListAsync();
            if (messeges.Count != 0)
            {
                foreach (var item in messeges)
                {
                    _context.Messages.Remove(item);
                    await _context.SaveChangesAsync();
                }
            }

            _context.Chats.Remove(chatEntity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ChatEntityExists(int id)
        {
            return _context.Chats.Any(e => e.Id == id);
        }
    }
}
