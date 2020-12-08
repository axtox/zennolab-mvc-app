using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZennoLab.Models;
using ZennoLab.Services.Validation;
using ZennoLab.Services;

namespace ZennoLab.Controllers
{
    public class FileUploadController : Controller
    {
        private readonly IDatasetStorageService _storage;
        private readonly IValidationService _validation;

        public FileUploadController(IDatasetStorageService storage, IValidationService validation)
        {
            _storage = storage;
            _validation = validation;
        }

        private IActionResult RedirectToIndexWithMessage(string errorMessage) {
            TempData["Message"] = errorMessage;
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Upload([FromForm]UserDataSetViewModel userDataSet)
        {
            var validationResult = await _validation.ValidateAsync(userDataSet);

            if(!validationResult.IsValid)
                return RedirectToIndexWithMessage(validationResult.ErrorMessage);

            _storage.Save(userDataSet);
            
            return RedirectToIndexWithMessage("File successfully uploaded to File System.");
        }
    }
}