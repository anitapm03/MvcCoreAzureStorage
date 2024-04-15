using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MvcCoreAzureStorage.Models;

namespace MvcCoreAzureStorage.Services
{
    public class ServiceStorageBlobs
    {
        private BlobServiceClient client;

        public ServiceStorageBlobs(BlobServiceClient client)
        {
            this.client = client;
        }

        //metodo para mostrar los containers
        public async Task<List<string>>
            GetContainersAsync()
        {
            List<string> containers = new List<string>();
            await foreach 
                (BlobContainerItem item in this.client.GetBlobContainersAsync())
            {
                containers.Add(item.Name);
            }
            return containers;
        }

        //metodo para crear un contenedor
        public async Task CreateContainer(string containerName)
        {
            await this.client.CreateBlobContainerAsync
                (containerName, PublicAccessType.Blob);
        }

        public async Task DeleteContainer(string containerName)
        {
            await this.client.DeleteBlobContainerAsync(containerName);
        }

        //metodo para mostrar los blobs de un container
        public async Task<List<BlobModel>>
            GetBlobsAsync(string containerName)
        {
            //recuperamos el cliente del container
            BlobContainerClient containerClient =
                this.client.GetBlobContainerClient(containerName);
            List<BlobModel> models = new List<BlobModel>();

            await foreach
                (BlobItem item in containerClient.GetBlobsAsync())
            {
                //necesitamos name, container name y url
                //debemos crear un blobclient 

                BlobClient blobClient = 
                    containerClient.GetBlobClient(item.Name);
                BlobModel blob = new BlobModel();
                blob.Nombre = item.Name;
                blob.Contenedor = containerName;
                blob.Url = blobClient.Uri.AbsoluteUri;

                models.Add(blob);
            }
            return models;
        }

        //metodo para eliminar un blob de un container
        public async Task DeleteBlob
            (string containerName, string blobName)
        {
            BlobContainerClient containerClient =
                this.client.GetBlobContainerClient(containerName);

            await containerClient.DeleteBlobAsync(blobName);
        }

        //metodo para subir un blob a un container
        public async Task UploadBlob
            (string containerName, string blobName, Stream stream)
        {
            BlobContainerClient containerClient =
                this.client.GetBlobContainerClient(containerName);

            await containerClient.UploadBlobAsync(blobName, stream);
        }
    }
}
