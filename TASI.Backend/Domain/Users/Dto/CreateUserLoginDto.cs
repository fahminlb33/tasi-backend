using System.ComponentModel.DataAnnotations;
using FluentValidation;
using TASI.Backend.Domain.Users.Entities;

namespace TASI.Backend.Domain.Users.Dto
{
    public class CreateUserLoginDto
    {
        public int UserId { get; set; }
        public UserRole Role { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class CreateUserLoginDtoValidator : AbstractValidator<CreateUserLoginDto>
    {
        public CreateUserLoginDtoValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().Length(5, 30);
            RuleFor(x => x.Username).NotEmpty().Length(5, 30);
            RuleFor(x => x.Password).NotEmpty().Length(5, 30);
            RuleFor(x => x.Role).NotEqual(UserRole.SuperAdmin);
        }
    }
}
