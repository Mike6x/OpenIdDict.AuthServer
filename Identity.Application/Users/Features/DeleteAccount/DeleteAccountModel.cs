using System.ComponentModel.DataAnnotations;

namespace Identity.Application.Users.Features.DeleteAccount
{
    public  class DeleteAccountModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
