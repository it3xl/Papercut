namespace Papercut.Message
{
    using System.IO;
    using System.Linq;

    using Core.Domain.Message;

    using MimeKit;

    public partial class MessageRepository
    {
        public const int SubjectFileNamePartLength = 60;
        public const int SubjectSubfolderNamePartLength = 40;

        private FileStream CreateUniqueFile(MimeMessage message, out string messagePath)
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
            var messageFolder = CreateFolder(message);
            var filePath = messageFolder.AddPath(GetFileName());

            while (!filePath.GetStreamTry(out stream))
            {
                _logger.Error($"Already exists: {filePath}");

                filePath = messageFolder.AddPath(GetFileName(appendRandom: true));
            }

            messagePath = filePath;

            return stream;
        }

        private string CreateFolder(MimeMessage message)
        {
            var mailHostPath = _messagePathConfigurator.DefaultSavePath
                .AddPath(message.Sender?.Address)
                .AddPathPrefixed(relativePath: message.Headers["Original-Envelope-ID"],
                    prefix: message.Subject.TakeUpTo(SubjectSubfolderNamePartLength),
                    prefixSeparator: " ");

            Directory.CreateDirectory(mailHostPath);

            return mailHostPath;
        }
    }
}
