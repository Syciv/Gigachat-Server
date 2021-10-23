using System.IO;
using System.Text;
using System.Drawing;

namespace GigachatServer.Services
{
    public static class ImageConvertService
    {
        public static byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }
    }
}
