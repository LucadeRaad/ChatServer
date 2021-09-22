using ChatServer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FriendServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FriendController : ControllerBase
    {
        private readonly ILogger<FriendController> _logger;

        private readonly MyAppData _myAppData;

        private FriendRepository _friendRepository;

        private readonly OnlineManager _onlineManager;

        public FriendController(
            ILogger<FriendController> logger,
            MyAppData myAppData,
            FriendRepository friendRepository,
            OnlineManager onlineManager
            )
        {
            _logger = logger;
            _myAppData = myAppData;
            _friendRepository = friendRepository;
            _onlineManager = onlineManager;
        }

        [HttpGet]
        public IEnumerable<Friend> Get(string name)
        {
            if (null == name)
            {
                return null;
            }

            _onlineManager.FriendIsActive(name);

            if (_myAppData.Friends.TryGetValue(name, out var output))
            {
                Console.WriteLine("Got friendlist of " + name);
                return output;
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public async Task PostAsync(string name, Friend friend)
        {
            if (friend.Name == name)
            {
                return;
            }

            if (!_myAppData.Friends.ContainsKey(name))
            {
                _myAppData.Friends[name] = new List<Friend>();
            }

            _onlineManager.FriendIsActive(name);

            foreach (var tempFriend in _myAppData.Friends[name])
            {
                if (tempFriend.Name == friend.Name)
                {
                    return;
                }
            }

            Console.WriteLine("Added friend " + friend.Name + " to " + name);

            _myAppData.Friends[name].Add(friend);

            await _friendRepository.SaveFriendAsync(name, _myAppData.Friends[name]);
        }

        [HttpDelete]
        public void DeleteFriend(string name, string friend)
        {
            if (null == name)
            {
                return;
            }

            if (null == friend)
            {
                friend = "";
            }

            if (!_myAppData.Friends.ContainsKey(name))
            {
                return;
            }

            _onlineManager.DeleteFriend(friend);

            foreach (var tempFriend in _myAppData.Friends[name])
            {
                if (tempFriend.Name == friend)
                {
                    _myAppData.Friends[name].Remove(tempFriend);

                    _friendRepository.DeleteFriend(name, tempFriend);

                    break;
                }
            }
        }
    }
}