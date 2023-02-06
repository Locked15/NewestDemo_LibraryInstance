using NewestDemo_Library;

namespace NewestDemo_Tests
{
    [TestClass]
    public class MainTests
    {
        [TestMethod]
        public void AvailablePeriods_SendTimeFrom22To12_GetResultArrayLength18()
        {
            var starts = new TimeSpan[]
            {
                new TimeSpan(2, 30, 0),
                new TimeSpan(4, 0, 0)
            };
            var durations = new int[]
            {
                30,
                60
            };
            var begin = new TimeSpan(22, 0, 0);
            var end = new TimeSpan(12, 0, 0);
            var consultationTime = 40;

            var expectedResultLength = 18;
            var result = Calculations.AvailablePeriods(starts, durations, begin, end, consultationTime);

            Assert.IsTrue(result.Length == expectedResultLength);
        }
    }
}
