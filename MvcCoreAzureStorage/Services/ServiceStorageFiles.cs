using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Microsoft.Extensions.Azure;

namespace MvcCoreAzureStorage.Services
{
    public class ServiceStorageFiles
    {
        //todo servicio de storage siempre usa un client
        //para acceder a sus recursos, dicho client
        //necesita unas keys
        private ShareDirectoryClient root;

        //debemos recibir las keys desde appsettings
        public ServiceStorageFiles(IConfiguration configuration) 
        {
            string keys = configuration.GetValue<string>
                ("AzureKeys:StorageAccount");
            //cada cliente de storage accede a un share 
            //mediante las claves 
            ShareClient client = new ShareClient(keys, "ejemplofiles");

            this.root = client.GetRootDirectoryClient();
        }

        //metodo para recuperar los ficheros de la raíz del shared
        public async Task<List<string>> GetFilesAsync()
        {
            List<string> files = new List<string>();

            await foreach
                (ShareFileItem item in this.root.GetFilesAndDirectoriesAsync())
            {
                files.Add(item.Name);
            }

            this.root.GetFilesAndDirectories();
            return files;
        }

        public async Task<string> ReadFileAsync(string fileName)
        {
            //necesitamos un client del recurso que queremos recuperar
            ShareFileClient file =
                this.root.GetFileClient(fileName);
            ShareFileDownloadInfo data =
                await file.DownloadAsync();

            Stream stream = data.Content;
            string contenido = "";

            using (StreamReader reader = new StreamReader(stream))
            {
                contenido = await reader.ReadToEndAsync();
                return contenido;
            }
        }

        public async Task UploadFileAsync
            (string fileName, Stream stream)
        {
            ShareFileClient file =
                this.root.GetFileClient(fileName);
            await file.CreateAsync(stream.Length);
            await file.UploadAsync(stream);
        }

        public async Task DeleteFileAsync(string fileName)
        {
            ShareFileClient file =
                this.root.GetFileClient(fileName);
            await file.DeleteAsync();
        }
    }
}
