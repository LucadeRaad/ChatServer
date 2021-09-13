using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ILogger<ChatController> _logger;

        private readonly MyAppData _myAppData;

        private ChatRepository _chatRepository;

        public ChatController(
            ILogger<ChatController> logger,
            MyAppData myAppData,
            ChatRepository chatRepository)
        {
            _logger = logger;
            _myAppData = myAppData;
            _chatRepository = chatRepository;
        }

        [HttpGet]
        public List<Chat> Get(string author, string recipient)
        {
            if (null == author || null == recipient)
            {
                return null;
            }

            if (_myAppData.Messages.TryGetValue(author, out var authorMessages))
            {
                if (authorMessages.ContainsKey(recipient))
                {
                    return _myAppData.Messages[author][recipient];
                }
            }
            else
            {
                return null;
            }

            //if (_myAppData.Messages == null)
            //{
            //    return null;
            //}

            //if (_myAppData.Messages.ContainsKey(chat.Author))
            //{
            //    if (_myAppData.Messages[chat.Author].ContainsKey(chat.Recipient))
            //    {
            //        List<Chat> output = _myAppData.Messages[chat.Author][chat.Recipient];

            //        _myAppData.Messages[chat.Author][chat.Recipient] = new List<Chat>();

            //        return output.ToArray();
            //    }
            //}

            return null;
        }

        [HttpPost]
        public async Task PostAsync(Chat chat)
        {
            chat.Date = DateTime.Now;

            if (!_myAppData.Messages.ContainsKey(chat.Author))
            {
                _myAppData.Messages[chat.Author] = new Dictionary<string, List<Chat>>();
            }

            if (!_myAppData.Messages[chat.Author].ContainsKey(chat.Recipient))
            {
                _myAppData.Messages[chat.Author][chat.Recipient] = new List<Chat>();
            }

            _myAppData.Messages[chat.Author][chat.Recipient].Add(chat);

            await _chatRepository.SaveChatAsync(chat);
        }
    }
}