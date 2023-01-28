namespace Dialogue.Models
{
    public class Choice<TIdentifier>
    {
        public TIdentifier Id { get; set; }
        public string Text { get; set; }
    }
}
