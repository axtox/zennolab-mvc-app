using System.Threading.Tasks;
using ZennoLab.Models;

namespace ZennoLab.Services.Validation
{
    public interface IValidationService {
        Task<ValidationResult> ValidateAsync(UserDataSetViewModel userDataSet);
    }
}