namespace Identity.Domain.Enums;

public enum ResponseStatus
{
    Success = 1,
    Error = 2,
    ValidationError = 3,
    NotFound = 4,
    Unauthorized = 5,
    BadRequest = 6
}