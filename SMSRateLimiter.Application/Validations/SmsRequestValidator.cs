using FluentValidation;
using SMSRateLimiter.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSRateLimiter.Application.Validations
{
    public class SmsRequestValidator : AbstractValidator<SmsRequestDto>
    {
        public SmsRequestValidator()
        {
            // Message could be handled using resource files.

            RuleFor(x => x.AccountId)
               .GreaterThan(0).WithMessage("Account number is invalid.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?\d{10,15}$").WithMessage("Phone number is invalid. Use format +1234567890.");
        }
    }
}
