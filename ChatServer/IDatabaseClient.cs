using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

namespace ChatServer
{
    public interface IDatabaseClient
    {
        Task CreateTableAsync(CreateTableRequest createTableRequest);

        Task<GetItemResponse> GetItemAsync(GetItemRequest getItemRequest);

        Task PutItemAsync(PutItemRequest putItemRequest);

        Task<UpdateItemResponse> UpdateItemAsync(UpdateItemRequest updateItemRequest);

        Task<ScanResponse> ScanAllAsync(ScanRequest request);

        Task<DeleteItemResponse> DeleteItemAsync(DeleteItemRequest request);
    }
}