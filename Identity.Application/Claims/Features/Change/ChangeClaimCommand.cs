using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Identity.Application.Claims.Features.Change
{
    // <summary>
    /// Change a claim to new on User or Role. 
    /// </summary>
    
    [DataContract]
    public class UpdateClaimCommand
    {

        [Required]
        [DataMember(IsRequired = true)]
        public string Owner { get; set; } = string.Empty;

        [Required]
        [DataMember(IsRequired = true)]
        public ClaimViewModel Original { get; set; } = null;


        [Required]
        [DataMember(IsRequired = true)]
        public ClaimViewModel Modified { get; set; } = null;


        public UpdateClaimCommand()
        {

        }


        public UpdateClaimCommand(string owner, ClaimViewModel original, ClaimViewModel modified)
        {
            this.Owner = owner;
            this.Original = original;
            this.Modified = modified;
        }
    }
}
