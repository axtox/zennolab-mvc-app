namespace ZennoLab.Models.Validation { 
    internal class DataSetValidationResults : ValidationResults {
        public int ExpectedNumberOfFiles {get; private set;}
        public bool HasAnswersFile {get; private set;}

        public DataSetValidationResults (int expectedNumberOfFiles, bool answersFileIncluded, bool isValidDataSet) : base(isValidDataSet) {
            ExpectedNumberOfFiles = expectedNumberOfFiles;
            HasAnswersFile = answersFileIncluded;
        }

        protected DataSetValidationResults (bool isValidDataSet, string errorMessage) : base(isValidDataSet, errorMessage) {}

        public static new DataSetValidationResults GetInvalidResult(string errorMessage) {
            return new DataSetValidationResults(false, errorMessage);
        }
    }
}