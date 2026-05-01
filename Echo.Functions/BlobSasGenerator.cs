using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs.Models;
namespace Echo.Functions;

public class BlobSasGenerator
{
    [Function("GenerateUploadUrl")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "generate-sas")] HttpRequest req)
    {
        string fileName = req.Query["fileName"];
        if(string.IsNullOrEmpty(fileName)) return new BadRequestObjectResult("fileName is required.");
        string uniqueFileName = $"{Guid.NewGuid()}_{fileName}";

        string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        string containerName = "chat-attachments";

        var blobServiceClient = new BlobServiceClient(connectionString);

        var serviceProperties = await blobServiceClient.GetPropertiesAsync();
        serviceProperties.Value.Cors.Clear();
        serviceProperties.Value.Cors.Add(new BlobCorsRule
        {
            AllowedOrigins = "*", 
            AllowedMethods = "GET,PUT,OPTIONS,POST,HEAD",
            AllowedHeaders = "*",
            ExposedHeaders = "*",
            MaxAgeInSeconds = 86400
        });
        await blobServiceClient.SetPropertiesAsync(serviceProperties.Value);


        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        await containerClient.CreateIfNotExistsAsync();

        var blobClient = containerClient.GetBlobClient(uniqueFileName);
        
        var sasBuilder = new Azure.Storage.Sas.BlobSasBuilder
        {
            BlobContainerName = containerName,
            BlobName = uniqueFileName,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(5)
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Write);
        var sasUri = blobClient.GenerateSasUri(sasBuilder);
        return new OkObjectResult(new
        {
            UploadUrl = sasUri.ToString(),
            PublicUrl = blobClient.Uri.ToString()
        });
    }
    
}
