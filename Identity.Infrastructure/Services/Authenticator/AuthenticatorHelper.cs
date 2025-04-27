using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;

namespace Identity.Infrastructure.Services.Authenticator;

public static class AuthenticatorHelper
{
    private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
    
    private  static UrlEncoder urlEncoder;
    
    public static string FormatKey(string unformattedKey)
    {
        var result = new StringBuilder();
        int currentPosition = 0;
        while (currentPosition + 4 < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
            currentPosition += 4;
        }
        if (currentPosition < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition));
        }

        return result.ToString().ToLowerInvariant();
    }

    public static string GenerateQrCodeUri(string email, string unformattedKey)
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            AuthenticatorUriFormat,
            urlEncoder.Encode("OpenIdDict-Identity"), // "Pixel-Identity""
            urlEncoder.Encode(email),
            unformattedKey);
    }
}