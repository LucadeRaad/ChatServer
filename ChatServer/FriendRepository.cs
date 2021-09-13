using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatServer
{
    public class FriendRepository
    {
        private const string TableName = "Friends";

        private readonly IDatabaseClient _client;

        private readonly MyAppData _myAppData;

        public FriendRepository(
            IDatabaseClient client,
            MyAppData myAppData
            )
        {
            _client = client;
            _myAppData = myAppData;
        }

        public async Task CreateTableAsync()
        {
            var request = new CreateTableRequest
            {
                TableName = TableName,
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = "Person",
                        AttributeType = "S"
                    }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "Person",
                        KeyType = "HASH"
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 10,
                    WriteCapacityUnits = 5
                }
            };

            await _client.CreateTableAsync(request);
        }

        public async Task SaveFriendAsync(string person, List<Friend> friends)
        {
            var newFriends = await GetFriendsAsync(person);
            var hasNewFriends = false;

            foreach (var friend in friends)
            {
                if (!newFriends.Contains(friend.Name))
                {
                    newFriends.Add(friend.Name);
                    hasNewFriends = true;
                }
            }

            if (hasNewFriends)
            {
                var request = new PutItemRequest
                {
                    TableName = TableName,
                    Item = new Dictionary<string, AttributeValue>
                    {
                        {"Person", new AttributeValue {S = person}},
                        {"Friends", new AttributeValue {SS = newFriends}}
                    }
                };

                await _client.PutItemAsync(request);
            }
        }

        public async Task<List<string>> GetFriendsAsync(string person)
        {
            var request = new GetItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    {"Person", new AttributeValue {S = person}},
                }
            };

            var response = await _client.GetItemAsync(request);

            if (response.Item.ContainsKey("Friends"))
            {
                return response.Item["Friends"].SS;
            }
            else
            {
                return new List<string>();
            }
        }

        public async void ScanDynamoDBAsync()
        {
            var request = new ScanRequest
            {
                TableName = TableName,
            };

            var response = await _client.ScanAllAsync(request);

            _myAppData.Friends = new Dictionary<string, List<Friend>>();

            foreach (var responseItem in response.Items)
            {
                var personName = responseItem["Person"].S;

                if (!_myAppData.Friends.ContainsKey(personName))
                {
                    _myAppData.Friends[personName] = new List<Friend>();
                }

                foreach (var friendName in responseItem["Friends"].SS)
                {
                    var friend = new Friend()
                    {
                        Name = friendName,
                        Presence = false
                    };

                    _myAppData.Friends[personName].Add(friend);
                }
            }
        }
    }
}