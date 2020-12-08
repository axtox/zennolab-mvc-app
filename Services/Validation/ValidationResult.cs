namespace ZennoLab.Services.Validation {
    public class ValidationResult {
        public bool IsValid {get; private set;}
        public string ErrorMessage {get; private set;}

        public ValidationResult(bool isValid, string errorMessage = "") {
            IsValid = isValid;
            ErrorMessage = errorMessage;
        }

        public static ValidationResult GetInvalidResult(string errorMessage) {
            return new ValidationResult(false, errorMessage);
        }
    }
}