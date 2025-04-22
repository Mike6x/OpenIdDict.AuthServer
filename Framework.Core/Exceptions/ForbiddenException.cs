using System.Net;

namespace Framework.Core.Exceptions;

public class ForbiddenException : CustomException
{
    public ForbiddenException() : base("You do not have permissions to access this resource.", HttpStatusCode.Forbidden)
    {
    }
}

// Add from fsh