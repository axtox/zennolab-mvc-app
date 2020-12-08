using System.Collections.Generic;
using ZennoLab.Models;

namespace ZennoLab.Services
{
    public interface IDatasetStorageService
    {
        void Save(UserDataSetViewModel dataset);
        IEnumerable<FileMetadataModel> Load();
    }
}