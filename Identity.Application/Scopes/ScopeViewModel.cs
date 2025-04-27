using System.ComponentModel.DataAnnotations;

namespace Identity.Application.Scopes
{
    public class ScopeViewModel
    {
        [Required(AllowEmptyStrings = true)]
        public string Id { get; set; } = string.Empty;
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string DisplayName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Resources associated with the scope.
        /// Resources are added on claims prinicipal
        /// </summary>
        public List<string> Resources { get; set; } = [];

    }
}
