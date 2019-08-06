namespace Papercut.Message
{
    using System;
    using System.IO;
    using System.Linq;

    using Common.Extensions;

    using Core.Domain.Message;
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
            const int suspiciousCrashesAfter = 10;
            var crashCount = 0;

            var path = BuildPath(message, suppressSubfolders);

            FileStream stream;
            var filePath = path.GenerateFilePath();
            while (!filePath.FileStreamTry(out stream, out var exception))
            {
                crashCount++;
                if (suspiciousCrashesAfter <= crashCount)
                {
                    var messageToFriend = @"Ask ""it aT it3xl.ru"" to fix it!";
                    _logger.Fatal(exception, messageToFriend);

                    // it3xl.ru: I've decided to see logs more often then have suffocated servers.
                    throw new Exception(messageToFriend, exception);

                    // To prevent suffocation of your machine.
                    //TaskExtension.Delay(TimeSpan.FromMilliseconds(300));
                }

                _logger.Error($"Error: {exception?.ToString() ?? "Unknown error"}; For: {filePath}");

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