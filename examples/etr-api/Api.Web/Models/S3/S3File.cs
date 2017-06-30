using System.Collections.Generic;

namespace Api.Web.Models.S3
{
    public class S3File
    {
        public string Filename { get; set; }
        public string ContentType { get; set; }
        public Dictionary<string,string> MetaData { get; set; }
        public string FileContent { get; set; }
    }
}