using TASI.Backend.Domain;

namespace TASI.Backend.Infrastructure.Resources
{
    public enum ErrorCodes
    {
        Unauthorized,
        Forbidden,
        ModelValidation,
        UnhandledException,
        NameIdentifierIsEmpty
    }

    public static class ErrorMessages
    {
        public const string Unauthorized = "You are unauthorized to access this resource";
        public const string Forbidden = "You don't have sufficent permission to access this resource";
        public const string ModelValidation = "Request body/params/query contains invalid value";
        public const string UnhandledException = "An error has occurred when executing you request";
        public const string NameIdentifierNull = "Unable to get name identifier claim from Authorization header";

        public static ErrorModel InternalExceptionModel = new(UnhandledException, ErrorCodes.ModelValidation);
    }
}
