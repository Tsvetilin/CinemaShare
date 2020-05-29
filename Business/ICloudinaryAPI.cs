using System.IO;
using System.Threading.Tasks;

namespace Business
{
    public interface ICloudinaryAPI
    {
        public Task<string> UploadImageAsync(MemoryStream imageMemoryStream, string fileName);
    }
}
