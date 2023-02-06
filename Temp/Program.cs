using NewestDemo_Library;

namespace Temp
{
    internal class Program
    {
        static void Main(string[] args)
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

            var res = Calculations.AvailablePeriods(starts, durations, begin, end, consultationTime);
            res.ToString();
        }
    }
}
