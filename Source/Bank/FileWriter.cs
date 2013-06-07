
namespace Bank
{
    public class FileWriter : IFileWriter
    {
        public void Write(string file, string contents)
        {
            System.IO.File.WriteAllText(file, contents);
        }
        public string Read(string file)
        {
            return System.IO.File.ReadAllText(file);
        }
    }
}
