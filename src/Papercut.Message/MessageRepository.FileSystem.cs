namespace Papercut.Message
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Core.Domain.Message;

    using MimeKit;

    public partial class MessageRepository
    {
        public const int SubjectFileNamePartLength = 60;
        public const int SubjectSubfolderNamePartLength = 40;

        private FileStream CreateUniqueFile(MimeMessage message, out string messagePath, bool suppressSubfolders = false)
        {
            var toAddress = message.To.FirstOrDefault()?.ToString();
            var subjectPart = message.Subject.TakeUpTo(SubjectFileNamePartLength);
            var contactId = message.Headers["X-ContactId"];

            string GetFileName(bool appendRandom = false)
            {
                var rootName = $"{toAddress}{subjectPart.Prepend()}";
                var fileName = rootName
                    .ToFileName(contactId, appendRandom);

                return fileName;
            }

            FileStream stream;
            var messageFolder = CreateFolder(message, suppressSubfolders);
            var filePath = messageFolder.AddPath(GetFileName());

            while (!filePath.GetStreamTry(out stream, out var exception))
            {
                _logger.Error($"Error: {exception?.Message ?? "Unknown error"}; For: {filePath}");

                filePath = messageFolder.AddPath(GetFileName(appendRandom: true));
            }

            messagePath = filePath;

            return stream;
        }

        private string CreateFolder(MimeMessage message, bool suppressSubfolders = false)
        {
            var hostPath = _messagePathConfigurator.DefaultSavePath;

            if (suppressSubfolders)
                return hostPath;

            var folder = message.Sender?.Address;
            if (string.IsNullOrEmpty(folder))
            {
                folder = message.From
                    .OfType<MailboxAddress>()
                    .FirstOrDefault()
                    ?.Address;
            }

            var mailPath = hostPath
                .AddPath(folder)
                .AddPathPrefixed(relativePath: message.Headers["Original-Envelope-ID"],
                    prefix: message.Subject.TakeUpTo(SubjectSubfolderNamePartLength),
                    prefixSeparator: " ");

            Directory.CreateDirectory(mailPath);

            return mailPath;
        }
    }
}
