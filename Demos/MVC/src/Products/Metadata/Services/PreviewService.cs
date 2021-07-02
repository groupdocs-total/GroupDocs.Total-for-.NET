using GroupDocs.Metadata.Common;
using GroupDocs.Metadata.Options;
using GroupDocs.Total.MVC.Products.Common.Entity.Web;
using GroupDocs.Total.MVC.Products.Metadata.Config;
using GroupDocs.Total.MVC.Products.Metadata.DTO;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GroupDocs.Total.MVC.Products.Metadata.Services
{
    public class PreviewService
    {
        private readonly MetadataConfiguration metadataConfiguration;

        private readonly FileService fileService;

        public PreviewService(MetadataConfiguration metadataConfiguration, FileService fileService)
        {
            this.metadataConfiguration = metadataConfiguration;
            this.fileService = fileService;
        }

        public DocumentPreviewDto LoadDocument(PostedDataDto postedData)
        {
            string password = string.IsNullOrEmpty(postedData.password) ? null : postedData.password;
            var documentPreview = new DocumentPreviewDto();

            // set password for protected document
            var loadOptions = new LoadOptions
            {
                Password = password
            };


            bool completed = ExecuteWithTimeLimit(TimeSpan.FromMilliseconds(metadataConfiguration.GetPreviewTimeLimit()), cancelationToken =>
            {
                using (Stream fileStream = fileService.GetSourceFileStream(postedData.guid))
                using (GroupDocs.Metadata.Metadata metadata = new GroupDocs.Metadata.Metadata(fileStream, loadOptions))
                {
                    cancelationToken.ThrowIfCancellationRequested();
                    IReadOnlyList<PageInfo> pages = metadata.GetDocumentInfo().Pages;

                    using (MemoryStream stream = new MemoryStream())
                    {
                        PreviewOptions previewOptions = new PreviewOptions(pageNumber => stream, (pageNumber, pageStream) => { });
                        previewOptions.PreviewFormat = PreviewOptions.PreviewFormats.PNG;

                        int pageCount = pages.Count;
                        if (metadataConfiguration.GetPreloadPageCount() > 0)
                        {
                            pageCount = metadataConfiguration.GetPreloadPageCount();
                        }

                        for (int i = 0; i < pageCount; i++)
                        {
                            cancelationToken.ThrowIfCancellationRequested();
                            previewOptions.PageNumbers = new[] { i + 1 };
                            try
                            {
                                metadata.GeneratePreview(previewOptions);
                            }
                            catch (NotSupportedException)
                            {
                                break;
                            }

                            PageDescriptionEntity pageData = GetPageDescriptionEntities(pages[i]);
                            string encodedImage = Convert.ToBase64String(stream.ToArray());
                            pageData.SetData(encodedImage);
                            documentPreview.SetPages(pageData);
                            stream.SetLength(0);
                        }
                    }
                }
            });

            documentPreview.SetTimeLimitExceeded(!completed);
            documentPreview.SetGuid(postedData.guid);

            // return document description
            return documentPreview;

        }

        private bool ExecuteWithTimeLimit(TimeSpan timeSpan, Action<CancellationToken> codeBlock)
        {
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(timeSpan);
                try
                {
                    Task task = Task.Run(() => codeBlock(cancellationTokenSource.Token), cancellationTokenSource.Token);
                    task.Wait(cancellationTokenSource.Token);
                    return true;
                }
                catch (OperationCanceledException)
                {
                    return false;
                }
                catch (AggregateException e)
                {
                    throw e.InnerExceptions[0];
                }
            }
        }

        private PageDescriptionEntity GetPageDescriptionEntities(PageInfo page)
        {
            PageDescriptionEntity pageDescriptionEntity = new PageDescriptionEntity();
            pageDescriptionEntity.number = page.PageNumber;
            pageDescriptionEntity.height = page.Height;
            pageDescriptionEntity.width = page.Width;
            return pageDescriptionEntity;
        }
    }
}