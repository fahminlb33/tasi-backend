using System.ComponentModel.DataAnnotations;
using FluentValidation;
using TASI.Backend.Domain.Users.Entities;

namespace TASI.Backend.Domain.Users.Dto
{
    public class EditUserLoginDto
    {
        public int UserId { get; set; }
        public UserRole Role { get; set; }
        public string Username { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }
    }

    public class EditUserLoginDtoValidator : AbstractValidator<EditUserLoginDto>
    {
        public EditUserLoginDtoValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().Length(5, 30);
            RuleFor(x => x.Username).NotEmpty().Length(5, 30);
            RuleFor(x => x.Role).NotEqual(UserRole.SuperAdmin);
        }
    }
}
