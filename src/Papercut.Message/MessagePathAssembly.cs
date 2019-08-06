
namespace Papercut.Message
{
    using System;
    using System.IO;
    using System.Collections.Generic;

    using Common.Extensions;
    using Common.Helper;

    using Core.Domain.Message;
    using Core.Domain.Paths;

    public class MessagePathAssembly : MessagePathAssemblyBase
    {
        public string Host { get; private set; }
        private readonly bool _ignoreFolders;
        private readonly List<string> _namePartsBeforeDate = new List<string>();
        private readonly List<string> _namePartsAfterDate = new List<string>();

        public MessagePathAssembly(string root, bool ignoreFolders = false)
        {
            if (!Path.IsPathRooted(root))
                throw new ArgumentException($"The path must be rooted: {root}", nameof(root));
            if (string.IsNullOrWhiteSpace(root))
                throw new ArgumentNullException(nameof(root));

            if(FoldersMaxLength < root.Length)
                throw new ArgumentOutOfRangeException(nameof(root));

            // It is important to prevent spaces as .NET crashes on trailing white spaces and creates leading ones during folder creation.
            var trimmed = root.Trim();

            var notEndedRoot = 
                // Normalizes a path and prevents multiple slashes.
                Path.GetFullPath(trimmed);

            // Removes a trailing slashes.
            if (notEndedRoot.EndsWith(Path.DirectorySeparatorChar.ToString())
                || notEndedRoot.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
            {
                notEndedRoot = notEndedRoot.Substring(0, notEndedRoot.Length - 1);
            }
            Host = notEndedRoot;

            _ignoreFolders = ignoreFolders;
        }

        public bool AddFolder(string folder)
        {
            if (_ignoreFolders)
                return false;

            if (FoldersMaxLength < Host.Length + ShortestFolderNameLength)
                return false;

             if (string.IsNullOrWhiteSpace(folder))
                return false;

            var trimmed = folder.Trim();

            var valid = trimmed.ValidPathPart();

            // Warning! Path.Combine doesn't removes leading and trailing white spaces.
            Host = Path.Combine(Host, valid);

            if(FoldersMaxLength < Host.Length)
                Host = Host.Substring(0, FoldersMaxLength);

            return true;
        }

        public void NamePartBeforeDate(string part)
        {
            if (string.IsNullOrWhiteSpace(part))
                return;

            var trimmed = part.Trim();
            var valid = trimmed.ValidPathPart();
            _namePartsBeforeDate.Add(valid);
        }

        public void NamePartAfterDate(string part)
        {
            if (string.IsNullOrWhiteSpace(part))
                return;

            var trimmed = part.Trim();
            var valid = trimmed.ValidPathPart();
            _namePartsAfterDate.Add(valid);
        }

        public void CreateDirectory()
        {
            Directory.CreateDirectory(Host);
        }

        public string GenerateFilePath(bool appendRandom = false)
        {
            CreateDirectory();

            var fileName = GenerateName(appendRandom);

            var path = Path.Combine(Host, fileName);

            return path;
        }

        private string GenerateName(bool appendRandom = false)
        {
            var allowedLength = WindowsMaxPathFoldersFile
                - Host.Length
                - ReservedNameLength;

            // It is a path logic error if allowedLength < 0.
            var allowedLengthBefore = allowedLength <= 0
                ? 0
                : allowedLength;

            var nameBeforeClipped = string.Empty;
            if (0 < allowedLengthBefore)
            {
                var nameBefore = string.Join(" ", _namePartsBeforeDate);
                nameBeforeClipped = nameBefore;
                if (allowedLengthBefore < nameBeforeClipped.Length)
                {
                    nameBeforeClipped = nameBeforeClipped.Substring(0, allowedLengthBefore);
                }
            }

            var allowedLengthAfter = allowedLengthBefore - nameBeforeClipped.Length;

            var nameAfterClipped = string.Empty;
            if (0 < allowedLengthAfter)
            {
                var nameAfter = string.Join(" ", _namePartsAfterDate).Prepend();
                nameAfterClipped = nameAfter;
                if (allowedLengthAfter < nameAfterClipped.Length)
                {
                    nameAfterClipped = nameAfterClipped.Substring(0, allowedLengthAfter);
                }
            }

            var timeStump = TimeStumpCurrent;
            var randomPart = appendRandom ? SmallRandomString() : string.Empty;

            var fullName =
                  nameBeforeClipped
                + timeStump
                + nameAfterClipped
                + randomPart.Prepend()
                + FileExtension
                ;

            return fullName;
        }
    }
}
