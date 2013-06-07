namespace Bank
{

    public interface IFileWriter
    {
        void Write(string file, string contents);
        string Read(string file);
    }

}
