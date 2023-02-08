namespace OneTimeBuckAPI.Core
{
    public static class CustomExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }

        public static bool EqualsInsensitive(this string s, string s2)
        {
            return s.Equals(s2, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
