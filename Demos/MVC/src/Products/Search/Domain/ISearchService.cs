using GroupDocs.Total.MVC.Products.Search.Dto;
using GroupDocs.Total.MVC.Products.Search.Dto.Info;
using GroupDocs.Total.MVC.Products.Search.Dto.Request;
using GroupDocs.Total.MVC.Products.Search.Dto.Response;
using System.IO;
using System.Threading.Tasks;

namespace GroupDocs.Total.MVC.Products.Search.Domain
{
    public interface ISearchService
    {
        Task AddFilesToIndexAsync(AddToIndexRequest request);

        void DeleteFiles(FilesDeleteRequest request);

        Task DownloadAndAddToIndexAsync(AddToIndexRequest request);

        SearchConfiguration GetConfiguration();

        IndexedFileDescriptionEntity[] GetFileStatus(FileStatusGetRequest request);

        GetStatusResponse GetStatus(SearchBaseRequest request);

        IndexedFileDescriptionEntity[] GetIndexedFiles(SearchBaseRequest request);

        IndexPropertiesResponse GetIndexProperties(SearchBaseRequest request);

        SearchAppInfo GetInfo(SearchBaseRequest request);

        Task<IndexedFileDescriptionEntity[]> GetUploadedFiles(SearchBaseRequest request);

        string Highlight(HighlightRequest request);

        MemoryStream GetSourceFile(HighlightRequest request, out string fileName);

        MemoryStream GetExtractedText(HighlightRequest request, out string fileName);

        PrepareDocumentResponse PrepareDocument(PrepareDocumentRequest request);

        GetDocumentPageResponse GetDocumentPage(GetDocumentPageRequest request);

        Stream GetResource(string containerName, string resourceName, out string contentType);

        void RemoveFileFromIndex(PostedDataEntity postedData);

        SummarySearchResult Search(SearchPostedData request);

        Task<UploadedDocumentEntity> UploadDocumentAsync(UploadDocumentContext context);

        MemoryStream GetAppInfo(string id);

        void RequestReindex(SearchBaseRequest request);

        AlphabetReadResponse GetAlphabetDictionary(SearchBaseRequest request);

        void SetAlphabetDictionary(AlphabetUpdateRequest request);

        StopWordsReadResponse GetStopWordDictionary(SearchBaseRequest request);

        void SetStopWordDictionary(StopWordsUpdateRequest request);

        SynonymsReadResponse GetSynonymGroups(SearchBaseRequest request);

        void SetSynonymGroups(SynonymsUpdateRequest request);

        HomophonesReadResponse GetHomophoneGroups(SearchBaseRequest request);

        void SetHomophoneGroups(HomophonesUpdateRequest request);

        SpellingCorrectorReadResponse GetSpellingCorrectorWords(SearchBaseRequest request);

        void SetSpellingCorrectorWords(SpellingCorrectorUpdateRequest request);
    }
}
