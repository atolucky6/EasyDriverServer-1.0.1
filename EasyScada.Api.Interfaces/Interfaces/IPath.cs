namespace EasyScada.Api.Interfaces
{
    public interface IPath
    {
        string Path { get; }
        T GetItem<T>(string pathToObject) where T : class, IPath;
    }
}
