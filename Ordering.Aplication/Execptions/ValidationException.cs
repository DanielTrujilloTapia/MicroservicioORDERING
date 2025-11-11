using FluentValidation.Results;

namespace Ordering.Application.Execptions
{
    public class ValidationException : ApplicationException
    {
        public ValidationException() : base("Uno o mas errores han ocurrido.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IEnumerable<ValidationFailure> failures) : this()
        {
            Errors = failures.GroupBy(e => e.PropertyName, e => e.ErrorMessage).ToDictionary(failtureGroup => failtureGroup.Key, failtureGroup => failtureGroup.ToArray());
        }

        public IDictionary<string, string[]> Errors { get; }
    }
}
