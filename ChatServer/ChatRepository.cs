using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatServer
{
    public class ChatRepository
    {
        private const string TableName = "Chats";

        private readonly IDatabaseClient _client;

        private readonly MyAppData _myAppData;

        public ChatRepository(
            IDatabaseClient client,
            MyAppData myAppData
            )
        {
            _client = client;
            _myAppData = myAppData;
        }

        public IDatabaseClient getClient()
        {
            return _client;
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
                        AttributeName = "Sender",
                        AttributeType = "S"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "Recipient",
                        AttributeType = "S"
                    }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "Sender",
                        KeyType = "HASH"
                    },
                    new KeySchemaElement
                    {
                        AttributeName = "Recipient",
                        KeyType = "RANGE"
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

        public async Task SaveChatAsync(Chat chat)
        {
            var request = new PutItemRequest
            {
                TableName = TableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    {"Sender", new AttributeValue {S = chat.Author}},
                    {"Recipient", new AttributeValue {S = chat.Recipient}},
                    {"Text", new AttributeValue {S = chat.Message}},
                    {"Read", new AttributeValue {S = chat.Read ? "Y" : "N"}},
                    {"Time", new AttributeValue {S = chat.Date.ToString()}}
                }
            };

            await _client.PutItemAsync(request);
        }

        public async void ScanDynamoDBAsync()
        {
            var request = new ScanRequest
            {
                TableName = TableName,
            };

            var response = await _client.ScanAllAsync(request);

            _myAppData.Messages = new Dictionary<string, Dictionary<string, List<Chat>>>();

            foreach (var responseItem in response.Items)
            {
                var sender = responseItem["Sender"].S;
                var recipient = responseItem["Recipient"].S;
                var text = responseItem["Text"].S;
                var read = responseItem["Read"].S;
                var time = responseItem["Time"].S;

                var chat = new Chat()
                {
                    Date = DateTime.Parse(time),

                    Message = text,

                    Author = sender,

                    Recipient = recipient,

                    Read = read == "Y" ? true : false
                };

                if (!_myAppData.Messages.ContainsKey(sender))
                {
                    _myAppData.Messages[sender] = new Dictionary<string, List<Chat>>();
                }

                if (!_myAppData.Messages[sender].ContainsKey(recipient))
                {
                    _myAppData.Messages[sender][recipient] = new List<Chat>();
                }

                _myAppData.Messages[sender][recipient].Add(chat);
            }
        }
    }
}