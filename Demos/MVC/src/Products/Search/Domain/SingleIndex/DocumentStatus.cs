namespace GroupDocs.Total.MVC.Products.Search.Domain.SingleIndex
{
    internal enum DocumentStatus
    {
        Unknown,
        NotIndexed,
        Pending,
        Indexing,
        Removing,
        Merging,
        SuccessfullyProcessed,
        Skipped,
        ProcessedWithError,
    }
}
