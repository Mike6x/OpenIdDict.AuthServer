using System.ComponentModel.DataAnnotations;

namespace Framework.Infrastructure.Options
{
    public class AppOptions : IOptionsRoot
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; } = "OpenIdDict.API";
    }
}
// Add from fsh