using GroupDocs.Search.Dictionaries;

namespace GroupDocs.Total.MVC.Products.Search.Dto
{
    public class AlphabetCharacter
    {
        public int Character { get; set; }

        public int Type { get; set; }

        public override string ToString()
        {
            return Character + ":" + (CharacterType)Type + ":" + (char)Character;
        }
    }
}
