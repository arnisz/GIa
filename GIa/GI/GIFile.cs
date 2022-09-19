namespace GI
{
    public class GiFile
    {
        private string _name;
        private string _content;

        public GiFile(string Name)
        {
            _name = Name;
        }

        public string Filename { get { return _name; } set { _name = value; } }
        public string Content { get { return _content; } set { _content = value; } }

        public long Size { get; set; }

        public override string ToString()
        {
            return _name + " " + Size;
        }

    }
}
