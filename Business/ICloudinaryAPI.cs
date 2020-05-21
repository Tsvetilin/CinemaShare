using System.IO;
using System.Threading.Tasks;

namespace Business
{
    public interface ICloudinaryAPI
    {
        public Task<string> UploadImage(MemoryStream imageMemoryStream, string fileName);
    }
}
