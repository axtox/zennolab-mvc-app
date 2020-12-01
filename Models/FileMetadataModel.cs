using System;

namespace ZennoLab.Models {
    public class FileMetadataModel
    {
        public int Id { get; set;}
        public string Name { get; set; }
        public string FileType { get; set; }
        public string Extension { get; set; }
        public string FilePath { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}