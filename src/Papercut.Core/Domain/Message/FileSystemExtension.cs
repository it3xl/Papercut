namespace Papercut.Core.Domain.Message
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;

    public static class FileSystemExtension
    {
        private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();
        private const string EmptyStringReplacement = "_";

        public static string AddPathPrefixed(this string hostPath, string relativePath, string prefix, string prefixSeparator = null)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return hostPath;

            var validPrefix = prefix.MakeValidFileNameOrEmpty();
            if (!string.IsNullOrEmpty(validPrefix))
            {
                validPrefix += prefixSeparator;
            }

            var subfolderValid = $"{validPrefix}{relativePath}"
                .MakeValidFileNameOrEmpty();

            var resolvedPath = Path.Combine(hostPath, subfolderValid);

            return resolvedPath;
        }

        public static string AddPath(this string hostPath, string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return hostPath;

            var subfolderValid = relativePath.MakeValidFileName(string.Empty);

            var resolvedPath = Path.Combine(hostPath, subfolderValid);

            return resolvedPath;
        }

        public static string MakeValidFileNameOrEmpty(this string inputText)
        {
            return MakeValidFileName(inputText, string.Empty);
        }

        /// <summary>Replaces characters in <c>text</c> that are not allowed in 
        /// file names with the specified replacement character.<para/>
        /// https://stackoverflow.com/questions/620605/how-to-make-a-valid-windows-filename-from-an-arbitrary-string/25223884#25223884
        /// </summary>
        /// <param name="inputText">Text to make into a valid filename. The same string is returned if it is valid already.</param>
        /// <param name="replacement">Replacement character, or null to simply remove bad characters.</param>
        /// <param name="emptyText">A replacement for the empty result.</param>
        /// <param name="fancy">Whether to replace quotes and slashes with the non-ASCII characters ” and ⁄.</param>
        /// <returns>A string that can be used as a filename. If the output string would otherwise be empty,
        /// returns <see cref="EmptyStringReplacement"/>.</returns>
        public static string MakeValidFileName(this string inputText, string emptyText = EmptyStringReplacement, char? replacement = '_', bool fancy = true)
        {
            var text = inputText ?? string.Empty;

            var invalids = InvalidFileNameChars;

            emptyText = emptyText ?? string.Empty;
            if (!string.IsNullOrEmpty(emptyText) && emptyText != EmptyStringReplacement)
            {
                emptyText = MakeValidFileName(emptyText);
            }

            var sb = new StringBuilder(text.Length);
            var changed = false;
            foreach (var ch in text)
            {
                if (!invalids.Contains(ch))
                {
                    sb.Append(ch);

                    continue;
                }

                changed = true;
                var repl = replacement ?? '\0';
                if (fancy)
                {
                    switch (ch)
                    {
                        case '"':
                            // U+201D right double quotation mark
                            repl = '”';
                            break;
                        case '\'':
                            // U+2019 right single quotation mark
                            repl = '’';
                            break;
                        case '/':
                            // U+2044 fraction slash
                            repl = '⁄';
                            break;
                    }
                }

                if (repl != '\0')
                {
                    sb.Append(repl);
                }
            }

            if (sb.Length == 0)
            {
                return emptyText;
            }

            return changed ? sb.ToString() : text;
        }

        public static string TakeUpTo(this string input, int length)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return new string(input.Take(length).ToArray());
        }

        public static string Prepend(this string input)
        {
            return input.PrependSpaceOrNull();
        }

        public static string PrependSpaceOrNull(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            return " " + input;
        }

        public static bool GetStreamTry(this string filePath, out FileStream stream, out Exception exception)
        {
            try
            {
                stream = File.Create(filePath);

                exception = null;

                return true;
            }
            catch(Exception exc)
            {
                stream = null;

                exception = exc;

                return false;
            }
        }


    }
}