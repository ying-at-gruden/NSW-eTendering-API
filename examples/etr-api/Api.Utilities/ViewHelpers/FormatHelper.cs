using System.IO;

namespace Api.Utilities.ViewHelpers
{
    public class FormatHelper
    {
        public byte[] convertStreamToByteArray(Stream inputStream)
        {
            byte[] result;
            using (var streamReader = new MemoryStream())
            {
                inputStream.CopyTo(streamReader);
                result = streamReader.ToArray();
            }

            return result;
        }
    }
}
