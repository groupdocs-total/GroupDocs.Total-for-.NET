namespace GroupDocs.Total.MVC.Products.Search.Domain.SingleIndex
{
    internal class OcrImageCounter
    {
        private readonly int _maxOcrImageCount;
        private int _count;

        public OcrImageCounter(Settings settings)
        {
            _maxOcrImageCount = settings.MaxOcrImageCount;
        }

        public bool Increase()
        {
            if (_count >= _maxOcrImageCount)
            {
                return false;
            }

            _count++;
            return true;
        }

        public void Reset()
        {
            _count = 0;
        }
    }
}
