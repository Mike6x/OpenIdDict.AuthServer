using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Identity.Application.Claims.DeleteClaim
{
    /// <summary>
    /// Remove a Claim form User or Role 
    /// </summary>
    public class DeleteClaimCommand
    {

        [Required]
        [DataMember(IsRequired = true)]
        public string Owner { get; set; } = string.Empty;
        
        [Required]
        [DataMember(IsRequired = true)]
        public ClaimViewModel ClaimToRemove { get; set; } = default!;


        public DeleteClaimCommand()
        {

        }

        public DeleteClaimCommand(string owner, ClaimViewModel claimToRemove)
        {
            this.Owner = owner;
            this.ClaimToRemove = claimToRemove;
        }
    }
}
