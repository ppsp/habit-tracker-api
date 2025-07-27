using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Threading.Tasks;

namespace HyperTaskTools
{
    public class AzureVaultConnector
    {
        private readonly SecretClient _secretClient;

        public AzureVaultConnector(string vaultName)
        {
            if (string.IsNullOrEmpty(vaultName))
                throw new ArgumentNullException(nameof(vaultName));

            var vaultUri = new Uri($"https://{vaultName}.vault.azure.net");
            _secretClient = new SecretClient(vaultUri, new DefaultAzureCredential());
        }

        public async Task<string> GetSecretValueStringAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            try
            {
                KeyVaultSecret secret = await _secretClient.GetSecretAsync(key);
                return secret.Value;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to retrieve secret '{key}' from Key Vault.", ex);
            }
        }
    }
}