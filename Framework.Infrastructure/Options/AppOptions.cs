using System.ComponentModel.DataAnnotations;

namespace Framework.Core.Options
{
    public class AppOptions : IOptionsRoot
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; } = "FSH.WebAPI";
    }
}
// Add from fsh