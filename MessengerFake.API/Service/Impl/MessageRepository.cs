using AutoMapper;
using AutoMapper.QueryableExtensions;
using MessengerFake.API.Data;
using MessengerFake.API.Helpers;
using MessengerFake.API.Models.DTOs;
using MessengerFake.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MessengerFake.API.Service.Impl
{
    public class MessageRepository(DataContext _context, IMapper _mapper) : IMessageRepository
    {
        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Connection?> GetConnection(string connectionId)
        {
            return await _context.Connections
                .FirstOrDefaultAsync(c => c.ConnectionId == connectionId);
        }

        public async Task<Group?> GetGroupForConnection(string connectionId)
        {
            return await _context.Groups
           .Include(x => x.Connections)
           .Where(x => x.Connections.Any(c => c.ConnectionId == connectionId))
           .FirstOrDefaultAsync();
        }

        public async Task<Message?> GetMessage(Guid id)
        {
            return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Group?> GetMessageGroup(string groupName)
        {
            return await _context.Groups
           .Include(x => x.Connections)
           .FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages
            .OrderByDescending(x => x.MessageSent)
            .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(x => x.Recipient.UserName == messageParams.Username
               && x.RecipientDeleted == false),
                "Outbox" => query.Where(x => x.Sender.UserName == messageParams.Username
                    && x.SenderDeleted == false),
                _ => query.Where(x => x.Recipient.UserName == messageParams.Username && x.DateRead == null
                    && x.RecipientDeleted == false)
            };

            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber,
            messageParams.PageSize);
        }

        public Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            throw new NotImplementedException();
        }

        public void RemoveConnection(Connection connection)
        {
            throw new NotImplementedException();
        }
    }
}
