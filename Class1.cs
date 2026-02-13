using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DevTools
{
    #region StringUtils
    public static class StringUtils
    {
        public static string ToCamelCase(this string str) => string.IsNullOrEmpty(str) ? str : char.ToLowerInvariant(str[0]) + str.Substring(1);
        public static string ToPascalCase(this string str) => string.IsNullOrEmpty(str) ? str : char.ToUpperInvariant(str[0]) + str.Substring(1);
        public static string ToSnakeCase(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            var regex = new Regex("([a-z0-9])([A-Z])");
            return regex.Replace(str, "$1_$2").ToLower();
        }
        public static string Reverse(this string str) { var arr = str?.ToCharArray(); Array.Reverse(arr); return new string(arr ?? Array.Empty<char>()); }
        public static bool IsPalindrome(this string str)
        {
            if (string.IsNullOrEmpty(str)) return false;
            str = Regex.Replace(str.ToLower(), @"[^a-z0-9]", "");
            var arr = str.ToCharArray();
            Array.Reverse(arr);
            return str == new string(arr);
        }
        public static string Truncate(this string str, int length) => string.IsNullOrEmpty(str) || length <= 0 ? "" : str.Length <= length ? str : str.Substring(0, length);
        public static string Repeat(this string str, int count) => string.Concat(Enumerable.Repeat(str ?? "", count));
        public static string RemoveWhitespace(this string str) => string.IsNullOrEmpty(str) ? str : Regex.Replace(str, @"\s+", "");
        public static int WordCount(this string str) => string.IsNullOrEmpty(str) ? 0 : Regex.Matches(str, @"\w+").Count;
        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);
        public static bool ContainsIgnoreCase(this string str, string value) => str?.IndexOf(value ?? "", StringComparison.OrdinalIgnoreCase) >= 0;
        public static string RemoveDigits(this string str) => string.IsNullOrEmpty(str) ? str : Regex.Replace(str, @"\d", "");
        public static string KeepDigits(this string str) => string.IsNullOrEmpty(str) ? str : Regex.Replace(str, @"\D", "");
        public static string ToTitleCase(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            var words = str.Split(' ');
            for (int i = 0; i < words.Length; i++)
                if (!string.IsNullOrEmpty(words[i]))
                    words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
            return string.Join(' ', words);
        }
        public static string RemoveSpecialChars(this string str) => string.IsNullOrEmpty(str) ? str : Regex.Replace(str, @"[^a-zA-Z0-9\s]", "");

        public static string PadLeftCustom(this string str, int totalWidth, char paddingChar = ' ') => str?.PadLeft(totalWidth, paddingChar);
        public static string PadRightCustom(this string str, int totalWidth, char paddingChar = ' ') => str?.PadRight(totalWidth, paddingChar);

        // 2. Substrings
        public static string Left(this string str, int length) => string.IsNullOrEmpty(str) ? str : str.Substring(0, Math.Min(length, str.Length));
        public static string Right(this string str, int length) => string.IsNullOrEmpty(str) ? str : str.Substring(Math.Max(0, str.Length - length));
        public static string Mid(this string str, int start, int length) => string.IsNullOrEmpty(str) ? str : str.Substring(Math.Max(0, start), Math.Min(length, str.Length - start));

        // 3. Case transformations
        public static string SwapCase(this string str) => string.IsNullOrEmpty(str) ? str :
            new string(str.Select(c => char.IsLetter(c) ? (char.IsUpper(c) ? char.ToLower(c) : char.ToUpper(c)) : c).ToArray());
        public static string CapitalizeWords(this string str) => string.IsNullOrEmpty(str) ? str : Regex.Replace(str, @"\b[a-z]", m => m.Value.ToUpper());

        // 4. Trimming
        public static string TrimStart(this string str, string prefix) => str != null && str.StartsWith(prefix) ? str.Substring(prefix.Length) : str;
        public static string TrimEnd(this string str, string suffix) => str != null && str.EndsWith(suffix) ? str.Substring(0, str.Length - suffix.Length) : str;
        public static string TrimBetween(this string str, string start, string end)
        {
            if (string.IsNullOrEmpty(str)) return str;
            int i = str.IndexOf(start);
            int j = str.IndexOf(end, i + start.Length);
            return i >= 0 && j > i ? str.Remove(i, j - i + end.Length) : str;
        }

        // 5. Replacements
        public static string ReplaceIgnoreCase(this string str, string oldValue, string newValue) => Regex.Replace(str ?? "", Regex.Escape(oldValue ?? ""), newValue ?? "", RegexOptions.IgnoreCase);
        public static string RemoveAll(this string str, string value) => (str ?? "").Replace(value ?? "", "");
        public static string ReplaceMultiple(this string str, Dictionary<string, string> replacements)
        {
            if (string.IsNullOrEmpty(str) || replacements == null) return str;
            foreach (var kv in replacements) str = str.Replace(kv.Key, kv.Value);
            return str;
        }

        // 6. Checks
        public static bool StartsWithIgnoreCase(this string str, string prefix) => str?.StartsWith(prefix ?? "", StringComparison.OrdinalIgnoreCase) ?? false;
        public static bool EndsWithIgnoreCase(this string str, string suffix) => str?.EndsWith(suffix ?? "", StringComparison.OrdinalIgnoreCase) ?? false;
        public static bool ContainsAny(this string str, params string[] values) => values.Any(v => str?.Contains(v) ?? false);
        public static bool ContainsAll(this string str, params string[] values) => values.All(v => str?.Contains(v) ?? false);
        public static bool IsNumeric(this string str) => double.TryParse(str, out _);
        public static bool IsAlpha(this string str) => Regex.IsMatch(str ?? "", @"^[a-zA-Z]+$");
        public static bool IsAlphaNumeric(this string str) => Regex.IsMatch(str ?? "", @"^[a-zA-Z0-9]+$");
        public static bool IsLower(this string str) => str?.All(char.IsLower) ?? false;
        public static bool IsUpper(this string str) => str?.All(char.IsUpper) ?? false;

        // 7. Splitting and joining
        public static string[] SplitByLength(this string str, int length)
        {
            if (string.IsNullOrEmpty(str) || length <= 0) return new string[] { str };
            return Enumerable.Range(0, (int)Math.Ceiling(str.Length / (double)length))
                             .Select(i => str.Substring(i * length, Math.Min(length, str.Length - i * length))).ToArray();
        }
        public static string JoinWith(this IEnumerable<string> values, string separator) => values == null ? "" : string.Join(separator, values);
        public static string ReverseWords(this string str) => string.IsNullOrEmpty(str) ? str : string.Join(' ', str.Split(' ').Reverse());
        public static string ShuffleWords(this string str) { var words = str?.Split(' ').ToList(); if (words != null) CollectionUtils.Shuffle(words); return string.Join(' ', words); }

        // 8. Padding & alignment
        public static string Center(this string str, int totalWidth, char paddingChar = ' ')
        {
            if (string.IsNullOrEmpty(str) || str.Length >= totalWidth) return str;
            int padding = totalWidth - str.Length;
            return new string(paddingChar, padding / 2) + str + new string(paddingChar, padding - padding / 2);
        }

        // 9. Encoding & decoding
        public static string ToBase64(this string str) => Convert.ToBase64String(Encoding.UTF8.GetBytes(str ?? ""));
        public static string FromBase64(this string str)
        {
            try { return Encoding.UTF8.GetString(Convert.FromBase64String(str ?? "")); }
            catch { return null; }
        }
        public static string UrlEncode(this string str) => WebUtility.UrlEncode(str ?? "");
        public static string UrlDecode(this string str) => WebUtility.UrlDecode(str ?? "");

        // 10. Regex utilities
        public static IEnumerable<string> Matches(this string str, string pattern) => Regex.Matches(str ?? "", pattern).Cast<Match>().Select(m => m.Value);
        public static string RemovePattern(this string str, string pattern) => Regex.Replace(str ?? "", pattern, "");
        public static string ExtractDigits(this string str) => Regex.Replace(str ?? "", @"\D", "");
        public static string ExtractLetters(this string str) => Regex.Replace(str ?? "", @"[^a-zA-Z]", "");

        // 11. Formatting
        public static string TruncateWithEllipsis(this string str, int maxLength) => string.IsNullOrEmpty(str) || str.Length <= maxLength ? str : str.Substring(0, maxLength) + "...";
        public static string RepeatWithSeparator(this string str, int count, string separator) => string.Join(separator, Enumerable.Repeat(str ?? "", count));

        // 12. Reversing & swapping
        public static string ReverseWordsOrder(this string str) => string.IsNullOrEmpty(str) ? str : string.Join(' ', str.Split(' ').Reverse());
        public static string ReverseCharactersInWords(this string str) => string.IsNullOrEmpty(str) ? str : string.Join(' ', str.Split(' ').Select(w => new string(w.Reverse().ToArray())));

        // 13. Misc
        public static string RemoveVowels(this string str) => Regex.Replace(str ?? "", "[aeiouAEIOU]", "");
        public static string RemoveConsonants(this string str) => Regex.Replace(str ?? "", "[^aeiouAEIOU\\s]", "");
        public static int CountOccurrences(this string str, string value) => string.IsNullOrEmpty(str) || string.IsNullOrEmpty(value) ? 0 : Regex.Matches(str, Regex.Escape(value)).Count;
        public static string PadBoth(this string str, int totalWidth, char paddingChar = ' ')
        {
            int pad = totalWidth - (str?.Length ?? 0);
            if (pad <= 0) return str;
            int left = pad / 2, right = pad - left;
            return new string(paddingChar, left) + str + new string(paddingChar, right);
        }

    }
    #endregion

    #region TimeUtils
    public static class TimeUtils
    {
        // --- Basic Unix Timestamp & TimeAgo ---
        public static long CurrentUnixTimestamp() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        public static string TimeAgo(DateTime dt)
        {
            var span = DateTime.UtcNow - dt.ToUniversalTime();
            if (span.TotalSeconds < 60) return $"{Math.Floor(span.TotalSeconds)} seconds ago";
            if (span.TotalMinutes < 60) return $"{Math.Floor(span.TotalMinutes)} minutes ago";
            if (span.TotalHours < 24) return $"{Math.Floor(span.TotalHours)} hours ago";
            if (span.TotalDays < 30) return $"{Math.Floor(span.TotalDays)} days ago";
            if (span.TotalDays < 365) return $"{Math.Floor(span.TotalDays / 30)} months ago";
            return $"{Math.Floor(span.TotalDays / 365)} years ago";
        }

        // --- Start & End of Periods ---
        public static DateTime StartOfTodayUtc() => DateTime.UtcNow.Date;
        public static DateTime EndOfTodayUtc() => DateTime.UtcNow.Date.AddDays(1).AddTicks(-1);
        public static DateTime StartOfMonthUtc() => new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        public static DateTime EndOfMonthUtc() => StartOfMonthUtc().AddMonths(1).AddTicks(-1);
        public static DateTime StartOfYearUtc() => new DateTime(DateTime.UtcNow.Year, 1, 1);
        public static DateTime EndOfYearUtc() => StartOfYearUtc().AddYears(1).AddTicks(-1);
        public static DateTime StartOfWeekUtc(DayOfWeek startDay = DayOfWeek.Monday)
        {
            var today = DateTime.UtcNow;
            int diff = (7 + (today.DayOfWeek - startDay)) % 7;
            return today.Date.AddDays(-diff);
        }
        public static DateTime EndOfWeekUtc(DayOfWeek startDay = DayOfWeek.Monday) => StartOfWeekUtc(startDay).AddDays(7).AddTicks(-1);

        // --- Add/Subtract Time ---
        public static DateTime AddDaysUtc(int days) => DateTime.UtcNow.AddDays(days);
        public static DateTime AddHoursUtc(int hours) => DateTime.UtcNow.AddHours(hours);
        public static DateTime AddMinutesUtc(int minutes) => DateTime.UtcNow.AddMinutes(minutes);
        public static DateTime AddSecondsUtc(int seconds) => DateTime.UtcNow.AddSeconds(seconds);
        public static DateTime SubtractDaysUtc(int days) => DateTime.UtcNow.AddDays(-days);
        public static DateTime SubtractHoursUtc(int hours) => DateTime.UtcNow.AddHours(-hours);

        // --- Checks ---
        public static bool IsWeekend(DateTime dt) => dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday;
        public static bool IsWeekday(DateTime dt) => !IsWeekend(dt);
        public static bool IsLeapYear(int year) => DateTime.IsLeapYear(year);
        public static bool IsToday(DateTime dt) => dt.Date == DateTime.UtcNow.Date;
        public static bool IsYesterday(DateTime dt) => dt.Date == DateTime.UtcNow.Date.AddDays(-1);
        public static bool IsTomorrow(DateTime dt) => dt.Date == DateTime.UtcNow.Date.AddDays(1);
        public static bool IsSameDay(DateTime a, DateTime b) => a.Date == b.Date;

        // --- Differences / Calculations ---
        public static int DaysUntil(DateTime futureDate) => (futureDate.Date - DateTime.UtcNow.Date).Days;
        public static int DaysSince(DateTime pastDate) => (DateTime.UtcNow.Date - pastDate.Date).Days;
        public static double SecondsSince(DateTime pastDate) => (DateTime.UtcNow - pastDate.ToUniversalTime()).TotalSeconds;
        public static double MinutesSince(DateTime pastDate) => (DateTime.UtcNow - pastDate.ToUniversalTime()).TotalMinutes;
        public static double HoursSince(DateTime pastDate) => (DateTime.UtcNow - pastDate.ToUniversalTime()).TotalHours;
        public static double DaysBetween(DateTime a, DateTime b) => (b.Date - a.Date).TotalDays;
        public static double WeeksBetween(DateTime a, DateTime b) => DaysBetween(a, b) / 7;

        // --- Start/End of Specific Units ---
        public static DateTime StartOfDay(DateTime dt) => dt.Date;
        public static DateTime EndOfDay(DateTime dt) => dt.Date.AddDays(1).AddTicks(-1);
        public static DateTime StartOfMonth(DateTime dt) => new DateTime(dt.Year, dt.Month, 1);
        public static DateTime EndOfMonth(DateTime dt) => StartOfMonth(dt).AddMonths(1).AddTicks(-1);
        public static DateTime StartOfYear(DateTime dt) => new DateTime(dt.Year, 1, 1);
        public static DateTime EndOfYear(DateTime dt) => StartOfYear(dt).AddYears(1).AddTicks(-1);

        // --- Weekday Operations ---
        public static DateTime NextWeekday(DateTime from, DayOfWeek day)
        {
            int offset = ((int)day - (int)from.DayOfWeek + 7) % 7;
            return from.AddDays(offset == 0 ? 7 : offset);
        }
        public static DateTime PreviousWeekday(DateTime from, DayOfWeek day)
        {
            int offset = ((int)from.DayOfWeek - (int)day + 7) % 7;
            return from.AddDays(-(offset == 0 ? 7 : offset));
        }

        // --- Conversion Utilities ---
        public static long ToUnixTimestamp(DateTime dt) => new DateTimeOffset(dt.ToUniversalTime()).ToUnixTimeSeconds();
        public static DateTime FromUnixTimestamp(long ts) => DateTimeOffset.FromUnixTimeSeconds(ts).UtcDateTime;
        public static string ToIso8601(DateTime dt) => dt.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
        public static DateTime FromIso8601(string iso) => DateTime.Parse(iso, null, System.Globalization.DateTimeStyles.AdjustToUniversal);

        // --- Formatting / Pretty Print ---
        public static string Format(DateTime dt, string format = "yyyy-MM-dd HH:mm:ss") => dt.ToString(format);
        public static string ToShortDate(DateTime dt) => dt.ToShortDateString();
        public static string ToShortTime(DateTime dt) => dt.ToShortTimeString();
        public static string ToLongDate(DateTime dt) => dt.ToLongDateString();
        public static string ToLongTime(DateTime dt) => dt.ToLongTimeString();
        public static string ToCustomString(DateTime dt, string format) => dt.ToString(format);

        // --- Month / Year Utilities ---
        public static int DaysInMonth(int year, int month) => DateTime.DaysInMonth(year, month);
        public static int WeekOfYear(DateTime dt) => System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(dt, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        public static DateTime FirstDayOfMonth(int year, int month) => new DateTime(year, month, 1);
        public static DateTime LastDayOfMonth(int year, int month) => FirstDayOfMonth(year, month).AddMonths(1).AddDays(-1);

        // --- Miscellaneous ---
        public static DateTime Tomorrow() => DateTime.UtcNow.Date.AddDays(1);
        public static DateTime Yesterday() => DateTime.UtcNow.Date.AddDays(-1);
        public static bool IsPast(DateTime dt) => dt < DateTime.UtcNow;
        public static bool IsFuture(DateTime dt) => dt > DateTime.UtcNow;
        public static DateTime Clamp(DateTime dt, DateTime min, DateTime max) => dt < min ? min : dt > max ? max : dt;
        public static string TimeUntilString(DateTime future)
        {
            var span = future - DateTime.UtcNow;
            if (span.TotalSeconds < 60) return $"{span.Seconds} seconds";
            if (span.TotalMinutes < 60) return $"{span.Minutes} minutes";
            if (span.TotalHours < 24) return $"{span.Hours} hours";
            return $"{span.Days} days";
        }
    }

    #endregion

    #region MathUtils
    public static class MathUtils
    {
        private static readonly Random _rnd = new Random();

        #region Basic
        public static int Clamp(int value, int min, int max) => Math.Min(Math.Max(value, min), max);
        public static double Clamp(double value, double min, double max) => Math.Min(Math.Max(value, min), max);
        public static double Lerp(double start, double end, double t) => start + (end - start) * t;
        public static int RandomInt(int min, int max) => _rnd.Next(min, max + 1);
        public static double RandomDouble(double min, double max) => _rnd.NextDouble() * (max - min) + min;
        public static long Factorial(int n) { long f = 1; for (int i = 2; i <= n; i++) f *= i; return f; }
        public static bool IsPrime(int n) { if (n < 2) return false; for (int i = 2; i <= Math.Sqrt(n); i++) if (n % i == 0) return false; return true; }
        public static int GCD(int a, int b) { while (b != 0) { int t = b; b = a % b; a = t; } return a; }
        public static int LCM(int a, int b) => (a * b) / GCD(a, b);
        public static double DegreesToRadians(double deg) => deg * Math.PI / 180;
        public static double RadiansToDegrees(double rad) => rad * 180 / Math.PI;
        public static double RoundToNearest(double value, double nearest) => Math.Round(value / nearest) * nearest;
        #endregion

        #region Advanced Arithmetic
        public static int Sum(params int[] nums) => nums.Sum();
        public static double Sum(params double[] nums) => nums.Sum();
        public static int Product(params int[] nums) => nums.Aggregate(1, (a, b) => a * b);
        public static double Product(params double[] nums) => nums.Aggregate(1.0, (a, b) => a * b);
        public static int Abs(int x) => Math.Abs(x);
        public static double Abs(double x) => Math.Abs(x);
        public static int Square(int x) => x * x;
        public static double Square(double x) => x * x;
        public static int Cube(int x) => x * x * x;
        public static double Cube(double x) => x * x * x;
        public static double Pow(double x, double y) => Math.Pow(x, y);
        public static double Sqrt(double x) => Math.Sqrt(x);
        public static double Cbrt(double x) => Math.Pow(x, 1.0 / 3);
        public static double Sign(double x) => Math.Sign(x);
        public static int Sign(int x) => Math.Sign(x);
        #endregion

        #region Trigonometry
        public static double Sin(double radians) => Math.Sin(radians);
        public static double Cos(double radians) => Math.Cos(radians);
        public static double Tan(double radians) => Math.Tan(radians);
        public static double ASin(double x) => Math.Asin(x);
        public static double ACos(double x) => Math.Acos(x);
        public static double ATan(double x) => Math.Atan(x);
        public static double ATan2(double y, double x) => Math.Atan2(y, x);
        public static double Sinh(double x) => Math.Sinh(x);
        public static double Cosh(double x) => Math.Cosh(x);
        public static double Tanh(double x) => Math.Tanh(x);
        public static double Sec(double x) => 1 / Math.Cos(x);
        public static double Csc(double x) => 1 / Math.Sin(x);
        public static double Cot(double x) => 1 / Math.Tan(x);
        #endregion

        #region Combinatorics
        public static long Permutations(int n, int r) => Factorial(n) / Factorial(n - r);
        public static long Combinations(int n, int r) => Factorial(n) / (Factorial(r) * Factorial(n - r));
        public static double BinomialCoefficient(int n, int k) => Combinations(n, k);
        public static double PascalTriangleValue(int row, int col) => Combinations(row, col);
        public static long Fibonacci(int n) { long a = 0, b = 1; for (int i = 0; i < n; i++) { var t = a; a = b; b = t + b; } return a; }
        public static IEnumerable<long> FibonacciSequence(int n) { long a = 0, b = 1; for (int i = 0; i < n; i++) { yield return a; var t = a; a = b; b = t + b; } }
        #endregion

        #region Number Theory
        public static bool IsEven(int n) => n % 2 == 0;
        public static bool IsOdd(int n) => n % 2 != 0;
        public static bool IsDivisibleBy(int n, int divisor) => n % divisor == 0;
        public static IEnumerable<int> PrimeFactors(int n)
        {
            for (int i = 2; i <= n; i++) while (n % i == 0) { yield return i; n /= i; }
        }
        public static int SumOfDigits(int n) => n.ToString().Select(c => int.Parse(c.ToString())).Sum();
        public static int ReverseDigits(int n) => int.Parse(n.ToString().Reverse().ToArray());
        public static int DigitCount(int n) => n.ToString().Length;
        #endregion

        #region Rounding & Approximation
        public static double Floor(double x) => Math.Floor(x);
        public static double Ceiling(double x) => Math.Ceiling(x);
        public static double Round(double x, int decimals = 0) => Math.Round(x, decimals);
        public static double Truncate(double x) => Math.Truncate(x);
        public static double RoundUpToNearest(double value, double nearest) => Math.Ceiling(value / nearest) * nearest;
        public static double RoundDownToNearest(double value, double nearest) => Math.Floor(value / nearest) * nearest;
        #endregion

        #region Statistics
        public static double Mean(params double[] nums) => nums.Average();
        public static double Median(params double[] nums)
        {
            var sorted = nums.OrderBy(x => x).ToArray();
            int n = sorted.Length;
            if (n % 2 == 0) return (sorted[n / 2 - 1] + sorted[n / 2]) / 2.0;
            return sorted[n / 2];
        }
        public static double Variance(params double[] nums)
        {
            double mean = Mean(nums);
            return nums.Sum(x => Math.Pow(x - mean, 2)) / nums.Length;
        }
        public static double StdDev(params double[] nums) => Math.Sqrt(Variance(nums));
        public static double Min(params double[] nums) => nums.Min();
        public static double Max(params double[] nums) => nums.Max();
        public static int IndexOfMin(params double[] nums) => Array.IndexOf(nums, nums.Min());
        public static int IndexOfMax(params double[] nums) => Array.IndexOf(nums, nums.Max());
        #endregion

        #region Random Utilities
        public static double RandomBetween(double min, double max) => _rnd.NextDouble() * (max - min) + min;
        public static int RandomSign() => _rnd.Next(0, 2) == 0 ? -1 : 1;
        public static int RandomEven(int min, int max) => RandomInt(min / 2, max / 2) * 2;
        public static int RandomOdd(int min, int max) => RandomInt(min / 2, max / 2) * 2 + 1;
        public static int RandomFromArray(params int[] arr) => arr[_rnd.Next(arr.Length)];
        public static double RandomFromArray(params double[] arr) => arr[_rnd.Next(arr.Length)];
        public static T RandomEnum<T>() where T : Enum
        {
            var values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(_rnd.Next(values.Length));
        }
        #endregion

        #region Geometry
        public static double Distance2D(double x1, double y1, double x2, double y2) => Math.Sqrt(Square(x2 - x1) + Square(y2 - y1));
        public static double Distance3D(double x1, double y1, double z1, double x2, double y2, double z2) => Math.Sqrt(Square(x2 - x1) + Square(y2 - y1) + Square(z2 - z1));
        public static double Hypotenuse(double a, double b) => Math.Sqrt(a * a + b * b);
        public static double CircleArea(double radius) => Math.PI * radius * radius;
        public static double CircleCircumference(double radius) => 2 * Math.PI * radius;
        public static double SphereVolume(double radius) => 4.0 / 3 * Math.PI * Math.Pow(radius, 3);
        public static double SphereSurfaceArea(double radius) => 4 * Math.PI * Math.Pow(radius, 2);
        public static double RectangleArea(double width, double height) => width * height;
        public static double RectanglePerimeter(double width, double height) => 2 * (width + height);
        public static double CubeVolume(double side) => Math.Pow(side, 3);
        public static double CubeSurfaceArea(double side) => 6 * Math.Pow(side, 2);
        #endregion
    }

    #endregion

    #region CollectionUtils
    public static class CollectionUtils
    {
        public static void Shuffle<T>(List<T> list)
        {
            var rnd = new Random();
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        public static bool IsNullOrEmpty<T>(IEnumerable<T> col) => col == null || !col.Any();
        public static IEnumerable<List<T>> Chunk<T>(IEnumerable<T> col, int size)
        {
            var list = col.ToList();
            for (int i = 0; i < list.Count; i += size)
                yield return list.GetRange(i, Math.Min(size, list.Count - i));
        }
        public static IEnumerable<T> DistinctBy<T, TKey>(IEnumerable<T> col, Func<T, TKey> keySelector) => col.GroupBy(keySelector).Select(g => g.First());
        public static List<T> RemoveNulls<T>(List<T> list) where T : class { list.RemoveAll(x => x == null); return list; }
        public static IEnumerable<T> ConcatSafe<T>(IEnumerable<T> a, IEnumerable<T> b) => (a ?? Enumerable.Empty<T>()).Concat(b ?? Enumerable.Empty<T>());
        public static Dictionary<TKey, TValue> ToDictionarySafe<T, TKey, TValue>(IEnumerable<T> col, Func<T, TKey> k, Func<T, TValue> v) => (col ?? Enumerable.Empty<T>()).ToDictionary(k, v);
        public static void ReverseInPlace<T>(List<T> list) => list.Reverse();
    }
    #endregion

    #region FileUtils
    public static class FileUtils
    {
        #region File Read/Write
        public static string ReadAllTextSafe(string path) => File.Exists(path) ? File.ReadAllText(path) : null;
        public static void WriteAllTextSafe(string path, string content) => File.WriteAllText(path, content ?? "");
        public static byte[] ReadAllBytesSafe(string path) => File.Exists(path) ? File.ReadAllBytes(path) : null;
        public static void WriteAllBytesSafe(string path, byte[] data) { if (data == null) data = Array.Empty<byte>(); File.WriteAllBytes(path, data); }
        public static IEnumerable<string> ReadLinesSafe(string path) => File.Exists(path) ? File.ReadLines(path) : Enumerable.Empty<string>();
        public static void AppendTextSafe(string path, string content) { File.AppendAllText(path, content ?? ""); }
        public static void AppendLinesSafe(string path, IEnumerable<string> lines) { File.AppendAllLines(path, lines ?? Enumerable.Empty<string>()); }
        public static void WriteLinesSafe(string path, IEnumerable<string> lines) { File.WriteAllLines(path, lines ?? Enumerable.Empty<string>()); }
        public static string ReadFirstLine(string path) => File.Exists(path) ? File.ReadLines(path).FirstOrDefault() : null;
        public static string ReadLastLine(string path) => File.Exists(path) ? File.ReadLines(path).LastOrDefault() : null;
        #endregion

        #region File Existence & Info
        public static bool FileExists(string path) => File.Exists(path);
        public static bool DirectoryExists(string path) => Directory.Exists(path);
        public static long FileSize(string path) => File.Exists(path) ? new FileInfo(path).Length : 0;
        public static DateTime FileCreatedTime(string path) => File.Exists(path) ? File.GetCreationTimeUtc(path) : DateTime.MinValue;
        public static DateTime FileModifiedTime(string path) => File.Exists(path) ? File.GetLastWriteTimeUtc(path) : DateTime.MinValue;
        public static string GetFileExtension(string path) => File.Exists(path) ? Path.GetExtension(path) : null;
        public static string GetFileName(string path) => File.Exists(path) ? Path.GetFileName(path) : null;
        public static string GetFileNameWithoutExtension(string path) => File.Exists(path) ? Path.GetFileNameWithoutExtension(path) : null;
        public static string GetDirectoryName(string path) => File.Exists(path) ? Path.GetDirectoryName(path) : null;
        #endregion

        #region File Manipulation
        public static void CopyFileSafe(string source, string dest) { if (File.Exists(source)) File.Copy(source, dest, true); }
        public static void MoveFileSafe(string source, string dest) { if (File.Exists(source)) File.Move(source, dest, true); }
        public static void DeleteFileSafe(string path) { if (File.Exists(path)) File.Delete(path); }
        public static void RenameFileSafe(string path, string newName)
        {
            if (!File.Exists(path)) return;
            string newPath = Path.Combine(Path.GetDirectoryName(path), newName);
            File.Move(path, newPath, true);
        }
        public static void CreateFileIfNotExists(string path) { if (!File.Exists(path)) File.WriteAllText(path, ""); }
        public static void TouchFile(string path) { if (File.Exists(path)) File.SetLastWriteTimeUtc(path, DateTime.UtcNow); else File.WriteAllText(path, ""); }
        public static void ClearFile(string path) { if (File.Exists(path)) File.WriteAllText(path, ""); }
        public static void TruncateFile(string path, long size)
        {
            if (!File.Exists(path)) return;
            using var fs = new FileStream(path, FileMode.Open);
            fs.SetLength(size);
        }
        #endregion

        #region Directory Utilities
        public static void CreateDirectoryIfNotExists(string path) { if (!Directory.Exists(path)) Directory.CreateDirectory(path); }
        public static void DeleteDirectorySafe(string path, bool recursive = true) { if (Directory.Exists(path)) Directory.Delete(path, recursive); }
        public static IEnumerable<string> ListFiles(string path, string searchPattern = "*.*", bool recursive = false)
            => Directory.Exists(path) ? Directory.GetFiles(path, searchPattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly) : Enumerable.Empty<string>();
        public static IEnumerable<string> ListDirectories(string path, string searchPattern = "*", bool recursive = false)
            => Directory.Exists(path) ? Directory.GetDirectories(path, searchPattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly) : Enumerable.Empty<string>();
        public static void CopyDirectorySafe(string source, string dest, bool overwrite = true)
        {
            if (!Directory.Exists(source)) return;
            Directory.CreateDirectory(dest);
            foreach (var file in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
            {
                string relative = Path.GetRelativePath(source, file);
                string targetPath = Path.Combine(dest, relative);
                Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
                File.Copy(file, targetPath, overwrite);
            }
        }
        public static void MoveDirectorySafe(string source, string dest) { if (Directory.Exists(source)) Directory.Move(source, dest); }
        #endregion

        #region Temporary Files & Random
        public static string CreateTempFile(string prefix = "tmp") => Path.Combine(Path.GetTempPath(), $"{prefix}_{Guid.NewGuid():N}.tmp");
        public static string CreateTempDirectory(string prefix = "tmp") { var dir = Path.Combine(Path.GetTempPath(), $"{prefix}_{Guid.NewGuid():N}"); Directory.CreateDirectory(dir); return dir; }
        public static void CleanTempDirectory(string path) { if (Directory.Exists(path)) Directory.Delete(path, true); }
        #endregion

        #region Search & Filters
        public static IEnumerable<string> FindFilesByExtension(string dir, string ext, bool recursive = true)
            => ListFiles(dir, $"*{ext}", recursive);
        public static IEnumerable<string> FindFilesContainingText(string dir, string text, bool recursive = true)
        {
            foreach (var file in ListFiles(dir, "*.*", recursive))
                if (File.ReadAllText(file).Contains(text)) yield return file;
        }
        public static IEnumerable<string> FindEmptyFiles(string dir, bool recursive = true)
            => ListFiles(dir, "*.*", recursive).Where(f => FileSize(f) == 0);
        #endregion

        #region File Comparisons
        public static bool AreFilesEqual(string file1, string file2)
        {
            if (!File.Exists(file1) || !File.Exists(file2)) return false;
            var b1 = File.ReadAllBytes(file1);
            var b2 = File.ReadAllBytes(file2);
            if (b1.Length != b2.Length) return false;
            for (int i = 0; i < b1.Length; i++) if (b1[i] != b2[i]) return false;
            return true;
        }
        public static bool IsFileEmpty(string path) => File.Exists(path) && new FileInfo(path).Length == 0;
        #endregion

        #region Misc
        public static string CombinePaths(params string[] paths) => Path.Combine(paths);
        public static string GetAbsolutePath(string path) => Path.GetFullPath(path);
        public static string GetRelativePath(string from, string to) => Path.GetRelativePath(from, to);
        public static string NormalizePath(string path) => Path.GetFullPath(new Uri(path).LocalPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        public static string ChangeFileExtension(string path, string newExt) => Path.ChangeExtension(path, newExt);
        #endregion
    }
    #endregion

    #region RandomUtils
    public static class RandomUtils
    {
        private static readonly Random _rnd = new Random();

        #region Basic Random
        public static bool RandomBool() => _rnd.Next(2) == 0;
        public static int RandomInt(int min, int max) => _rnd.Next(min, max + 1);
        public static long RandomLong(long min, long max) => (long)(_rnd.NextDouble() * (max - min) + min);
        public static double RandomDouble(double min, double max) => _rnd.NextDouble() * (max - min) + min;
        public static float RandomFloat(float min, float max) => (float)(_rnd.NextDouble() * (max - min) + min);
        public static decimal RandomDecimal(decimal min, decimal max) => (decimal)(_rnd.NextDouble() * (double)(max - min) + (double)min);
        public static DateTime RandomDate(DateTime start, DateTime end)
        {
            var range = (end - start).TotalSeconds;
            return start.AddSeconds(_rnd.NextDouble() * range);
        }
        public static Guid RandomGuid() => Guid.NewGuid();
        public static T RandomEnum<T>() where T : Enum
        {
            var values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(_rnd.Next(values.Length));
        }
        #endregion

        #region Random Strings
        private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private const string _alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const string _digits = "0123456789";
        private const string _hex = "0123456789ABCDEF";

        public static string RandomString(int length) => new string(Enumerable.Repeat(_chars, length).Select(s => s[_rnd.Next(s.Length)]).ToArray());
        public static string RandomAlpha(int length) => new string(Enumerable.Repeat(_alpha, length).Select(s => s[_rnd.Next(s.Length)]).ToArray());
        public static string RandomDigits(int length) => new string(Enumerable.Repeat(_digits, length).Select(s => s[_rnd.Next(s.Length)]).ToArray());
        public static string RandomHex(int length) => new string(Enumerable.Repeat(_hex, length).Select(s => s[_rnd.Next(s.Length)]).ToArray());
        public static string RandomPassword(int length, bool useSpecial = true)
        {
            string special = "!@#$%^&*()_+-={}[]|:;<>,.?/";
            string pool = _chars + (useSpecial ? special : "");
            return new string(Enumerable.Repeat(pool, length).Select(s => s[_rnd.Next(s.Length)]).ToArray());
        }
        #endregion

        #region Random Collections
        public static T RandomFromArray<T>(T[] arr) => arr[_rnd.Next(arr.Length)];
        public static T RandomFromList<T>(List<T> list) => list[_rnd.Next(list.Count)];
        public static IEnumerable<T> RandomSample<T>(IEnumerable<T> col, int count) => col.OrderBy(_ => _rnd.Next()).Take(count);
        public static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = _rnd.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
        public static IEnumerable<T> ShuffleEnumerable<T>(IEnumerable<T> col) => col.OrderBy(_ => _rnd.Next());
        #endregion

        #region Weighted Random
        public static T RandomWeighted<T>(Dictionary<T, double> items)
        {
            double total = items.Values.Sum();
            double r = _rnd.NextDouble() * total;
            foreach (var kv in items)
            {
                if ((r -= kv.Value) <= 0) return kv.Key;
            }
            return default;
        }
        #endregion

        #region Random Colors
        public static string RandomColorHex() => $"#{RandomHex(6)}";
        public static (int R, int G, int B) RandomColorRgb() => (_rnd.Next(256), _rnd.Next(256), _rnd.Next(256));
        #endregion

        #region Random Filenames & Paths
        public static string RandomFileName(string extension = "txt") => $"{Guid.NewGuid():N}.{extension}";
        public static string RandomDirectoryName() => $"dir_{Guid.NewGuid():N}";
        #endregion

        #region Misc
        public static bool CoinFlip() => RandomBool();
        public static int DiceRoll(int sides = 6) => _rnd.Next(1, sides + 1);
        public static double Gaussian(double mean = 0, double stdDev = 1)
        {
            double u1 = 1.0 - _rnd.NextDouble();
            double u2 = 1.0 - _rnd.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + stdDev * randStdNormal;
        }
        public static T RandomOrDefault<T>(IList<T> list, T defaultValue) => list.Count > 0 ? list[_rnd.Next(list.Count)] : defaultValue;
        public static T RandomOrNull<T>(IList<T> list) where T : class => list.Count > 0 ? list[_rnd.Next(list.Count)] : null;
        #endregion
    }
    #endregion

    #region UrlUtils
    public static class UrlUtils
    {
        #region Validation
        public static bool IsValidUrl(string url) => Uri.TryCreate(url, UriKind.Absolute, out _);
        public static bool IsHttpUrl(string url) => Uri.TryCreate(url, UriKind.Absolute, out var u) && (u.Scheme == Uri.UriSchemeHttp || u.Scheme == Uri.UriSchemeHttps);
        public static bool IsHttpsUrl(string url) => Uri.TryCreate(url, UriKind.Absolute, out var u) && u.Scheme == Uri.UriSchemeHttps;
        public static bool IsFileUrl(string url) => Uri.TryCreate(url, UriKind.Absolute, out var u) && u.Scheme == Uri.UriSchemeFile;
        public static bool HasQuery(string url) => Uri.TryCreate(url, UriKind.Absolute, out var u) && !string.IsNullOrEmpty(u.Query);
        public static bool HasFragment(string url) => Uri.TryCreate(url, UriKind.Absolute, out var u) && !string.IsNullOrEmpty(u.Fragment);
        #endregion

        #region Domain & Host
        public static string GetDomain(string url) => Uri.TryCreate(url, UriKind.Absolute, out var u) ? u.Host : null;
        public static string GetHostName(string url) => GetDomain(url);
        public static string GetSubDomain(string url)
        {
            var host = GetDomain(url);
            if (host == null) return null;
            var parts = host.Split('.');
            return parts.Length > 2 ? string.Join(".", parts.Take(parts.Length - 2)) : null;
        }
        public static string GetTld(string url)
        {
            var host = GetDomain(url);
            if (host == null) return null;
            var parts = host.Split('.');
            return parts.Length > 1 ? parts.Last() : null;
        }
        public static int GetPort(string url) => Uri.TryCreate(url, UriKind.Absolute, out var u) ? (u.IsDefaultPort ? -1 : u.Port) : -1;
        public static string GetScheme(string url) => Uri.TryCreate(url, UriKind.Absolute, out var u) ? u.Scheme : null;
        #endregion

        #region Path & Segments
        public static string GetPath(string url) => Uri.TryCreate(url, UriKind.Absolute, out var u) ? u.AbsolutePath : null;
        public static IEnumerable<string> GetPathSegments(string url) => Uri.TryCreate(url, UriKind.Absolute, out var u) ? u.Segments.Select(s => s.Trim('/')) : Enumerable.Empty<string>();
        public static string GetFileName(string url) => Uri.TryCreate(url, UriKind.Absolute, out var u) ? System.IO.Path.GetFileName(u.AbsolutePath) : null;
        public static string GetFileExtension(string url) => Uri.TryCreate(url, UriKind.Absolute, out var u) ? System.IO.Path.GetExtension(u.AbsolutePath) : null;
        public static string RemoveFileName(string url) => Uri.TryCreate(url, UriKind.Absolute, out var u) ? url.Substring(0, url.LastIndexOf(u.AbsolutePath) + 1) : url;
        public static string CombinePaths(string baseUrl, string relativePath)
        {
            if (!baseUrl.EndsWith("/")) baseUrl += "/";
            return new Uri(new Uri(baseUrl), relativePath).ToString();
        }
        public static string NormalizeUrl(string url) => Uri.TryCreate(url, UriKind.Absolute, out var u) ? u.ToString() : null;
        #endregion

        #region Query Parameters
        public static string AppendQueryParam(string url, string key, string value)
        {
            var uri = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uri.Query);
            query[key] = value;
            uri.Query = query.ToString();
            return uri.ToString();
        }
        public static string RemoveQueryParam(string url, string key)
        {
            var uri = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uri.Query);
            query.Remove(key);
            uri.Query = query.ToString();
            return uri.ToString();
        }
        public static string GetQueryParam(string url, string key)
        {
            var query = Uri.TryCreate(url, UriKind.Absolute, out var u) ? HttpUtility.ParseQueryString(u.Query) : null;
            return query?[key];
        }
        public static Dictionary<string, string> GetAllQueryParams(string url)
        {
            var dict = new Dictionary<string, string>();
            if (Uri.TryCreate(url, UriKind.Absolute, out var u))
            {
                var q = HttpUtility.ParseQueryString(u.Query);
                foreach (string k in q) dict[k] = q[k];
            }
            return dict;
        }
        public static string SetQueryParam(string url, string key, string value) => AppendQueryParam(url, key, value);
        public static string ClearQuery(string url)
        {
            var uri = new UriBuilder(url) { Query = "" };
            return uri.ToString();
        }
        public static bool QueryContains(string url, string key)
        {
            var q = GetAllQueryParams(url);
            return q.ContainsKey(key);
        }
        #endregion

        #region Fragment
        public static string GetFragment(string url) => Uri.TryCreate(url, UriKind.Absolute, out var u) ? u.Fragment.TrimStart('#') : null;
        public static string SetFragment(string url, string fragment)
        {
            var uri = new UriBuilder(url) { Fragment = fragment };
            return uri.ToString();
        }
        public static string RemoveFragment(string url)
        {
            var uri = new UriBuilder(url) { Fragment = "" };
            return uri.ToString();
        }
        #endregion

        #region Encoding/Decoding
        public static string UrlEncode(string value) => HttpUtility.UrlEncode(value);
        public static string UrlDecode(string value) => HttpUtility.UrlDecode(value);
        public static string Base64EncodeUrl(string value) => Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value));
        public static string Base64DecodeUrl(string value) => System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(value));
        #endregion

        #region Comparison & Misc
        public static bool UrlEquals(string url1, string url2) => string.Equals(NormalizeUrl(url1), NormalizeUrl(url2), StringComparison.OrdinalIgnoreCase);
        public static string RemoveScheme(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var u)) return url.Substring(u.Scheme.Length + 3);
            return url;
        }
        public static string ChangeScheme(string url, string scheme)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var u))
            {
                u = new UriBuilder(u) { Scheme = scheme, Port = -1 }.Uri;
                return u.ToString();
            }
            return url;
        }
        public static string EnsureTrailingSlash(string url) => url.EndsWith("/") ? url : url + "/";
        public static string RemoveTrailingSlash(string url) => url.EndsWith("/") ? url.TrimEnd('/') : url;
        public static string EnsureLeadingSlash(string path) => path.StartsWith("/") ? path : "/" + path;
        public static string RemoveLeadingSlash(string path) => path.StartsWith("/") ? path.TrimStart('/') : path;
        #endregion
    }
    #endregion

    #region JsonUtils
    public static class JsonUtils
    {
        #region Serialization / Deserialization
        public static string Serialize<T>(T obj, bool indented = false)
        {
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = indented });
        }

        public static T Deserialize<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return default;
            return JsonSerializer.Deserialize<T>(json);
        }

        public static object Deserialize(string json, Type type)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;
            return JsonSerializer.Deserialize(json, type);
        }

        public static string PrettyPrintJson(string json)
        {
            if (!IsValidJson(json)) return json;
            var obj = JsonSerializer.Deserialize<object>(json);
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
        }

        public static string MinifyJson(string json)
        {
            if (!IsValidJson(json)) return json;
            var obj = JsonSerializer.Deserialize<object>(json);
            return JsonSerializer.Serialize(obj);
        }
        #endregion

        #region Validation
        public static bool IsValidJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return false;
            try
            {
                JsonDocument.Parse(json);
                return true;
            }
            catch { return false; }
        }
        #endregion

        #region Merging
        public static string MergeJson(string json1, string json2)
        {
            var dict1 = Deserialize<Dictionary<string, object>>(json1) ?? new Dictionary<string, object>();
            var dict2 = Deserialize<Dictionary<string, object>>(json2) ?? new Dictionary<string, object>();
            foreach (var kv in dict2) dict1[kv.Key] = kv.Value;
            return Serialize(dict1);
        }

        public static string MergeJsonDeep(string json1, string json2)
        {
            var dict1 = Deserialize<Dictionary<string, object>>(json1) ?? new Dictionary<string, object>();
            var dict2 = Deserialize<Dictionary<string, object>>(json2) ?? new Dictionary<string, object>();
            foreach (var kv in dict2)
            {
                if (dict1.ContainsKey(kv.Key) && dict1[kv.Key] is JsonElement e1 && kv.Value is JsonElement e2 && e1.ValueKind == JsonValueKind.Object && e2.ValueKind == JsonValueKind.Object)
                {
                    var merged = MergeJson(e1.GetRawText(), e2.GetRawText());
                    dict1[kv.Key] = JsonSerializer.Deserialize<JsonElement>(merged);
                }
                else dict1[kv.Key] = kv.Value;
            }
            return Serialize(dict1);
        }

        public static string RemoveProperty(string json, string property)
        {
            var dict = Deserialize<Dictionary<string, object>>(json);
            if (dict != null && dict.ContainsKey(property)) dict.Remove(property);
            return Serialize(dict);
        }

        public static string RenameProperty(string json, string oldProp, string newProp)
        {
            var dict = Deserialize<Dictionary<string, object>>(json);
            if (dict != null && dict.ContainsKey(oldProp))
            {
                dict[newProp] = dict[oldProp];
                dict.Remove(oldProp);
            }
            return Serialize(dict);
        }
        #endregion

        #region Conversions
        public static Dictionary<string, object> ToDictionary(string json)
        {
            return Deserialize<Dictionary<string, object>>(json) ?? new Dictionary<string, object>();
        }

        public static List<T> ToList<T>(string json)
        {
            return Deserialize<List<T>>(json) ?? new List<T>();
        }

        public static T[] ToArray<T>(string json)
        {
            return Deserialize<T[]>(json) ?? Array.Empty<T>();
        }

        public static string FromDictionary(Dictionary<string, object> dict) => Serialize(dict);
        public static string FromList<T>(List<T> list) => Serialize(list);
        public static string FromArray<T>(T[] array) => Serialize(array);
        #endregion

        #region Safe Retrieval
        public static T GetProperty<T>(string json, string property)
        {
            var dict = ToDictionary(json);
            if (dict.ContainsKey(property))
            {
                try { return JsonSerializer.Deserialize<T>(Serialize(dict[property])); }
                catch { return default; }
            }
            return default;
        }

        public static bool HasProperty(string json, string property)
        {
            var dict = ToDictionary(json);
            return dict.ContainsKey(property);
        }
        #endregion

        #region Enum Handling
        public static string SerializeEnum<T>(T value) where T : Enum => JsonSerializer.Serialize(value);
        public static T DeserializeEnum<T>(string json) where T : Enum => JsonSerializer.Deserialize<T>(json);
        public static List<string> EnumNames<T>() where T : Enum => Enum.GetNames(typeof(T)).ToList();
        public static List<T> EnumValues<T>() where T : Enum => Enum.GetValues(typeof(T)).Cast<T>().ToList();
        #endregion

        #region Formatting Helpers
        public static string CamelCaseProperties(string json)
        {
            var dict = ToDictionary(json);
            var newDict = dict.ToDictionary(k => char.ToLowerInvariant(k.Key[0]) + k.Key.Substring(1), v => v.Value);
            return Serialize(newDict);
        }

        public static string PascalCaseProperties(string json)
        {
            var dict = ToDictionary(json);
            var newDict = dict.ToDictionary(k => char.ToUpperInvariant(k.Key[0]) + k.Key.Substring(1), v => v.Value);
            return Serialize(newDict);
        }

        public static string RemoveNullProperties(string json)
        {
            var dict = ToDictionary(json);
            var clean = dict.Where(kv => kv.Value != null).ToDictionary(kv => kv.Key, kv => kv.Value);
            return Serialize(clean);
        }
        #endregion

        #region Query Helpers
        public static bool PropertyEquals(string json, string property, object value)
        {
            var propValue = GetProperty<object>(json, property);
            return propValue != null && propValue.Equals(value);
        }

        public static string AddOrUpdateProperty(string json, string property, object value)
        {
            var dict = ToDictionary(json);
            dict[property] = value;
            return Serialize(dict);
        }

        public static string MergeProperties(string json, Dictionary<string, object> newProps)
        {
            var dict = ToDictionary(json);
            foreach (var kv in newProps) dict[kv.Key] = kv.Value;
            return Serialize(dict);
        }
        #endregion

        #region Misc Utilities
        public static bool IsEmpty(string json)
        {
            var dict = ToDictionary(json);
            return dict.Count == 0;
        }

        public static int PropertyCount(string json)
        {
            var dict = ToDictionary(json);
            return dict.Count;
        }
        #endregion
    }
    #endregion

    #region ReflectionUtils
    public static class ReflectionUtils
    {
        public static object GetPropertyValue(object obj, string propName) => obj.GetType().GetProperty(propName)?.GetValue(obj);
        public static void SetPropertyValue(object obj, string propName, object value) => obj.GetType().GetProperty(propName)?.SetValue(obj, value);
        public static IEnumerable<string> GetMethodNames(object obj) => obj.GetType().GetMethods().Select(m => m.Name);
        public static bool HasProperty(object obj, string propName) => obj.GetType().GetProperty(propName) != null;
        public static bool HasMethod(object obj, string methodName) => obj.GetType().GetMethod(methodName) != null;
    }
    #endregion

    #region ProcessUtils
    public static class ProcessUtils
    {
        public static bool IsProcessRunning(string name) => Process.GetProcessesByName(name).Any();
        public static void KillProcess(string name) { foreach (var p in Process.GetProcessesByName(name)) p.Kill(); }
        public static void StartProcess(string path, string args) => Process.Start(path, args);
        public static IEnumerable<Process> GetProcessesByName(string name) => Process.GetProcessesByName(name);
        public static void OpenFileWithDefaultApp(string path) => Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
    }
    #endregion

    #region CryptoUtils
    public static class CryptoUtils
    {
        public static string MD5Hash(string input) { using var md5 = MD5.Create(); return BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(input))).Replace("-", "").ToLower(); }
        public static string SHA256Hash(string input) { using var sha = SHA256.Create(); return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(input))).Replace("-", "").ToLower(); }
        public static string Base64Encode(string input) => Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        public static string Base64Decode(string input) => Encoding.UTF8.GetString(Convert.FromBase64String(input));
        public static string RandomKey(int length = 32) => RandomUtils.RandomString(length);
    }
    #endregion

    #region ValidationUtils
    public static class ValidationUtils
    {
        public static bool IsEmail(string s) => Regex.IsMatch(s ?? "", @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        public static bool IsPhoneNumber(string s) => Regex.IsMatch(s ?? "", @"^\+?\d{7,15}$");
        public static bool IsCreditCard(string s) => Regex.IsMatch(s ?? "", @"^\d{13,19}$");
        public static bool IsStrongPassword(string s) => Regex.IsMatch(s ?? "", @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$");
        public static bool IsUrl(string s) => Uri.TryCreate(s, UriKind.Absolute, out _);
    }
    #endregion

    #region ColorUtils
    public static class ColorUtils
    {
        public static (int R, int G, int B) HexToRgb(string hex)
        {
            hex = hex.Replace("#", "");
            if (hex.Length == 6)
            {
                return (
                    Convert.ToInt32(hex.Substring(0, 2), 16),
                    Convert.ToInt32(hex.Substring(2, 2), 16),
                    Convert.ToInt32(hex.Substring(4, 2), 16)
                );
            }
            throw new ArgumentException("Invalid hex color.");
        }

        public static string RgbToHex(int r, int g, int b) => $"#{r:X2}{g:X2}{b:X2}";

        private static readonly Random _rnd = new Random();
        public static string RandomColor() => RgbToHex(_rnd.Next(0, 256), _rnd.Next(0, 256), _rnd.Next(0, 256));

        public static (int R, int G, int B) BrightenColor(int r, int g, int b, double factor = 1.2)
        {
            r = Math.Min((int)(r * factor), 255);
            g = Math.Min((int)(g * factor), 255);
            b = Math.Min((int)(b * factor), 255);
            return (r, g, b);
        }

        public static (int R, int G, int B) DarkenColor(int r, int g, int b, double factor = 0.8)
        {
            r = Math.Max((int)(r * factor), 0);
            g = Math.Max((int)(g * factor), 0);
            b = Math.Max((int)(b * factor), 0);
            return (r, g, b);
        }
    }
    #endregion

    #region ConsoleUtils
    public static class ConsoleUtils
    {
        public static void WriteLineColor(string text, ConsoleColor color)
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = old;
        }

        public static void ClearLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop);
        }

        public static void WriteProgressBar(double percent)
        {
            int width = Console.WindowWidth - 10;
            int filled = (int)(percent * width);
            Console.Write("\r[" + new string('#', filled) + new string('-', width - filled) + $"] {percent:P0}");
        }

        public static void WriteTable(List<List<string>> rows)
        {
            if (!rows.Any()) return;
            int cols = rows.Max(r => r.Count);
            int[] widths = new int[cols];
            for (int c = 0; c < cols; c++)
                widths[c] = rows.Max(r => r.ElementAtOrDefault(c)?.Length ?? 0);

            foreach (var row in rows)
            {
                for (int c = 0; c < cols; c++)
                {
                    string cell = row.ElementAtOrDefault(c) ?? "";
                    Console.Write(cell.PadRight(widths[c] + 2));
                }
                Console.WriteLine();
            }
        }

        public static string ReadSecureInput()
        {
            var sb = new StringBuilder();
            ConsoleKeyInfo key;
            while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (key.Key == ConsoleKey.Backspace && sb.Length > 0) sb.Length--;
                else if (!char.IsControl(key.KeyChar)) sb.Append(key.KeyChar);
            }
            Console.WriteLine();
            return sb.ToString();
        }
    }
    #endregion

    #region DateUtils
    public static class DateUtils
    {
        public static DateTime NextWeekday(DateTime from) => from.AddDays(1 + ((int)DayOfWeek.Monday - (int)from.DayOfWeek + 7) % 7);
        public static DateTime PreviousWeekday(DateTime from) => from.AddDays(-1 - ((int)from.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7);
        public static int DaysInMonth(int year, int month) => DateTime.DaysInMonth(year, month);
        public static bool IsLeapYear(int year) => DateTime.IsLeapYear(year);
        public static bool IsToday(DateTime dt) => dt.Date == DateTime.Today;
    }
    #endregion

    #region NetworkUtils
    public static class NetworkUtils
    {
        public static bool PingHost(string host)
        {
            try
            {
                using var ping = new System.Net.NetworkInformation.Ping();
                var reply = ping.Send(host);
                return reply.Status == System.Net.NetworkInformation.IPStatus.Success;
            }
            catch { return false; }
        }

        public static string GetLocalIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString();
        }

        public static string DownloadString(string url)
        {
            using var wc = new WebClient();
            return wc.DownloadString(url);
        }

        public static int GetResponseCode(string url)
        {
            var req = WebRequest.Create(url) as HttpWebRequest;
            req.Method = "HEAD";
            using var resp = req.GetResponse() as HttpWebResponse;
            return (int)resp.StatusCode;
        }

        public static bool UrlExists(string url)
        {
            try { return GetResponseCode(url) < 400; }
            catch { return false; }
        }
    }
    #endregion

    #region LoggerUtils
    public static class LoggerUtils
    {
        public static void LogInfo(string msg) => ConsoleUtils.WriteLineColor("[INFO] " + msg, ConsoleColor.Cyan);
        public static void LogWarning(string msg) => ConsoleUtils.WriteLineColor("[WARN] " + msg, ConsoleColor.Yellow);
        public static void LogError(string msg) => ConsoleUtils.WriteLineColor("[ERROR] " + msg, ConsoleColor.Red);
        public static void LogDebug(string msg) => ConsoleUtils.WriteLineColor("[DEBUG] " + msg, ConsoleColor.Gray);
        public static void LogException(Exception ex) => LogError(ex.ToString());
    }
    #endregion

    #region EnumUtils
    public static class EnumUtils
    {
        public static string[] GetNames<T>() where T : Enum => Enum.GetNames(typeof(T));
        public static T[] GetValues<T>() where T : Enum => (T[])Enum.GetValues(typeof(T));
        public static T ParseIgnoreCase<T>(string value) where T : Enum => (T)Enum.Parse(typeof(T), value, true);
        public static Dictionary<string, T> ToDictionary<T>() where T : Enum =>
            Enum.GetValues(typeof(T)).Cast<T>().ToDictionary(e => e.ToString(), e => e);
        public static T RandomEnum<T>() where T : Enum => RandomUtils.RandomEnum<T>();
    }
    #endregion

    #region ConversionUtils
    public static class ConversionUtils
    {
        public static int ToInt(object o) => Convert.ToInt32(o);
        public static double ToDouble(object o) => Convert.ToDouble(o);
        public static bool ToBool(object o) => Convert.ToBoolean(o);
        public static decimal ToDecimal(object o) => Convert.ToDecimal(o);
        public static DateTime ToDateTime(object o) => Convert.ToDateTime(o);
    }
    #endregion

    #region ThreadingUtils
    public static class ThreadingUtils
    {
        public static void SleepMs(int ms) => Thread.Sleep(ms);

        public static async Task RunAsync(Func<Task> action) => await action();

        public static async Task RunDelayed(Func<Task> action, int delayMs)
        {
            await Task.Delay(delayMs);
            await action();
        }

        public static void RetryAction(Action action, int retries = 3, int delayMs = 1000)
        {
            for (int i = 0; i < retries; i++)
            {
                try { action(); return; }
                catch { if (i == retries - 1) throw; Thread.Sleep(delayMs); }
            }
        }

        public static async Task RunWithTimeout(Func<Task> action, int timeoutMs)
        {
            var task = action();
            if (await Task.WhenAny(task, Task.Delay(timeoutMs)) != task)
                throw new TimeoutException("Task timed out.");
        }
    }
    #endregion

}