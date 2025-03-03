using FluentValidation.TestHelper;
using SMSRateLimiter.Application.DTOs;
using SMSRateLimiter.Application.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSRateLimiter.Application.Tests.ValidatorTests
{
    [TestFixture]
    public class SmsRequestValidatorTests
    {
        private readonly SmsRequestValidator _validator;

        public SmsRequestValidatorTests()
        {
            _validator = new SmsRequestValidator();
        }

        [Test]
        public void Should_Have_Error_When_PhoneNumber_Is_Empty()
        {
            var model = new SmsRequestDto { PhoneNumber = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
        }


        [Test]
        public void Should_Have_Error_When_PhoneNumber_Exceed_Char_Limit()
        {
            var model = new SmsRequestDto { PhoneNumber = "+12345678901234567" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
        }

        [Test]
        public void Should_Not_Have_Error_For_Valid_Model()
        {
            var model = new SmsRequestDto { PhoneNumber = "+1234567890", AccountId = 1};
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
