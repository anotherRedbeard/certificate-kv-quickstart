# certificate-kv-quickstart
Sample application(s) that will create Azure Key Value certificates. Originally pulled from [this repo](https://github.com/MicrosoftDocs/azure-docs/blob/main/articles/key-vault/certificates/quick-create-net.md)

## Object Model

The Azure Key Vault certificate client library for .NET allows you to manage certificates. The [Code examples](https://github.com/MicrosoftDocs/azure-docs/blob/main/articles/key-vault/certificates/quick-create-net.md#code-examples) section shows how to create a client, set a certificate, retrieve a certificate, and delete a certificate.

### Authenticate and create a client

Application requests to most Azure services must be authorized. Using the [DefaultAzureCredential](/dotnet/azure/sdk/authentication#defaultazurecredential) class provided by the [Azure Identity client library](/dotnet/api/overview/azure/identity-readme) is the recommended approach for implementing passwordless connections to Azure services in your code. `DefaultAzureCredential` supports multiple authentication methods and determines which method should be used at runtime. This approach enables your app to use different authentication methods in different environments (local vs. production) without implementing environment-specific code. 

In this quickstart, `DefaultAzureCredential` authenticates to key vault using the credentials of the local development user logged into the Azure CLI. When the application is deployed to Azure, the same `DefaultAzureCredential` code can automatically discover and use a managed identity that is assigned to an App Service, Virtual Machine, or other services. For more information, see [Managed Identity Overview](/azure/active-directory/managed-identities-azure-resources/overview).

In this example, the name of your key vault is expanded to the key vault URI, in the format `https://<your-key-vault-name>.vault.azure.net`. For more information about authenticating to key vault, see [Developer's Guide](/azure/key-vault/general/developers-guide#authenticate-to-key-vault-in-code).

```csharp
string keyVaultName = Environment.GetEnvironmentVariable("KEY_VAULT_NAME");
var kvUri = "https://" + keyVaultName + ".vault.azure.net";

var client = new CertificateClient(new Uri(kvUri), new DefaultAzureCredential());
```

### Running Locally

There are a couple of pre-requisites to running this code.

* An Azure subscription - [create a free one here](https://azure.microsoft.com/free/dotnet)
* [.NET 6 SDK or later](https://dotnet.microsoft.com/download)
* [Azure CLI](/cli/azure/install-azure-cli)
* A Key Vault - you can create one using [Azure portal](../general/quick-create-portal.md), [Azure CLI](../general/quick-create-cli.md), or [Azure PowerShell](../general/quick-create-powershell.md).

To run this code locally all you need to do is the following:

* Clone this repo
* You will need to be logged into your azure subscription via the Azure CLI using an account that has full access to Certificates in the Key Vault you created in your subscription.  Here is the command you can use to login locally:

    ```azurecli-interactive
    az login
    ```
    
    Here is a command you can use to ensure you have the proper permissions.

    ```azurecli
    az keyvault set-policy --name <your-key-vault-name> --upn user@domain.com --certificate-permissions delete get list create purge
    ```
* Run the `dotnet build` command
* Execute the code by running `dotnet run <command> <certificate-name>`
