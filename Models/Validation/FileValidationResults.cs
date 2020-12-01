namespace ZennoLab.Models.Validation {

    internal class FileValidationResults : ValidationResults {
        public FileMetadataModel FileMetadata { get; private set; }

        public FileValidationResults(bool isValidFile, FileMetadataModel fileMetadata) : base(isValidFile)  {
            FileMetadata = fileMetadata;
        }

        protected FileValidationResults (bool isValidDataSet, string errorMessage) : base(isValidDataSet, errorMessage) {}
        
        public static new FileValidationResults GetInvalidResult(string errorMessage) {
            return new FileValidationResults(false, errorMessage);
        }
    }
}