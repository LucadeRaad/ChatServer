using Amazon.DynamoDBv2.DataModel;

namespace ChatServer
{
    [DynamoDBTable("friend")]
    public class Friend
    {
        [DynamoDBHashKey]
        public string Name { get; set; }

        public bool Presence { get; set; }
    }
}