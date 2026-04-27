using Arak.DAL.Database;
using Arak.DAL.Entities;
using Arak.DAL.Models;
using Arak.DAL.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arak.DAL.Repository.Implementation
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        public MessageRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Message>> GetConversationAsync(string userId1, string userId2, int page, int pageSize)
        {
            var messages = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                            (m.SenderId == userId2 && m.ReceiverId == userId1))
                .OrderByDescending(m => m.SentAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Return in chronological order for display
            messages.Reverse();
            return messages;
        }

        public async Task<IEnumerable<ConversationResult>> GetUserConversationsAsync(string userId)
        {
            // Step 1: Get the latest message ID and unread count for each conversation
            var conversationStats = await _context.Messages
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Select(g => new
                {
                    ParticipantId = g.Key,
                    LatestMessageId = g.OrderByDescending(m => m.SentAt).Select(m => m.Id).FirstOrDefault(),
                    UnreadCount = g.Count(m => m.ReceiverId == userId && m.ReadAt == null)
                })
                .ToListAsync();

            if (!conversationStats.Any())
                return Enumerable.Empty<ConversationResult>();

            // Step 2: Fetch the actual message entities with Includes
            var latestMessageIds = conversationStats.Select(s => s.LatestMessageId).ToList();

            var messages = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => latestMessageIds.Contains(m.Id))
                .ToListAsync();

            var results = new List<ConversationResult>();
            foreach (var stat in conversationStats)
            {
                var msg = messages.FirstOrDefault(m => m.Id == stat.LatestMessageId);
                if (msg != null)
                {
                    results.Add(new ConversationResult
                    {
                        LatestMessage = msg,
                        UnreadCount = stat.UnreadCount
                    });
                }
            }

            return results.OrderByDescending(r => r.LatestMessage.SentAt);
        }

        public async Task<bool> UserExistsAsync(string userId)
        {
            return await _context.ApplicationUsers.AnyAsync(u => u.Id == userId);
        }

        public async Task<Message?> GetByIdWithUsersAsync(int id)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<int> MarkConversationAsReadAsync(string currentUserId, string otherUserId)
        {
            var now = DateTime.UtcNow;
            return await _context.Messages
                .Where(m => m.SenderId == otherUserId
                         && m.ReceiverId == currentUserId
                         && m.ReadAt == null)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.ReadAt, now));
        }
    }
}
