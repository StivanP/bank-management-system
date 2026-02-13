using Common;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.Services
{
    public static class ServiceResultExtentions<T>
            where T : class, new()
    {
        public static ServiceResult<T> Failure(T? data, ModelStateDictionary errors)
        {
            var errorsList = new List<Error>();

            foreach (var kvp in errors)
            {
                if (kvp.Value.Errors.Count > 0)
                {
                    errorsList.Add(new Error
                    {
                        Key = kvp.Key,
                        Messages = kvp.Value.Errors
                            .Select(e => e.ErrorMessage)
                            .ToList()
                    });
                }
            }

            return new ServiceResult<T>
            {
                IsSuccess = false,
                Data = data ?? new T(),
                Errors = errorsList
            };
        }
    }

}