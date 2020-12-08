using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using ZennoLab.Models;

namespace ZennoLab.Services.Validation {
    public class DatasetFileValidationService : DatasetValidationService {
        public override async Task<ValidationResult> ValidateAsync(UserDataSetViewModel userDataSet)
        {
            var datasetValidationResult = await base.ValidateAsync(userDataSet);
            if(!datasetValidationResult.IsValid)
                return datasetValidationResult;
            
            if(!CheckFileExtention(userDataSet.File.ContentType))
                return ValidationResult.GetInvalidResult("Only .zip files can be uploaded.");

            return await ValidateDatasetFileContents(userDataSet);
        }

        private bool CheckFileExtention(string filemime) {
            return filemime == "application/zip";
        }

        private async Task<ValidationResult> ValidateDatasetFileContents(UserDataSetViewModel userDataSet)
        {
            using(var file = userDataSet.File.OpenReadStream()) {
                using(var archive = new ZipArchive(file, ZipArchiveMode.Read)) {

                    var rangeOfFilesQuantity = CalculateExpectedRangeForFilesQuantity(userDataSet);
                    if(archive.Entries.Count < rangeOfFilesQuantity.Start.Value || archive.Entries.Count > rangeOfFilesQuantity.End.Value)
                        return ValidationResult.GetInvalidResult($"Number of items in .zip archive is incorrect. Available range for your " +
                                                $"dataset is from {rangeOfFilesQuantity.Start.Value} to {rangeOfFilesQuantity.End.Value}");
                    
                    if(DatasetHasAnswersFile(userDataSet))
                        return await ValidatePicturesUsingAnswersFile(archive.Entries);
                    else
                        return await ValidatePictures();
                }
            }
        }

        private Range CalculateExpectedRangeForFilesQuantity(UserDataSetViewModel userDataSet)
        {            
            var options = new bool[] {userDataSet.HasLatinSymbols, userDataSet.HasSpecialSymbols, 
                                userDataSet.HasDigits, userDataSet.HasCyrillicSymbols,
                                userDataSet.IsCaseSensitive};
            var evaluatedOptions = options.Where(option => option).Count();
            var expectedNumberOfFiles =  evaluatedOptions * 3000;

            return (2000 + expectedNumberOfFiles)..(3000 + expectedNumberOfFiles);
        }

        private bool DatasetHasAnswersFile(UserDataSetViewModel userDataSet) {
            return userDataSet.AnswersSource == PictureAnswers.SeparateFile;
        }

        //TODO: Check for answer names against DataSet options!
        //this method always return true
        private async Task<ValidationResult> ValidatePicturesUsingAnswersFile(ICollection<ZipArchiveEntry> entries)
        {
            var answersEntry = entries.FirstOrDefault(zipFile => zipFile.Name.Equals("answers.txt"));

            if(answersEntry == null)
                return ValidationResult.GetInvalidResult("Can not locate answers.txt file in your zip. Add one or choose another answer source option.");

            using (var answersFile = answersEntry.Open()) {
                var answersCounter = 0;
                using(var reader = new StreamReader(answersFile)) {
                    string imageAnswer;
                    
                    while((imageAnswer = await reader.ReadLineAsync()) != null) {

                        //next to check - file name in splitAnswer[0] to be real and compare it with allEntries
                        //and answer in splitAnswer[1] to satisfy the options (has cyrillic, has latin and etc.)
                        var splitAnswer = imageAnswer.Split(':', 2);

                        answersCounter++;

                    }
                }
            }

            return new ValidationResult(true);
        }

        //TODO: check for file names that will include answer and extention
        // answer must satisfy DataSet options!
        private async Task<ValidationResult> ValidatePictures()
        {
            return new ValidationResult(true);
        }
    }
    
}