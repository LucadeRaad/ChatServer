using Amazon.Runtime;

namespace ChatServer
{
    public class AwsCredentials : AWSCredentials
    {
        public AwsCredentials()
        {
        }

        public override ImmutableCredentials GetCredentials()
        {
            return new ImmutableCredentials("myaccesskey", "mysecretkey", null); // local DynamoDB emulator ignores credentials
        }
    }
}