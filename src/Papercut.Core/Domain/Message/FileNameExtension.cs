namespace Papercut.Core.Domain.Message
{
    using System;
    using System.Globalization;
    using System.IO;

    using Common.Helper;

    public static class FileNameExtension
    {
        /// <summary>
        /// You must delete all existing email files if you change any parameters of this class!
        /// </summary>
        public static class FixFileParam
        {
            /// <summary>
            /// Message file name extension.<para/>
            /// You can use another message file extension. Just delete all existing message files.
            /// </summary>
            internal const string FileExtension = ".eml";

            /// <summary>
            /// The mark sign for the date-time stamp in the file name if the stamp isn't first.<para/>
            /// You can use another mark sign. Just delete all existing message files.
            /// </summary>
            public const char StampPrefix = '^';

            /// <summary>
            /// Format for the date-time stamp in the message file name.<para/>
            /// You can use another timestamp format. Just delete all existing message files.<para/>
            /// Bug. Do not use FFF instead of fff! I.e. with FFF for 170ms we'll get 17 and we'll be forced to use RegEx dirty coding.
            /// </summary>
            public const string StampFormat = "yy.MM.dd-HHmm-ss-fff";

            //public const string StampFormat = "yyyyMMddHHmmssFFF";
        }

        public static string ToFileName(this string rootName, bool appendRandom = false)
        {
            var dateTimeFormatted = DateTime.Now.ToString(FixFileParam.StampFormat);
            var randomPart = appendRandom ? StringHelpers.SmallRandomString() : null;

            var fullName =
                $"{rootName}{FixFileParam.StampPrefix}{dateTimeFormatted}{randomPart.Prepend()}{FixFileParam.FileExtension}";

            var valid = fullName
                .MakeValidFileName();

            return valid;
        }

        public static DateTime? GetCreatedDate(this FileInfo file)
        {
            // it3xl.ru: I don't why Papercut uses the date-time stamp in message file names and I save their logic.
            // Possibly they separate a message received time and message file creation time.

            string fileName = file.Name;

            var dateTimePart = fileName.Substring(0, FixFileParam.StampFormat.Length);
            var success = DateTime.TryParseExact(dateTimePart,
                FixFileParam.StampFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var receivedDateTime);

            if (success)
            {
                return receivedDateTime;
            }

            var stampPrefixIndex = fileName.LastIndexOf(FixFileParam.StampPrefix);
            var hasTimeStamp = 0 < stampPrefixIndex;
            if (hasTimeStamp)
            {
                var stampStartIndex = stampPrefixIndex + 1;
                dateTimePart = fileName.Substring(stampStartIndex, FixFileParam.StampFormat.Length);

                success = DateTime.TryParseExact(dateTimePart,
                    FixFileParam.StampFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out receivedDateTime);

                if (success)
                {
                    return receivedDateTime;
                }
            }

            receivedDateTime = file.CreationTime;

            return receivedDateTime;
        }
    }
}