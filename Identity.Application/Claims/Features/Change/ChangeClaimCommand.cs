using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Identity.Application.Claims.Features.Change
{
    public class ChangeClaimCommand
    {
        public string Owner { get; set; } = string.Empty;

        public ClaimViewModel Original { get; set; } = new ClaimViewModel();

        public ClaimViewModel Modified { get; set; } = new ClaimViewModel();

    }
}
