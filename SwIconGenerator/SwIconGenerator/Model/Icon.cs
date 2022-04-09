using System.IO;

namespace SwIconGenerator
{
    public class Icon
    {
        public Icon(string file)
        {
            FilePathName = file;
            Name=Path.GetFileName(file);
        }

        public string Name { get; set; }

        public string FilePathName { get; set; }
    }
}
