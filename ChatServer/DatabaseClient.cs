using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace ChatServer
{
    public class DatabaseClient : IDatabaseClient
    {
        private const string StatusUnknown = "UNKNOWN";
        private const string StatusActive = "ACTIVE";

        private readonly IAmazonDynamoDB _client;

        public DatabaseClient(IAmazonDynamoDB client)
        {
            _client = client;
        }

        public async Task CreateTableAsync(CreateTableRequest createTableRequest)
        {
            var status = await GetTableStatusAsync(createTableRequest.TableName);

            if (status != StatusUnknown)
            {
                return;
            }

            await _client.CreateTableAsync(createTableRequest);

            await WaitUntilTableReady(createTableRequest.TableName);
        }

        public async Task<GetItemResponse> GetItemAsync(GetItemRequest getItemRequest)
        {
            return await _client.GetItemAsync(getItemRequest);
        }

        public async Task PutItemAsync(PutItemRequest putItemRequest)
        {
            await _client.PutItemAsync(putItemRequest);
        }

        public async Task<UpdateItemResponse> UpdateItemAsync(UpdateItemRequest updateItemRequest)
        {
            return await _client.UpdateItemAsync(updateItemRequest);
        }

        public async Task<DeleteItemResponse> DeleteItemAsync(DeleteItemRequest request)
        {
            return await _client.DeleteItemAsync(request);
        }

        public async Task<ScanResponse> ScanAllAsync(ScanRequest request)
        {
            return await _client.ScanAsync(request);
        }

        private async Task<string> GetTableStatusAsync(string tableName)
        {
            try
            {
                var response = await _client.DescribeTableAsync(new DescribeTableRequest
                {
                    TableName = tableName
                });

                return response?.Table.TableStatus;
            }
            catch (ResourceNotFoundException)
            {
                return StatusUnknown;
            }
        }

        private async Task WaitUntilTableReady(string tableName)
        {
            var status = await GetTableStatusAsync(tableName);

            for (var i = 0; i < 10 && status != StatusActive; ++i)
            {
                await Task.Delay(500);
                status = await GetTableStatusAsync(tableName);
            }
        }
    }
}