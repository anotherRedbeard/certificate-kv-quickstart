using System;
using Azure.Identity;
using Azure.Security.KeyVault.Certificates;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Please specify a command (create, retrieve, or delete) and a certificate name.");
            return;
        }

        var command = args[0];
        var certificateName = args[1];
        var keyVaultName = Environment.GetEnvironmentVariable("KEY_VAULT_NAME") ?? string.Empty;
        if (string.IsNullOrWhiteSpace(keyVaultName))
        {
            throw new Exception("Missing environment variable KEY_VAULT_NAME");
        }
        var kvUri = "https://" + keyVaultName + ".vault.azure.net";
        var client = new CertificateClient(new Uri(kvUri), new DefaultAzureCredential());

        switch (command)
        {
            case "create":
                await CreateCertificateAsync(client, keyVaultName, certificateName);
                break;
            case "retrieve":
                await RetrieveCertificateAsync(client, keyVaultName, certificateName);
                break;
            case "createAndRetrieve":
                await CreateCertificateAsync(client, keyVaultName, certificateName);
                await RetrieveCertificateAsync(client, keyVaultName, certificateName);
                break;
            case "delete":
                await DeleteCertificateAsync(client, keyVaultName, certificateName);
                break;
            case "deleteAndPurge":
                await DeleteCertificateAsync(client, keyVaultName, certificateName);
                await PurgeCertificateAsync(client, keyVaultName, certificateName);
                break;
            default:
                Console.WriteLine($"Unknown command: {command}");
                break;
        }
    }

    static async Task CreateCertificateAsync(CertificateClient client, string keyVaultName, string certificateName = "defaultCertificate")
    {
        Console.Write($"Creating a certificate in {keyVaultName} called '{certificateName}' ... ");
        var operation = await client.StartCreateCertificateAsync(certificateName, CertificatePolicy.Default);
        var certificate = await operation.WaitForCompletionAsync();
        Console.WriteLine("Done.");
    }

    static async Task RetrieveCertificateAsync(CertificateClient client, string keyVaultName, string certificateName = "defaultCertificate")
    {
        Console.WriteLine($"Retrieving your certificate from {keyVaultName}.");
        var myCertByName = await client.GetCertificateAsync(certificateName);
        Console.WriteLine($"Your certificate version is '{myCertByName.Value.Properties.Version}'.");
    }

    static async Task DeleteCertificateAsync(CertificateClient client, string keyVaultName, string certificateName = "defaultCertificate")
    {
        Console.Write($"Deleting your certificate from {keyVaultName}.");
        var delOperation = await client.StartDeleteCertificateAsync(certificateName);

        // You only need to wait for completion if you want to purge or recover the certificate.
        await delOperation.WaitForCompletionAsync();
        var certForDeletion = delOperation.Value;
        Console.WriteLine($"Done, certificated named {certForDeletion.Name} is deleted.");
    }

    static async Task PurgeCertificateAsync(CertificateClient client, string keyVaultName, string certificateName = "defaultCertificate")
    {
        Console.Write($"Purging your certificate from {keyVaultName}.");
        await client.PurgeDeletedCertificateAsync(certificateName);
        Console.WriteLine($"Done, your certificate is purged.");
    }
}