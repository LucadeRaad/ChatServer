using System.Collections.Generic;

namespace ChatServer
{
    public class MyAppData
    {
        public Dictionary<string, Dictionary<string, List<Chat>>> Messages = new Dictionary<string, Dictionary<string, List<Chat>>>();

        public Dictionary<string, List<Friend>> Friends = new Dictionary<string, List<Friend>>();
    }
}