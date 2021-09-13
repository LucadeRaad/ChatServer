using Amazon.DynamoDBv2;

namespace ChatServer
{
    public class DynamoDbClientFactory
    {
        public static AmazonDynamoDBClient CreateClient()
        {
            var dynamoDbConfig = new AmazonDynamoDBConfig
            {
                ServiceURL = "http://192.168.1.117:8000", // local DynamoDB emulator
            };

            var awsCredentials = new AwsCredentials();

            return new AmazonDynamoDBClient(awsCredentials, dynamoDbConfig);
        }
    }
}