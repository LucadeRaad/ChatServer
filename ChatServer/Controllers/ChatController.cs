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
        public List<Chat> Get(string author, string recipient, Boolean isReading)
        {
            if (null == author || null == recipient)
            {
                return null;
            }

            if (_myAppData.Messages.TryGetValue(author, out var authorMessages))
            {
                if (authorMessages.ContainsKey(recipient))
                {
                    Console.WriteLine("Got chats from " + author + " to " + recipient);

                    if (isReading)
                    {
                        var readChats = _myAppData.Messages[author][recipient];

                        foreach (var chat in readChats)
                        {
                            chat.Read = true;
                        }
                    }

                    var output = new List<Chat>(_myAppData.Messages[author][recipient]);

                    output.AddRange(_myAppData.Messages[recipient][author]);

                    output.Sort((m1, m2) => m1.Date.CompareTo(m2.Date));

                    return output;
                }
            }
            else
            {
                return null;
            }

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

            Console.WriteLine("Posted a chat from " + chat.Author + " to " + chat.Recipient);

            _myAppData.Messages[chat.Author][chat.Recipient].Add(chat);

            await _chatRepository.SaveChatAsync(chat);
        }
    }
}