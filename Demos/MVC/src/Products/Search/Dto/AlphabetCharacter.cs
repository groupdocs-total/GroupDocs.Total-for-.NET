using GroupDocs.Search.Dictionaries;

namespace GroupDocs.Total.MVC.Products.Search.Dto
{
    public class AlphabetCharacter
    {
        public int character { get; set; }

        public int type { get; set; }

        public override string ToString()
        {
            return character + ":" + (CharacterType)type + ":" + (char)character;
        }
    }
}
