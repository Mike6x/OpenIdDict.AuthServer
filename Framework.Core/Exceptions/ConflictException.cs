using System.Net;

namespace Framework.Core.Exceptions
{
    public class ConflictException : CustomException
{
    public ConflictException(string message)
        : base(message, null, HttpStatusCode.Conflict)
    {
    }
}
}
