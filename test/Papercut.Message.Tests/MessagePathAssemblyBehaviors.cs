namespace Papercut.Message.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Core.Domain.Message;

    [TestClass]
    public class MessagePathAssemblyBehaviors
    {
        #region private static string ExtraLongDirName = "..."
        private static string ExtraLongDirName = "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890";
        #endregion


        public static string RootPath{ get; set; }
        public static int FolderLengthRemaining{ get; set; }

        [ClassInitialize]
        public static void TestClassInitialize(TestContext test)
        {
            RootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test");
            FolderLengthRemaining = MessagePathAssemblyBase.FoldersMaxLength - RootPath.Length;
        }

        [ClassCleanup]
        public static async Task TestClassCleanup()
        {
            try
            {
                Directory.Delete(RootPath, true);
            }
            catch
            {
                // It is a small hope to survive under a tension with an antivirus (file locking).
                await Task.Delay(0);
                Directory.Delete(RootPath, true);
            }
        }


        [TestMethod]
        public void HaveFixedAllowedFolderLength()
        {
            Assert.AreEqual(MessagePathAssemblyBase.FoldersMaxLength, 217);
        }

        [TestMethod]
        public void AddsFolder()
        {
            var folderName = "1234567890";

            var a = new MessagePathAssembly(RootPath);
            Assert.IsTrue(a.AddFolder(folderName));

            var expectedPath = Path.Combine(RootPath, folderName);
            Assert.AreEqual(expectedPath, a.Host);

            a.CreateDirectory();
        }

        [TestMethod]
        public void AddsFolders()
        {
            var firstFolder = "1234567890";
            var secondFolder = "2234567890";
            var thirdFolder = "3234567890";

            var a = new MessagePathAssembly(RootPath);
            Assert.IsTrue(a.AddFolder(firstFolder));
            Assert.IsTrue(a.AddFolder(secondFolder));
            Assert.IsTrue(a.AddFolder(thirdFolder));

            var expectedPath = Path.Combine(RootPath, firstFolder, secondFolder, thirdFolder);
            Assert.AreEqual(expectedPath, a.Host);

            a.CreateDirectory();
        }

        [TestMethod]
        public void LimitsFolderLength()
        {
            var a = new MessagePathAssembly(RootPath);
            Assert.IsTrue(a.AddFolder(ExtraLongDirName));

            Assert.AreEqual(MessagePathAssemblyBase.FoldersMaxLength, a.Host.Length);

            a.CreateDirectory();
        }

        [TestMethod]
        public void CutsFoldersCorrectly()
        {
            var slashLength = "\\".Length;

            var folderName = "1234567890";

            var left0 = ExtraLongDirName.Substring(0, FolderLengthRemaining - slashLength);
            var a0 = new MessagePathAssembly(RootPath);
            Assert.IsTrue(a0.AddFolder(left0));
            Assert.AreEqual(Path.Combine(RootPath, left0), a0.Host);
            Assert.IsFalse(a0.AddFolder(folderName));
            a0.CreateDirectory();

            var left1 = ExtraLongDirName.Substring(0, FolderLengthRemaining - 1 - slashLength);
            var a1 = new MessagePathAssembly(RootPath);
            Assert.IsTrue(a1.AddFolder(left1));
            Assert.AreEqual(Path.Combine(RootPath, left1), a1.Host);
            Assert.IsFalse(a1.AddFolder(folderName));
            a1.CreateDirectory();

            var left2 = ExtraLongDirName.Substring(0, FolderLengthRemaining - 2 - slashLength);
            var a2 = new MessagePathAssembly(RootPath);
            Assert.IsTrue(a2.AddFolder(left2));
            Assert.AreEqual(Path.Combine(RootPath, left2), a2.Host);
            Assert.IsFalse(a2.AddFolder(folderName));
            a2.CreateDirectory();

            var left3 = ExtraLongDirName.Substring(0, FolderLengthRemaining - 3 - slashLength);
            var a3 = new MessagePathAssembly(RootPath);
            Assert.IsTrue(a3.AddFolder(left3));
            Assert.AreEqual(Path.Combine(RootPath, left3), a3.Host);
            Assert.IsFalse(a3.AddFolder(folderName));
            a3.CreateDirectory();

            var left4 = ExtraLongDirName.Substring(0, FolderLengthRemaining - 4 - slashLength);
            var a4 = new MessagePathAssembly(RootPath);
            Assert.IsTrue(a4.AddFolder(left4));
            Assert.AreEqual(Path.Combine(RootPath, left4), a4.Host);
            Assert.IsFalse(a4.AddFolder(folderName));
            a4.CreateDirectory();


            var left5 = ExtraLongDirName.Substring(0, FolderLengthRemaining - 5 - slashLength);
            var a5 = new MessagePathAssembly(RootPath);
            Assert.IsTrue(a5.AddFolder(left5));
            Assert.AreEqual(Path.Combine(RootPath, left5), a5.Host);
            Assert.IsTrue(a5.AddFolder(folderName));
            Assert.IsTrue(a5.Host.EndsWith(@"\1234"));
            a5.CreateDirectory();

            var left6 = ExtraLongDirName.Substring(0, FolderLengthRemaining - 6 - slashLength);
            var a6 = new MessagePathAssembly(RootPath);
            Assert.IsTrue(a6.AddFolder(left6));
            Assert.AreEqual(Path.Combine(RootPath, left6), a6.Host);
            Assert.IsTrue(a6.AddFolder(folderName));
            Assert.IsTrue(a6.Host.EndsWith(@"\12345"));
            a6.CreateDirectory();

            var left7 = ExtraLongDirName.Substring(0, FolderLengthRemaining - 7 - slashLength);
            var a7 = new MessagePathAssembly(RootPath);
            Assert.IsTrue(a7.AddFolder(left7));
            Assert.AreEqual(Path.Combine(RootPath, left7), a7.Host);
            Assert.IsTrue(a7.AddFolder(folderName));
            Assert.IsTrue(a7.Host.EndsWith(@"\123456"));
            a7.CreateDirectory();

            var left8 = ExtraLongDirName.Substring(0, FolderLengthRemaining - 8 - slashLength);
            var a8 = new MessagePathAssembly(RootPath);
            Assert.IsTrue(a8.AddFolder(left8));
            Assert.AreEqual(Path.Combine(RootPath, left8), a8.Host);
            Assert.IsTrue(a8.AddFolder(folderName));
            Assert.IsTrue(a8.Host.EndsWith(@"\1234567"));
            a8.CreateDirectory();
        }

        [TestMethod]
        public void IgnoresFoldersOnMaxFolderLimitExceeded()
        {
            var a = new MessagePathAssembly(RootPath);
            Assert.IsTrue(a.AddFolder(ExtraLongDirName));
            Assert.IsFalse(a.AddFolder("1234567890"));
            a.CreateDirectory();
        }

        [TestMethod]
        public void GeneratesFileName()
        {
            var a = new MessagePathAssembly(RootPath);
            Assert.IsTrue(a.AddFolder("folder 1"));
            Assert.IsTrue(a.AddFolder("folder 2"));

            a.NamePartBeforeDate("Some");
            a.NamePartBeforeDate("name");
            a.NamePartBeforeDate("before");
            a.NamePartBeforeDate("a date");

            a.NamePartAfterDate("Some");
            a.NamePartAfterDate("name");
            a.NamePartAfterDate("after");
            a.NamePartAfterDate("a date");

            var filePath = a.GenerateFilePath();
            File.Create(filePath)
                .Close();

            var randomFilePath = a.GenerateFilePath(true);

            Assert.IsTrue(randomFilePath.Contains("before a date"));
            Assert.IsTrue(randomFilePath.Contains("after a date"));

            File.Create(randomFilePath)
                .Close();
        }

        [TestMethod]
        public void CutsNamePartBeforeDate()
        {
            var a = new MessagePathAssembly(RootPath);
            Assert.IsTrue(a.AddFolder(ExtraLongDirName));

            a.NamePartBeforeDate("Some");
            a.NamePartBeforeDate("name");
            a.NamePartBeforeDate("before");
            a.NamePartBeforeDate("a date");

            a.NamePartAfterDate("Some");
            a.NamePartAfterDate("name");
            a.NamePartAfterDate("after");
            a.NamePartAfterDate("a date");

            var filePath = a.GenerateFilePath();
            File.Create(filePath)
                .Close();

            var randomFilePath = a.GenerateFilePath(true);

            Assert.IsFalse(randomFilePath.Contains("before"));
            Assert.IsFalse(randomFilePath.Contains("after"));

            File.Create(randomFilePath)
                .Close();
        }

        [TestMethod]
        public void CutsNamePartAfterDate()
        {
            var a = new MessagePathAssembly(RootPath);
            Assert.IsTrue(a.AddFolder(ExtraLongDirName));

            a.NamePartBeforeDate("Before");

            a.NamePartAfterDate("Some");
            a.NamePartAfterDate("name");
            a.NamePartAfterDate("after");
            a.NamePartAfterDate("a date");

            var filePath = a.GenerateFilePath();
            File.Create(filePath)
                .Close();

            var randomFilePath = a.GenerateFilePath(true);

            Assert.IsTrue(randomFilePath.Contains("Before"));
            Assert.IsFalse(randomFilePath.Contains("name"));
            Assert.IsFalse(randomFilePath.Contains("after"));

            File.Create(randomFilePath)
                .Close();
        }

        [TestMethod]
        public void LimitsNameLength()
        {
            var a = new MessagePathAssembly(RootPath);
            Assert.IsTrue(a.AddFolder(ExtraLongDirName));

            a.NamePartBeforeDate("Some name before a date");
            a.NamePartAfterDate("Some name after a date");

            var randomFilePath = a.GenerateFilePath(true);

            File.Create(randomFilePath)
                .Close();

            Assert.AreEqual(randomFilePath.Length, MessagePathAssemblyBase.WindowsMaxPathFoldersFile);

            var exceededPath = randomFilePath + "x";
            Assert.ThrowsException<DirectoryNotFoundException>(() => File.Create(exceededPath).Dispose());
        }


    }
}
