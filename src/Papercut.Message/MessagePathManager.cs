namespace Papercut.Message
{
    using System.IO;
    using System.Linq;
    using Common.Extensions;
    using Core.Domain.Paths;
    using MimeKit;
    using Serilog;

    public class MessagePathManager
    {
        private const int SubjectFileNamePartLength = 60;
        private const int SubjectSubfolderNamePartLength = 40;


        readonly ILogger _logger;
        readonly IMessagePathConfigurator _messagePathConfigurator;

        public MessagePathManager(ILogger logger, IMessagePathConfigurator messagePathConfigurator)
        {
            _logger = logger;
            _messagePathConfigurator = messagePathConfigurator;
        }

        public FileStream CreateUniqueFile(MimeMessage message, out string messagePath, bool suppressSubfolders = false)
        {
            var path = BuildPath(message, suppressSubfolders);

            FileStream stream;
            var filePath = path.GenerateFilePath();
            while (!filePath.FileStreamTry(out stream, out var exception))
            {
                _logger.Error($"Error: {exception?.Message ?? "Unknown error"}; For: {filePath}");

                filePath = path.GenerateFilePath(appendRandom: true);
            }

            messagePath = filePath;

            return stream;
        }

        private MessagePathAssembly BuildPath(MimeMessage message, bool suppressSubfolders)
        {
            var path = new MessagePathAssembly(_messagePathConfigurator.DefaultSavePath, suppressSubfolders);

            BuildFolders(message, path);
            BuildName(message, path);

            return path;
        }

        private static void BuildFolders(MimeMessage message, MessagePathAssembly path)
        {
            var sender = message.Sender?.Address;
            if (string.IsNullOrEmpty(sender))
            {
                sender = message.From
                    .OfType<MailboxAddress>()
                    .FirstOrDefault()
                    ?.Address;
            }

            path.AddFolder(sender);

            path.AddFolder(message.Headers["Original-Envelope-ID"].Append() + message.Subject.TakeUpTo(SubjectSubfolderNamePartLength));
        }

        private static void BuildName(MimeMessage message, MessagePathAssembly path)
        {
            var toAddress = message.To
                .OfType<MailboxAddress>()
                .FirstOrDefault()
                ?.Address;
            path.NamePartBeforeDate(toAddress);

            path.NamePartBeforeDate(message.Headers["X-ContactId"]);

            // Added as an informing bonus.
            path.NamePartAfterDate(message.Subject.TakeUpTo(SubjectFileNamePartLength));
        }
    }
}
