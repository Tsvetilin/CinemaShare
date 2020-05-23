using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class CloudinaryAPI : ICloudinaryAPI
    {
        private readonly string cloudName;
        private readonly string apiKey;
        private readonly string apiSecret;

        public CloudinaryAPI(string cloudName, string apiKey, string apiSecret)
        {
            this.cloudName = cloudName;
            this.apiKey = apiKey;
            this.apiSecret = apiSecret;
        }
        /// <summary>
        /// Uploads new image with selected parameters
        /// </summary>
        /// <returns>Image uri</returns>
        public async Task<string> UploadImage(MemoryStream imageMemoryStream, string fileName)
        {
            imageMemoryStream.Position = 0;
            var cloudinaryAccount = new Account(cloudName, apiKey, apiSecret);
            Cloudinary cloudinary = new Cloudinary(cloudinaryAccount);
            string publicId = Guid.NewGuid().ToString() + fileName;
            var file = new FileDescription(fileName, imageMemoryStream);
            var uploadParams = new ImageUploadParams
            {
                File = file,
                Format = "jpg",
                PublicId = publicId,
                UseFilename = true,
            };
            uploadParams.Check();
            var uploadResult = await cloudinary.UploadAsync(uploadParams);
            var uri = uploadResult.SecureUri.AbsoluteUri;
            return uri;
        }
    }
}
