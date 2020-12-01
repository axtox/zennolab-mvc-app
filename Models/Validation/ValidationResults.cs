namespace ZennoLab.Models.Validation {
    internal class ValidationResults {
        public bool IsValid {get; private set;}
        public string ErrorMessage {get; private set;}

        protected ValidationResults(bool isValid, string errorMessage = "") {
            IsValid = isValid;
            ErrorMessage = errorMessage;
        }

        public static ValidationResults GetInvalidResult(string errorMessage) {
            return new ValidationResults(false, errorMessage);
        }
    }
}