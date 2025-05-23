﻿using System.Net;

namespace Framework.Core.Exceptions;
public class GeneralException : Exception
{
    public IEnumerable<string> ErrorMessages { get; }

    public HttpStatusCode StatusCode { get; }

    public GeneralException(string message, IEnumerable<string> errors, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        : base(message)
    {
        ErrorMessages = errors;
        StatusCode = statusCode;
    }

    public GeneralException(string message) : base(message)
    {
        ErrorMessages = new List<string>();
    }
}
