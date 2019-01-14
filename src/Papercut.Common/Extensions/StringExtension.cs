namespace Papercut.Common.Extensions
{
    using System.Linq;

    public static class StringExtension
    {
        public static string TakeUpTo(this string input, int length)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return new string(input.Take(length).ToArray());
        }

        public static string Prepend(this string input)
        {
            return input.PrependSpaceOrEmpty();
        }

        public static string PrependSpaceOrEmpty(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return " " + input;
        }

        public static string Append(this string input)
        {
            return input.AppendSpaceOrEmpty();
        }

        public static string AppendSpaceOrEmpty(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return input + " ";
        }

    }
}