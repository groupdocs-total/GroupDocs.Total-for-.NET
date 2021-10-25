using GroupDocs.Search.Options;
using System.IO;
using System.Linq;
using Index = GroupDocs.Search.Index;

namespace GroupDocs.Total.MVC.Products.Search.Domain.SingleIndex
{
    internal class DeleteIndexTask : IndexTask
    {
        private readonly DocumentDescriptor[] _descriptors;

        public DeleteIndexTask(
            ILogger logger,
            IndexFactoryService indexFactoryService,
            DocumentStatusService documentStatusService,
            Settings settings,
            string userId,
            string[] fileNames)
            : base(logger, indexFactoryService, settings, documentStatusService, userId, fileNames)
        {
            var userDirectory = Path.Combine(Settings.StoragePath, UserId);
            var uploadedDirectory = Path.Combine(userDirectory, Settings.UploadedDirectoryName);
            _descriptors = fileNames
                .Select(fileName =>
                {
                    var filePath = Path.Combine(uploadedDirectory, fileName);
                    var descriptor = new DocumentDescriptor(userId, fileName, filePath);
                    return descriptor;
                })
                .ToArray();
        }

        public override void Init()
        {
            SetStatus(DocumentStatus.Pending);
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
            SetStatus(DocumentStatus.Removing);
        }

        public override void AfterRun()
        {
            SetStatus(DocumentStatus.NotIndexed);
        }

        protected override void RunProtected(Index index)
        {
            var keys = _descriptors
                .Select(dd => dd.DocumentKey)
                .ToArray();
            index.Delete(new UpdateOptions(), keys);

            index.Optimize();
        }
    }
}
