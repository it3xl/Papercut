namespace Papercut.Core.Domain.Message
{
    using System;
    using System.Globalization;
    using System.IO;

    using Common.Extensions;

    /// <summary>
    /// You must delete all existing email files if you change any parameters of this class!
    /// </summary>
    public abstract class MessagePathAssemblyBase
    {
        /// <summary>
        /// Message file name extension with the dot prefix.<para/>
        /// !!! Delete all existing message files if you'll change this value.
        /// </summary>
        protected const string FileExtension = ".eml";

        /// <summary>
        /// The mark sign for the date-time stamp in the file name if the stamp isn't first.<para/>
        /// !!! Delete all existing message files if you'll change this value.
        /// </summary>
        protected const char DateStampPrefix = '^';

        /// <summary>
        /// Format for the date-time stamp in the message file name.<para/>
        /// !!! Delete all existing message files if you'll change this value.
        /// </summary>
        /// <remarks>Bug. Do not use FFF instead of fff! I.e. with FFF for 170ms we'll get 17 and we'll be forced to use RegEx dirty coding.</remarks>
        //public const string StampFormat = "yyyyMMddHHmmssFFF";
        protected const string StampFormat = "yy.MM.dd-HHmm-ss-fff";

        /// <summary>
        /// 260 - 1 (invisible terminating null character for the current system codepage)
        /// https://docs.microsoft.com/en-us/windows/win32/fileio/naming-a-file
        /// </summary>
        public const int WindowsMaxPathFoldersFile = 259;
        private const int WindowsMaxPathFolders = 247;

        protected const int ShortestFolderNameLength = 5;

        public static string TimeStumpCurrent => DateStampPrefix + DateTime.Now.ToString(StampFormat);

        private static int _maxPathFolders;
        /// <summary>
        /// Max allowed length for a path without a file name.
        /// </summary>
        public static int FoldersMaxLength
        {
            get
            {
                if (_maxPathFolders != 0)
                    return _maxPathFolders;

                _maxPathFolders = WindowsMaxPathFolders - ReservedNameLength;

                return _maxPathFolders;
            }
        }

        private static int _reservedPathLength;
        protected static int ReservedNameLength
        {
            get
            {
                if (_reservedPathLength != 0)
                    return _reservedPathLength;

                var directorySeparatorLength = 1;

                var randomPartLength = SmallRandomString().Prepend().Length;

                _reservedPathLength =
                      directorySeparatorLength
                    + TimeStumpCurrent.Length
                    + randomPartLength
                    + FileExtension.Length;

                return _reservedPathLength;
            }
        }

        public static DateTime? DateFromName(FileInfo file)
        {
            // it3xl.ru: I don't know why Papercut team prefers to use a date-time stamp in message file-names, I just keep their logic.
            // Possibly they separate a message received time and message file creation time.

            var fileName = file.Name;

            var dateTimePart = fileName.Substring(0, StampFormat.Length);
            var success = DateTime.TryParseExact(dateTimePart,
                StampFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var receivedDateTime);

            if (success)
            {
                return receivedDateTime;
            }

            var stampPrefixIndex = fileName.LastIndexOf(DateStampPrefix);
            var hasTimeStamp = 0 < stampPrefixIndex;
            if (hasTimeStamp)
            {
                var stampStartIndex = stampPrefixIndex + 1;
                dateTimePart = fileName.Substring(stampStartIndex, StampFormat.Length);

                success = DateTime.TryParseExact(dateTimePart,
                    StampFormat,
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

        public static string SmallRandomString()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 3);
        }
    }
}