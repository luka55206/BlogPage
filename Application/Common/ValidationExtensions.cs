using FluentValidation;
namespace BlogPage.Application.Common;

public static class ValidationExtensions
{
    public static async Task<(bool IsValid, IResult? ErrorResult)> ValidateAsync<T>(
        this T request,
        IServiceProvider services) where T : class
    {
     var validator = services.GetService<IValidator<T>>();

     if (validator == null)
     {
         return (true, null);
         
     }

     var validationResult = await validator.ValidateAsync(request);

     if (!validationResult.IsValid)
     {
         var errors = validationResult.Errors
             .GroupBy(e => e.PropertyName)
             .ToDictionary(
                 g => g.Key,
                 g => g.Select(e => e.ErrorMessage).ToArray()
             );
         return (false, Results.ValidationProblem(errors));
     }
     return (true, null);
    } 
    
}