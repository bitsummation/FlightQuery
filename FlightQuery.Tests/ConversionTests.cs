using FlightQuery.Sdk;
using NUnit.Framework;
using System;

namespace FlightQuery.Tests
{
    [TestFixture]
    public class ConversionTests
    {
        [TestCase("2020/3/7 9:15", 3, 7, 2020)]
        [TestCase("3/15/2020 9:15", 3, 15, 2020)]
        [TestCase("1991-12-31", 12, 31, 1991)]
        public void TestDate(string dateString, int month, int day, int year)
        {
            var date = (DateTime)Conversion.ConvertStringToDateTime(dateString);
            Assert.IsTrue(date.Month == month);
            Assert.IsTrue(date.Day == day);
            Assert.IsTrue(date.Year == year);

        }
    }
}
