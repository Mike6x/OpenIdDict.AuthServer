using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Identity.Application.Claims;
using Identity.Application.Users.Dtos;

namespace Identity.Application.Users.Features.AddUserClaim
{
    [DataContract]
    public class AddUserClaimCommand
    {
        /// <summary>
        /// User or Role to which claim should be added
        /// </summary>
        [Required]
        [DataMember(IsRequired = true)]      
        public string Owner { get; set; } = string.Empty;

        /// <summary>
        /// Claim to add to the role
        /// </summary>
        [Required]
        [DataMember(IsRequired = true)]
        public ClaimViewModel ClaimToAdd { get; set; } = new ClaimViewModel();

        /// <summary>
        /// constructor
        /// </summary>
        public AddUserClaimCommand()
        {

        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="claimToAdd"></param>
        public AddUserClaimCommand(string owner, ClaimViewModel claimToAdd)
        {
            this.Owner = owner;
            this.ClaimToAdd = claimToAdd;
        }
    }
}
