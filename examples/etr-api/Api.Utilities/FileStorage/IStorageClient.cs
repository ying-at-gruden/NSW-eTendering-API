using System.IO;

namespace Api.Utilities.FileStorage
{
    public interface IStorageClient
    {
        bool Exists(string file_path);

        string[] List(string filter);

        Stream ReadFile(string file_path);

        void DeleteFile(string file_path);
    }
}