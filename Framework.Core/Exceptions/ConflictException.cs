using System.Net;

namespace Framework.Core.Exceptions;

public class ConflictException(string message) : CustomException(message, HttpStatusCode.Conflict);