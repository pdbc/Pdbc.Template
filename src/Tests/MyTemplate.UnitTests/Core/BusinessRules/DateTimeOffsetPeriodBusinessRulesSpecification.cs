using System;
using Aertssen.Framework.Tests;
using NUnit.Framework;

namespace MyTemplate.UnitTests.Core.BusinessRules
{
    public class DateTimeOffsetPeriodBusinessRulesSpecification : ContextSpecification<DateTimeOffsetPeriodBusinessRules>
    {
        [TestCase(0, 0, false)]
        [TestCase(1, 0, true)]
        [TestCase(-1, 0, false)]
        [TestCase(0, 1, false)]
        [TestCase(0, -1, false)]
        public void Verify_period_rule_succeeds(Int32 addDays, Int32 addHours, Boolean expectedResult)
        {
            var from = DateTimeOffset.Now;
            var to = DateTimeOffset.Now.AddDays(addDays).AddHours(addHours);

            SUT.FromDateMustBeBeforeToDate(from, to).ShouldBeEqualTo(expectedResult);
        }
    }
}
