using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ZennoLab.Models {
    public interface IMetadataStorage {
        IEnumerable<FileMetadataModel> RetrieveDataSetMetadata();
        void AddFileMetadata(FileMetadataModel fileMetadata);
    }

    public class FileMetadataMock : IMetadataStorage {
        private ICollection<FileMetadataModel> FilesMetadata { get; set; }

        public FileMetadataMock() {
            FilesMetadata = new List<FileMetadataModel>();
        }

        public void AddFileMetadata(FileMetadataModel fileMetadata)
        {
            FilesMetadata.Add(fileMetadata);
        }

        public IEnumerable<FileMetadataModel> RetrieveDataSetMetadata()
        {
            return FilesMetadata;
        }
    }

    public class FileMetadataDbContext : DbContext, IMetadataStorage {
        public FileMetadataDbContext (DbContextOptions<FileMetadataDbContext> options)
                    : base(options)
        {
        }

        private DbSet<FileMetadataModel> FilesMetadata { get; set; }

        public void AddFileMetadata(FileMetadataModel fileMetadata)
        {
            FilesMetadata.Add(fileMetadata);

            this.SaveChanges();
        }

        public IEnumerable<FileMetadataModel> RetrieveDataSetMetadata()
        {
            return FilesMetadata.ToArray();
        }
    }
}