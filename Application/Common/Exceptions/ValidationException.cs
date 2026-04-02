using FluentValidation.Results;

namespace Application.Common.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(IList<ValidationFailure> failures)
        : base(BuildMessage(failures))
    {
    }

    private static string BuildMessage(IList<ValidationFailure> failures) =>
        $"Validation failed: {Environment.NewLine} {string.Join(Environment.NewLine, failures.Select(f => $"- {f.PropertyName}: {f.ErrorMessage}"))}";
}
