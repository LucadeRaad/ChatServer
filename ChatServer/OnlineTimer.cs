using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatServer
{
    public class OnlineManager
    {
        private const int OnlineTimeoutInMs = 30000;

        private MyAppData _myAppData;

        private Dictionary<string, Timer> _onlineTimers = new Dictionary<string, Timer>();

        public OnlineManager(MyAppData myAppData)
        {
            _myAppData = myAppData;
        }

        private void SetPresence(string friendNameWithNewPresence, bool present)
        {
            foreach (var friendName in _myAppData.Friends.Keys)
            {
                foreach (var friendOfFriend in _myAppData.Friends[friendName])
                {
                    if (friendOfFriend.Name == friendNameWithNewPresence)
                    {
                        friendOfFriend.Presence = present;
                    }
                }
            }
        }

        private void OnlineTimer(object state)
        {
            var offlineFriendName = state as string;

            SetPresence(offlineFriendName, false);
        }

        public void FriendIsActive(string friendName)
        {
            if (_onlineTimers.ContainsKey(friendName))
            {
                _onlineTimers[friendName].Change(Timeout.Infinite, Timeout.Infinite);
            }

            _onlineTimers[friendName] = new Timer(OnlineTimer, friendName, OnlineTimeoutInMs, Timeout.Infinite);

            SetPresence(friendName, true);
        }

        public void DeleteFriend(string friendName)
        {
            if (_onlineTimers.ContainsKey(friendName))
            {
                _onlineTimers[friendName].Change(Timeout.Infinite, Timeout.Infinite);
            }
        }
    }
}