using Arak.BLL.DTO;
using Arak.BLL.Service.Abstraction;
using Arak.DAL.Entities;
using Arak.DAL.Models;
using Arak.DAL.Repository.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arak.BLL.Service.Implementation
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;

        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<DtoMessage> SendMessageAsync(DtoSendMessage dto, string senderId, string receiverId)
        {
            if (!await _messageRepository.UserExistsAsync(receiverId))
                throw new KeyNotFoundException("Receiver not found.");

            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = dto.Content,
                SentAt = DateTime.UtcNow
            };

            await _messageRepository.CreateAsync(message);
            await _messageRepository.SaveChangesAsync();

            // Reload with navigation properties so we can map names
            var saved = await _messageRepository.GetByIdWithUsersAsync(message.Id);
            return MapToDto(saved!);
        }

        public async Task<IEnumerable<DtoMessage>> GetConversationHistoryAsync(string currentUserId, string otherUserId, int page = 1, int pageSize = 50)
        {
            var messages = await _messageRepository.GetConversationAsync(currentUserId, otherUserId, page, pageSize);
            return messages.Select(MapToDto);
        }

        public async Task<IEnumerable<DtoConversation>> GetUserConversationsAsync(string currentUserId)
        {
            var results = await _messageRepository.GetUserConversationsAsync(currentUserId);

            return results.Select(r =>
            {
                var isCurrentSender = r.LatestMessage.SenderId == currentUserId;
                var otherUser = isCurrentSender ? r.LatestMessage.Receiver : r.LatestMessage.Sender;

                return new DtoConversation
                {
                    ParticipantId = isCurrentSender ? r.LatestMessage.ReceiverId : r.LatestMessage.SenderId,
                    LastMessage = r.LatestMessage.Content,
                    LastMessageTime = r.LatestMessage.SentAt,
                    UnreadCount = r.UnreadCount,
                    Participant = new DtoConversationParticipant
                    {
                        Name = otherUser?.Name ?? otherUser?.UserName ?? "Unknown",
                        Avatar = ""
                    }
                };
            }).ToList();
        }

        public async Task<bool> MarkAsReadAsync(int messageId, string currentUserId)
        {
            var message = await _messageRepository.GetByIdAsync(messageId);
            if (message == null || message.ReceiverId != currentUserId)
                return false;

            message.ReadAt = DateTime.UtcNow;

            await _messageRepository.UpdateAsync(message);
            await _messageRepository.SaveChangesAsync();

            return true;
        }

        public async Task<int> MarkConversationAsReadAsync(string currentUserId, string otherUserId)
        {
            return await _messageRepository.MarkConversationAsReadAsync(currentUserId, otherUserId);
        }

        /// <summary>
        /// Maps a Message entity (with loaded navigation properties) to a DtoMessage.
        /// </summary>
        private static DtoMessage MapToDto(Message m)
        {
            return new DtoMessage
            {
                Id = m.Id,
                SenderId = m.SenderId,
                SenderName = m.Sender?.Name ?? m.Sender?.UserName ?? "Unknown",
                ReceiverId = m.ReceiverId,
                ReceiverName = m.Receiver?.Name ?? m.Receiver?.UserName ?? "Unknown",
                Content = m.Content,
                SentAt = m.SentAt,
                ReadAt = m.ReadAt
            };
        }
    }
}
