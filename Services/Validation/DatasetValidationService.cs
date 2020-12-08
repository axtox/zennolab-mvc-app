using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZennoLab.Models;

namespace ZennoLab.Services.Validation {
    public class DatasetValidationService : IValidationService {
        public virtual async Task<ValidationResult> ValidateAsync(UserDataSetViewModel userDataSet) {

            if(!CheckDatasetName(userDataSet.Name))
                return ValidationResult.GetInvalidResult("Name contains invalid characters.");

            if(!CheckDatasetOptions(userDataSet))
                return ValidationResult.GetInvalidResult("Check if your dataset has Cyrillic Symbols, Latin Symbols or Digits.");

            if(!CheckDatasetForAnswers(userDataSet))
                return ValidationResult.GetInvalidResult("Can not produce picture processing without answers." +
                "Please include file with answers to your zip file or include answers in picture filename.");

            return new ValidationResult(true);
        }

        private bool CheckDatasetName(string datasetName) {
            return !datasetName.Contains("captcha") || !Regex.IsMatch(datasetName, @"\P{IsBasicLatin}");
        }

        private bool CheckDatasetOptions(UserDataSetViewModel userDataSet) {
            return userDataSet.HasCyrillicSymbols || userDataSet.HasDigits || userDataSet.HasLatinSymbols;
        }

        private bool CheckDatasetForAnswers(UserDataSetViewModel userDataSet) {
            return userDataSet.AnswersSource != PictureAnswers.None;
        }
     }
}