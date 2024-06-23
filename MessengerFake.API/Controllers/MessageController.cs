using AutoMapper;
using MessengerFake.API.Extensions;
using MessengerFake.API.Models.DTOs;
using MessengerFake.API.Models.Entities;
using MessengerFake.API.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessengerFake.API.Controllers
{
    [Authorize]
    public class MessageController(IUnitOfWork unitOfWork,
    IMapper mapper) : BaseApiController
    {
        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUsername();

            if (username == createMessageDto.RecipientUsername.ToLower())
            {
                return BadRequest("Bạn không thể gửi tin nhắn cho chính mình");
            }

            var sender = await unitOfWork.UserRepository.GetUserByUsernameAsync(username); // username người dùng gửi

            var recipient = await unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername); // username người dùng nhận

            if (recipient == null || sender == null || sender.UserName == null || recipient.UserName == null)
            {
                return BadRequest("Không thể gửi tin nhắn vào lúc này");
            }

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            unitOfWork.MessageRepository.AddMessage(message);
        }
    }
}
