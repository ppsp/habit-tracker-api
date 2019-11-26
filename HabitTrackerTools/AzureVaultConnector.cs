using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

namespace HabitTrackerTools
{
    public class AzureVaultConnector
    {
        private string VaultName { get; set; }

        public AzureVaultConnector(string vaultName)
        {
            this.VaultName = vaultName;
        }

        public string GetSecretValueString(string key)
        {
            Logger.Debug("Getting secret for key : " + key);
            Logger.Debug("Getting secret for uri : " + $"https://{this.VaultName}.vault.azure.net/secrets/{key}");

            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            var secretJson = keyVaultClient.GetSecretAsync($"https://{this.VaultName}.vault.azure.net/secrets/{key}")
                                           .ConfigureAwait(false).GetAwaiter().GetResult();

            return secretJson.Value;
        }
    }
}
