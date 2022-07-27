using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Extensions.Caching;
using Amazon.SecretsManager.Model;

namespace Demo.AWS.Secrets
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var mySecretName = "MySecret";

            var secret = await GetSecretFromCacheAsync(mySecretName);

            if (string.IsNullOrWhiteSpace(secret))
            {
                secret = await GetSecretAsync(mySecretName);
            }

            Console.WriteLine("shhh!!! my secret: " + secret ?? "-- not found --");
        }

        private static async Task<string?> GetSecretAsync(string secretName)
        {
            var config = new AmazonSecretsManagerConfig { RegionEndpoint = RegionEndpoint.USEast2 };
            IAmazonSecretsManager client = new AmazonSecretsManagerClient("<AWS-ACCESS-KEY-ID>", "<AWS-SECRET-ACCESS-KEY>", config);

            GetSecretValueRequest request = new GetSecretValueRequest()
            {
                SecretId = secretName,
                VersionStage = "AWSCURRENT" // VersionStage defaults to AWSCURRENT if unspecified.
            };

            GetSecretValueResponse? response = null;

            try
            {
                response = await client.GetSecretValueAsync(request);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return response?.SecretString;
        }

        private static async Task<string> GetSecretFromCacheAsync(string secretName)
        {
            SecretsManagerCache cache = new();
            return await cache.GetSecretString(secretName);
        }
    }
}