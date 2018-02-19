using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Net;
using AzureToolkit.Models;
using System.Collections.Generic;
using System.Linq;

namespace WebApplicationBasic.Controllers
{
    [Route("api/[controller]")]
    public class ImagesController : Controller
    {
        private CloudBlobContainer _container;
        private AzureToolkitContext _context;

        public ImagesController(AzureToolkitContext context)
        {
            CloudStorageAccount storageAccount = new CloudStorageAccount(
                new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
                        "azrtkthectagon",
                        "/kZ740n7/npVoCm35EzlCt1djhzh4G/ziPs9+9WIeAtGdsvKkQanDBkWx5llQumuGyGf5v7di5AuITeupWPTZw=="), true);
            // Create a blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            _container = blobClient.GetContainerReference("savedimages");
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostImage([FromBody]ImagePostRequest request)
        {
            var uploadedImageBlob = await UploadImage(request);
            SaveImageMetadata(request, uploadedImageBlob);
            return Ok();
        }

        private async Task<CloudBlockBlob> UploadImage(ImagePostRequest request)
        {
            CloudBlockBlob blockBlob = _container.GetBlockBlobReference($"{request.Id}.{request.EncodingFormat}");
            HttpWebRequest aRequest = (HttpWebRequest)WebRequest.Create(request.URL);
            HttpWebResponse aResponse = (await aRequest.GetResponseAsync()) as HttpWebResponse;
            var stream = aResponse.GetResponseStream();
            await blockBlob.UploadFromStreamAsync(stream);
            stream.Dispose();
            return blockBlob;
        }

        private void SaveImageMetadata(ImagePostRequest request, CloudBlockBlob blockBlob)
        {
            var savedImage = new SavedImage();
            savedImage.UserId = request.UserId;
            savedImage.Description = request.Description;
            savedImage.StorageUrl = blockBlob.Uri.ToString();
            savedImage.Tags = new List<SavedImageTag>();

            foreach (var tag in request.Tags)
            {
                savedImage.Tags.Add(new SavedImageTag() { Tag = tag });
            }

            _context.Add(savedImage);
            _context.SaveChanges();
        }
    }

    public class ImagePostRequest
    {
        public string UserId { get; set; }
        public string Description { get; set; }
        public string[] Tags { get; set; }
        public string URL { get; set; }
        public string Id { get; set; }
        public string EncodingFormat { get; set; }


    }
}