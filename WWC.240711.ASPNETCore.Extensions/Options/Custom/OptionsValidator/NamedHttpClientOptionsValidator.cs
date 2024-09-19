using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions
{
    public class NamedHttpClientOptionsValidator : IValidateOptions<List<NamedHttpClientOptions>>
    {
        public ValidateOptionsResult Validate(string name, List<NamedHttpClientOptions> options)
        {
            foreach (var clientOption in options)
            {
                var validationResults = new List<ValidationResult>();
                var context = new ValidationContext(clientOption, serviceProvider: null, items: null);

                // 验证每个 NamedHttpClientOptions 实例
                if (!Validator.TryValidateObject(clientOption, context, validationResults, true))
                {
                    var errors = string.Join(", ", validationResults.Select(x => x.ErrorMessage));
                    return ValidateOptionsResult.Fail($"Validation failed for client '{clientOption.Name}': {errors}");
                }
            }

            return ValidateOptionsResult.Success;
        }
    }
}
