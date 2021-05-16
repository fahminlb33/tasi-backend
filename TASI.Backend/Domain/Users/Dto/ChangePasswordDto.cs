using FluentValidation;

namespace TASI.Backend.Domain.Users.Dto
{
    public class ChangePasswordDto
    {
        public int LoginId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string RetypeNewPassword { get; set; }
    }

    public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordDtoValidator()
        {
            RuleFor(x => x.CurrentPassword).NotEmpty().Length(5, 30);
            RuleFor(x => x.NewPassword).NotEmpty().Length(5, 30);
            RuleFor(x => x.RetypeNewPassword).NotEmpty().Length(5, 30);
            RuleFor(x => x.RetypeNewPassword).Equal(x => x.NewPassword);
        }
    }
}
