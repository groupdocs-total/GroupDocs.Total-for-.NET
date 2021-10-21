namespace GroupDocs.Total.MVC.Products.Search.Dto.Request
{
    public class SearchPostedData : PostedDataEntity
    {
        public string Query { get; set; }

        public string SearchType { get; set; }

        public bool CaseSensitiveSearch { get; set; }

        public bool FuzzySearch { get; set; }

        public int FuzzySearchMistakeCount { get; set; }

        public bool FuzzySearchOnlyBestResults { get; set; }

        public bool KeyboardLayoutCorrection { get; set; }

        public bool SynonymSearch { get; set; }

        public bool HomophoneSearch { get; set; }

        public bool WordFormsSearch { get; set; }

        public bool SpellingCorrection { get; set; }

        public int SpellingCorrectionMistakeCount { get; set; }

        public bool SpellingCorrectionOnlyBestResults { get; set; }
    }
}
