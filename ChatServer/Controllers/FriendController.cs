using ChatServer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        public FriendController(
            ILogger<FriendController> logger,
            MyAppData myAppData,
            FriendRepository friendRepository)
        {
            _logger = logger;
            _myAppData = myAppData;
            _friendRepository = friendRepository;
        }

        [HttpGet]
        public IEnumerable<Friend> Get(string name)
        {
            if (null == name)
            {
                return null;
            }

            if (_myAppData.Friends.TryGetValue(name, out var output))
            {
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

            foreach (var tempFriend in _myAppData.Friends[name])
            {
                if (tempFriend.Name == friend.Name)
                {
                    return;
                }
            }

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

            if (!_myAppData.Friends.ContainsKey(name))
            {
                return;
            }

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