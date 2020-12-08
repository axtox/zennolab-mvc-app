using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using ZennoLab.Models;

namespace ZennoLab.Services
{
    public class DatasetStorageService : IDatasetStorageService
    {
        private IMetadataStorage _metadataStorage;

        public DatasetStorageService(IMetadataStorage metadataStorage) 
        {
            _metadataStorage = metadataStorage;
        }

        public IEnumerable<FileMetadataModel> Load()
        {
            return _metadataStorage.RetrieveDataSetMetadata();
        }

        public void Save(UserDataSetViewModel dataset)
        {
            var path = SaveFile(dataset.File);

            var metadata = new FileMetadataModel {
                CreatedDate = dataset.Date,
                Name = dataset.Name,
                FilePath = path,
                FileType = dataset.File.ContentType, 
                Extension = Path.GetExtension(dataset.File.FileName)
            };

            _metadataStorage.AddFileMetadata(metadata);
        }

        private string SaveFile(IFormFile file) {
            var path = Path.GetTempFileName();
            using (var copyFile = System.IO.File.Create(path)) {
                file.CopyTo(copyFile);
            }
            return path;
        }
    }
}