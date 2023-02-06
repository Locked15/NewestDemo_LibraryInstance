using NewestDemo_Library;

namespace Temp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var starts = new TimeSpan[]
            {
                new(10, 00, 0),
                new(11, 00, 0),
                new(15, 00, 0),
                new(15, 30, 0),
                new(16, 50, 0)
            };
            var durations = new int[]
            {
                60,
                30,
                10,
                10,
                40
            };
            var begin = new TimeSpan(08, 00, 00);
            var end = new TimeSpan(18, 00, 00);
            var consultationTime = 30;

            var res = Calculations.AvailablePeriods(starts, durations, begin, end, consultationTime);
            res.ToString();
        }
    }
}
