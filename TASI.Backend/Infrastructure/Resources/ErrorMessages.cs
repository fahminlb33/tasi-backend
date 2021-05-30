using TASI.Backend.Domain;

namespace TASI.Backend.Infrastructure.Resources
{
    public enum ErrorCodes
    {
        Unauthorized,
        Forbidden,
        NotFound,
        ModelValidation,
        UnhandledException,
        DataDuplicated,
        InvalidSequentialProcess,
        NotEnoughStock
    }

    public static class ErrorMessages
    {
        public const string Unauthorized = "You are unauthorized to access this resource";
        public const string Forbidden = "You don't have sufficent permission to access this resource";
        public const string NotFound = "The specified resource cannot be found";
        public const string ModelValidation = "Request body/params/query contains invalid value";
        public const string UnhandledException = "An error has occurred when executing you request";

        public static readonly ErrorModel InternalExceptionModel = new(UnhandledException, ErrorCodes.UnhandledException);
    }
}
