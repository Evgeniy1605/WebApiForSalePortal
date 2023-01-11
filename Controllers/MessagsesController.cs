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
    public class MessagsesController : ControllerBase
    {
        private readonly SalePortalDbConnection _context;

        public MessagsesController(SalePortalDbConnection context)
        {
            _context = context;
        }

        // GET: api/Messagses
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MessageEntity>>> GetMessages()
        {
            
            return await _context.Messages.Include(x => x.Sender).Include(x => x.Chat).ToListAsync();
        }

        // GET: api/Messagses/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<MessageEntity>> GetMessageEntity(int id)
        {
            var messageEntity = await _context.Messages.Include(x => x.Sender).Include(x => x.Chat).SingleOrDefaultAsync(x => x.Id == id);

            if (messageEntity == null)
            {
                return NotFound();
            }

            return messageEntity;
        }

        // PUT: api/Messagses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutMessageEntity(int id, MessageEntity messageEntity)
        {
            if (id != messageEntity.Id || messageEntity.SenderId == 0|| messageEntity.ChatId == 0)
            {
                return BadRequest();
            }
            var sender = await _context.Users.SingleOrDefaultAsync(x => x.Id == messageEntity.SenderId);
            var chat = await _context.Chats.SingleOrDefaultAsync(x => x.Id == messageEntity.ChatId);
            if (chat == null || sender == null)
            {
                return BadRequest();
            }
            messageEntity.Sender = sender;
            messageEntity.Chat = chat;

            _context.Messages.Update(messageEntity);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MessageEntityExists(id))
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

        // POST: api/Messagses

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<MessageEntity>> PostMessageEntity(MessageEntity messageEntity)
        {
            if (messageEntity.SenderId == 0 || messageEntity.ChatId == 0)
            {
                return BadRequest();
            }
            var sender = await _context.Users.SingleOrDefaultAsync(x => x.Id == messageEntity.SenderId);
            var chat = await _context.Chats.SingleOrDefaultAsync(x => x.Id == messageEntity.ChatId);
            if (chat == null || sender == null)
            {
                return BadRequest();
            }
            messageEntity.Sender = sender;
            messageEntity.Chat = chat;


            _context.Messages.Add(messageEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMessageEntity", new { id = messageEntity.Id }, messageEntity);
        }

        // DELETE: api/Messagses/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteMessageEntity(int id)
        {
            var messageEntity = await _context.Messages.FindAsync(id);
            if (messageEntity == null)
            {
                return NotFound();
            }

            _context.Messages.Remove(messageEntity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MessageEntityExists(int id)
        {
            return _context.Messages.Any(e => e.Id == id);
        }
    }
}
