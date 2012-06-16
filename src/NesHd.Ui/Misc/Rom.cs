namespace NesHd.Ui.Misc
{
    public class Rom
    {
        private string _name = "";
        private string _path = "";

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
    }
}