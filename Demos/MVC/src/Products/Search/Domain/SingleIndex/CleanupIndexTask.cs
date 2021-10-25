using GroupDocs.Search.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Index = GroupDocs.Search.Index;

namespace GroupDocs.Total.MVC.Products.Search.Domain.SingleIndex
{
    internal class CleanupIndexTask : IndexTask
    {
        public CleanupIndexTask(
            ILogger logger,
            IndexFactoryService indexFactoryService,
            DocumentStatusService documentStatusService,
            Settings settings)
            : base(logger, indexFactoryService, settings, documentStatusService, string.Empty, Enumerable.Empty<string>())
        {
        }

        public override void Init()
        {
        }

        public override void BeforePreprocess()
        {
        }

        public override void AfterPreprocess()
        {
        }

        public override void Preprocess()
        {
        }

        public override void BeforeRun()
        {
        }

        public override void AfterRun()
        {
        }

        protected override void RunProtected(Index index)
        {
            UserFolder[] outdatedFolders;
            var activeFolders = GetActiveFolders(out outdatedFolders);
            var uploadedDirectoryName = Settings.UploadedDirectoryName;
            var keys = outdatedFolders
                .SelectMany(uf => uf.GetDocuments(uploadedDirectoryName))
                .Select(dd => dd.DocumentKey)
                .ToArray();
            for (int i = 0; i < outdatedFolders.Length; i++)
            {
                try
                {
                    Directory.Delete(outdatedFolders[i].FullPath, true);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Outdated user folder delete error.");
                }
            }
            var result = index.Delete(new UpdateOptions(), keys);

            DocumentStatusService.Reload();
        }

        private UserFolder[] GetActiveFolders(out UserFolder[] outdated)
        {
            var folders = Directory.GetDirectories(Settings.StoragePath, "*", SearchOption.TopDirectoryOnly);
            var activeList = new List<UserFolder>();
            var outdatedList = new List<UserFolder>();
            for (int i = 0; i < folders.Length; i++)
            {
                var fullPath = folders[i];
                UserFolder userFolder;
                if (UserFolder.TryCreate(fullPath, out userFolder))
                {
                    var uploadedDirectory = Path.Combine(userFolder.FullPath, Settings.UploadedDirectoryName);
                    Directory.CreateDirectory(uploadedDirectory);
                    var uploadedFiles = Directory.GetFiles(uploadedDirectory, "*.*", SearchOption.TopDirectoryOnly);
                    if (CheckIfActive(uploadedFiles))
                    {
                        activeList.Add(userFolder);
                    }
                    else
                    {
                        outdatedList.Add(userFolder);
                    }
                }
            }
            outdated = outdatedList.ToArray();
            return activeList.ToArray();
        }

        private bool CheckIfActive(string[] uploadedFiles)
        {
            foreach (var filePath in uploadedFiles)
            {
                var lastWriteTime = File.GetLastWriteTime(filePath);
                var removalTime = lastWriteTime + Settings.MinFolderLifetime;
                if (DateTime.Now <= removalTime)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
