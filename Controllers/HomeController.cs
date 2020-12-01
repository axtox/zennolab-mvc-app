using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZennoLab.Models;
using ZennoLab.Models.Validation;

namespace ZennoLab.Controllers
{
    public class HomeController : Controller
    {
        private readonly FileMetadataDbContext _context;

        public HomeController(FileMetadataDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["FilesAdded"] = _context.FilesMetadata.ToArray();
            return View();
        }

        private IActionResult RedirectToIndexWithError(string errorMessage) {
            ViewData["Message"] = errorMessage;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, [FromForm]UserDataSetViewModel userDataSet)
        {
            var validationResult = ValidateUserDataSet(userDataSet);
            if(!validationResult.IsValid)
                return RedirectToIndexWithError(validationResult.ErrorMessage);

            var fileValidationResult = await ValidateFile(file, validationResult);

            if(!fileValidationResult.IsValid)
                return RedirectToIndexWithError(validationResult.ErrorMessage);
            
            //save file 
            var path = SaveFile(file);
            
            //add metadata to SQL
            var metadata = fileValidationResult.FileMetadata;
            metadata.CreatedDate = userDataSet.Date;
            metadata.Name = userDataSet.Name;
            metadata.FilePath = await path;

            _context.FilesMetadata.Add(metadata);
            await _context.SaveChangesAsync();

            TempData["Message"] = "File successfully uploaded to File System.";
            return RedirectToAction("Index");
        }

        private DataSetValidationResults ValidateUserDataSet(UserDataSetViewModel userDataSet) {
            //validate dataset name
            var name = userDataSet.Name.ToLower();
            if(name.Contains("captcha") || Regex.IsMatch(name, @"\P{IsBasicLatin}"))
                return DataSetValidationResults.GetInvalidResult("Name contains invalid characters.");

            //check for invalid characters
            if(!userDataSet.HasCyrillicSymbols && !userDataSet.HasDigits & !userDataSet.HasLatinSymbols)
                return DataSetValidationResults.GetInvalidResult("Check if your dataset has Cyrillic Symbols, Latin Symbols or Digits.");

            //evaluate chosen options to calculate total number of files
            var options = new bool[] {userDataSet.HasLatinSymbols, userDataSet.HasSpecialSymbols, 
                                userDataSet.HasDigits, userDataSet.HasCyrillicSymbols,
                                userDataSet.IsCaseSensitive};
            var evaluatedOptions = options.Where(option => option).Count();
            var numberOfFiles = evaluatedOptions * 3000;

            var hasAnswersFile = userDataSet.AnswersSource == PictureAnswers.SeparateFile;

            if(userDataSet.AnswersSource == PictureAnswers.None)
                return DataSetValidationResults.GetInvalidResult("Can not produce picture processing without answers." +
                 "Please include file with answers to your zip file or include answers in picture filename.");

            return new DataSetValidationResults(numberOfFiles, hasAnswersFile, true);
        }

        private async Task<FileValidationResults> ValidateFile(IFormFile file, DataSetValidationResults validationResult)
        {
            if(file.ContentType != "application/zip")
                return FileValidationResults.GetInvalidResult("Only .zip files can be uploaded.");

            using(var archive = new ZipArchive(file.OpenReadStream(), ZipArchiveMode.Read)) {

                if(archive.Entries.Count < 2000 + validationResult.ExpectedNumberOfFiles || 
                archive.Entries.Count > 3000 + validationResult.ExpectedNumberOfFiles)
                    return FileValidationResults.GetInvalidResult($"Number of items in .zip archive is incorrect. Available range for your " +
                     $"dataset is from {2000 + validationResult.ExpectedNumberOfFiles} to {3000 + validationResult.ExpectedNumberOfFiles}");

                //potential Strategy Pattern - choose strategy based on HasAnswersFile flag
                if(validationResult.HasAnswersFile) {
                    var answersFile = archive.Entries.FirstOrDefault(zipFile => zipFile.Name.Equals("answers.txt"));

                    if(answersFile == null)
                        return FileValidationResults.GetInvalidResult("Can not locate answers.txt file in your zip. Add one or choose another answer source option.");

                    await CheckAnswersFile(answersFile.Open(), archive.Entries);
                } else {
                    //check for file names that will include answer and extention
                }
            }

            return new FileValidationResults(true, new FileMetadataModel
            {
                FileType = file.ContentType, 
                Extension = Path.GetExtension(file.FileName)
            });
        }

        private async Task<bool> CheckAnswersFile(Stream answersFile, ICollection<ZipArchiveEntry> allEntries) {
            var answersCounter = 0;
            using(var reader = new StreamReader(answersFile)) {
                var imageAnswer = await reader.ReadLineAsync();

                //next to check - file name in splitAnswer[0] to be real and compare it with allEntries
                //and answer in splitAnswer[1] to satisfy the options (has cyrillic, has latin and etc.)
                var splitAnswer = imageAnswer.Split(':', 2);

                answersCounter++;
            }

            if(answersCounter == allEntries.Count - 1)
                return true;

            return false;
        }

        private async Task<string> SaveFile(IFormFile file) {
            var path = Path.GetTempFileName();
            using (var copyFile = System.IO.File.Create(path)) {
                await file.CopyToAsync(copyFile);
            }
            return path;
        }
    }
}
