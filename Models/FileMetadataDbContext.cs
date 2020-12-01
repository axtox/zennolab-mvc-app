using Microsoft.EntityFrameworkCore;

namespace ZennoLab.Models {
    public class FileMetadataDbContext : DbContext {
        public FileMetadataDbContext (DbContextOptions<FileMetadataDbContext> options)
                    : base(options)
        {
        }

        public DbSet<FileMetadataModel> FilesMetadata { get; set; }
    }
}