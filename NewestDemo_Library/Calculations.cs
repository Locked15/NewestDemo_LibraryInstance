using System.Text;

namespace NewestDemo_Library
{
    public class Calculations
    {
        private const bool IsWorkerLazy = false;

        private const string ResultArrayItemTemplate = "{0} — {1}";

        public static string[] AvailablePeriods(TimeSpan[] startTimes,
                                                int[] durations,
                                                TimeSpan beginWorkingTime,
                                                TimeSpan endWorkingTime,
                                                int consultationTime)
        {
            if (CompletePreExecutionChecks(beginWorkingTime, endWorkingTime, startTimes, durations, consultationTime, out string resultingMessage))
            {
                TimeSpan workingHours = CalculateWorkingHoursByValues(beginWorkingTime, endWorkingTime);
                (TimeSpan _timePassed, TimeSpan _currentTime) iterator = (new TimeSpan(), beginWorkingTime);
                IDictionary<TimeSpan, int> breakTimes = startTimes.Zip(durations, (key, value) => new { key, value })
                                                                  .ToDictionary(val => val.key, val => val.value);


                var results = CompleteIterationProcess(iterator, consultationTime, workingHours, breakTimes);
                return GenerateResponseByCalculationResult(results);
            }

            throw new ArgumentException(resultingMessage);
        }

        private static IList<(TimeSpan, TimeSpan)> CompleteIterationProcess((TimeSpan _timePassed, TimeSpan _currentTime) iterator, int consultationTime,
                                                                            TimeSpan workingHours, IDictionary<TimeSpan, int> breakTimes)
        {
            int breaksCount = 0;
            IList<(TimeSpan, TimeSpan)> results = new List<(TimeSpan, TimeSpan)>(1);

            while (iterator._timePassed < workingHours)
            {
                if (GetAvailableBreakValueForCurrentTime(iterator._currentTime, consultationTime, breakTimes) is var result && result != null)
                {
                    UpdateIteratorObject(result.Value.Item3 + result.Value.Item2, ref iterator);
                    ++breaksCount;
                }
                else
                {
                    TimeSpan leftTime = workingHours - iterator._timePassed;

                    if (leftTime.TotalMinutes >= consultationTime || !IsWorkerLazy)
                        ExecuteCurrentTask(consultationTime, results, ref iterator);
                    else
                        break;
                }
            }

            if (!CompleteBreaksCheck(breaksCount, breakTimes.Count))
                throw new ArgumentException("Ошибка: Отправленное расписание не позволяет провести указанные перерывы.");

            return results;
        }

        private static void ExecuteCurrentTask(double executionTime, IList<(TimeSpan, TimeSpan)> results,
                                               ref (TimeSpan _timePassed, TimeSpan _currentTime) iterator)
        {
            var timeBeforeExecution = iterator._currentTime;
            UpdateIteratorObject(executionTime, ref iterator);
            var timeAfterExecution = iterator._currentTime;

            results.Add((timeBeforeExecution, timeAfterExecution));
        }

        private static string[] GenerateResponseByCalculationResult(IList<(TimeSpan, TimeSpan)> results)
        {
            var builder = new StringBuilder();
            foreach (var result in results)
                builder.AppendLine(string.Format(ResultArrayItemTemplate, result.Item1, result.Item2));

            return builder.ToString().Split("\r\n").Where(str => str != string.Empty).ToArray();
        }

        #region Basic Calculuus Functions

        private static TimeSpan CalculateWorkingHoursByValues(TimeSpan beginWorkingTime, TimeSpan endWorkingTime)
        {
            if (beginWorkingTime > endWorkingTime)
                endWorkingTime = endWorkingTime.Add(TimeSpan.FromDays(1));

            return new TimeSpan(endWorkingTime.Ticks - beginWorkingTime.Ticks);
        }

        private static TimeSpan NormalizeCurrentTime(TimeSpan currentTime)
        {
            if (currentTime.Days > 0)
                currentTime = currentTime.Add(TimeSpan.FromDays(-1));

            return currentTime;
        }

        private static (TimeSpan, int, int)? GetAvailableBreakValueForCurrentTime(TimeSpan currentTime, int allowedSpace,
                                                                                  IDictionary<TimeSpan, int> breakTimes)
        {
            foreach (var breakTime in breakTimes)
            {
                foreach (var minuteOffset in Enumerable.Range(0, allowedSpace + 1))
                {
                    if (breakTime.Key == currentTime.Add(TimeSpan.FromMinutes(minuteOffset)))
                        return (breakTime.Key, breakTime.Value, minuteOffset);
                }
            }

            return null;
        }
        #endregion

        #region Aside Functions

        private static bool CompletePreExecutionChecks(TimeSpan beginWorkingTime, TimeSpan endWorkingTime,
                                                       TimeSpan[] startTimes, int[] durations, int consultationTime,
                                                       out string message)
        {
            if (!startTimes.Any() || !durations.Any())
            {
                message = "Ошибка: Один из отправленных показателей был пустым.";
                return false;
            }
            else if (startTimes.Length != durations.Length)
            {
                message = "Ошибка: Количество перерывов и времен не совпадает.";
                return false;
            }

            message = string.Empty;
            return true;
        }

        private static void UpdateIteratorObject(double minutesToInsert, ref (TimeSpan _timePassed, TimeSpan _currentTime) iterator)
        {
            iterator._currentTime = iterator._currentTime.Add(TimeSpan.FromMinutes(minutesToInsert));
            iterator._timePassed = iterator._timePassed.Add(TimeSpan.FromMinutes(minutesToInsert));

            iterator._currentTime = NormalizeCurrentTime(iterator._currentTime);
        }

        private static bool CompleteBreaksCheck(int breaksCount, int declaredBreaks) => breaksCount == declaredBreaks;
        #endregion
    }
}
