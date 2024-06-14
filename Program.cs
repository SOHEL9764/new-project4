using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;

namespace KeyVault
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get environment variables for Azure AD authentication
            string? clientSecret = Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET");
            string? tenantId = Environment.GetEnvironmentVariable("AZURE_TENANT_ID");
            string? clientId = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID");

            // Check if any of the environment variables are null or empty
            if (string.IsNullOrEmpty(clientSecret))
            {
                Console.WriteLine("Environment variable AZURE_CLIENT_SECRET is missing or empty.");
                return;
            }

            if (string.IsNullOrEmpty(tenantId))
            {
                Console.WriteLine("Environment variable AZURE_TENANT_ID is missing or empty.");
                return;
            }

            if (string.IsNullOrEmpty(clientId))
            {
                Console.WriteLine("Environment variable AZURE_CLIENT_ID is missing or empty.");
                return;
            }

            // Build the client credentials
            var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);

            // Key Vault URL
            string keyVaultUrl = "https://eastuskeyvault01.vault.azure.net/";

            try
            {
                // Create a new SecretClient to access the Key Vault
                var client = new SecretClient(new Uri(keyVaultUrl), clientSecretCredential);

                // Fetch the secret from the Key Vault
                KeyVaultSecret secret = client.GetSecret("azuresql");
                Console.WriteLine(secret.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while accessing the Key Vault: {ex.Message}");
            }

            Console.ReadKey();
        }
    }
}
