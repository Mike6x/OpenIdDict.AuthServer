using Microsoft.AspNetCore.Http;

namespace Identity.Application.OpenIddict;

public interface IOpenIdDictAuthorizationService
{
    Task<IResult> AuthorizeAsync();
    Task<IResult> AcceptAsync();
    IResult Deny();
    Task<IResult> VerifyAsync();
    Task<IResult> VerifyAcceptAsync();
    IResult VerifyDeny();
    Task<IResult> ExchangeAsync();

    Task<IResult> EndSessionAsync();

}