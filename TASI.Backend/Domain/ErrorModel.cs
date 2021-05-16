using TASI.Backend.Infrastructure.Resources;

namespace TASI.Backend.Domain
{
    public record ErrorModel (string Message, ErrorCodes AppCode, object Data = null);
}
