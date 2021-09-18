using System;

namespace ChatServer
{
    public class Chat
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }

        public string Message { get; set; }

        public string Author { get; set; }

        public string Recipient { get; set; }

        public bool Read { get; set; }
    }
}