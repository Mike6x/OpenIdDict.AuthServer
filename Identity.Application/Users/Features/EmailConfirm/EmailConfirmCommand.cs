using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FSH.Framework.Core.Identity.Users.Features.EmailConfirm
{
    public class EmailConfirmCommand
    {
        public string UserId { get; set; } = default!;

        public string Code { get; set; } = default!;

        public string Tenant { get; set; } = default!;
    }
}