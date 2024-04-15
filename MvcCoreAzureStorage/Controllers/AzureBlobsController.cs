using Microsoft.AspNetCore.Mvc;
using MvcCoreAzureStorage.Models;
using MvcCoreAzureStorage.Services;
using System.ComponentModel;
using System.Reflection.Metadata;

namespace MvcCoreAzureStorage.Controllers
{
    public class AzureBlobsController : Controller
    {

        private ServiceStorageBlobs service;

        public AzureBlobsController(ServiceStorageBlobs service)
        {
            this.service = service;
        }

        public async Task<IActionResult> Index()
        {
            List<string> containers =
                await this.service.GetContainersAsync();

            return View(containers);
        }

        public IActionResult CreateContainer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> 
            CreateContainer(string containername)
        {
            await this.service.CreateContainer(containername);

            ViewData["MSG"] = "Container creado correctamente";
            return View();
        }

        public async Task<IActionResult> 
            DeleteContainer(string containername)
        {
            await this.service.DeleteContainer(containername);

            return RedirectToAction("Index");
        }

        //ver los blobs de un container
        public async Task<IActionResult> 
            VerBlobsContainer(string containername)
        {
            List<BlobModel> models =
                 await this.service.GetBlobsAsync(containername);
            return View(models);
        }

        public async Task<IActionResult> DeleteBlob
            (string containername, string blobname)
        {
            await this.service.DeleteBlob(containername, blobname);
            return RedirectToAction("VerBlobsContainer",
                new { containername = containername });
        }

        public IActionResult UploadBlob(string containername)
        {
            ViewData["CONTAINER"] = containername;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult>
            UploadBlob(string containername, IFormFile file)
        {
            string blobName = file.FileName;
            using (Stream stream = file.OpenReadStream())
            {
                await this.service.UploadBlob
                    (containername, blobName, stream);
            }
            return RedirectToAction("VerBlobsContainer"
                , new { containername = containername });
        }

    }
}
