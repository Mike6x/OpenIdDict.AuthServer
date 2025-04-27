using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Identity.Application.Claims.Features.Add
{
    /// <summary>
    /// Add a Claim to User or Role 
    /// </summary>
    [DataContract]
    public class AddClaimCommand
    {
        [Required]
        [DataMember(IsRequired = true)]      
        public string Owner { get; set; } = string.Empty;

        [Required]
        [DataMember(IsRequired = true)]
        public ClaimViewModel ClaimToAdd { get; set; } = new ClaimViewModel();

        public AddClaimCommand()
        {

        }

        public AddClaimCommand(string owner, ClaimViewModel claimToAdd)
        {
            this.Owner = owner;
            this.ClaimToAdd = claimToAdd;
        }
    }
}
