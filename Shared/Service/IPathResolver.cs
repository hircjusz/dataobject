namespace SoftwareMind.Shared.Service
{
    public interface IPathResolver
    {
        string ResolveContentPath(string path = null, bool checkExistance = true);
        string ResolveResourcePath(string path = null, bool checkExistance = true);
        string ResolveTemporaryPath(string path = null, bool checkExistance = true);
        string ResolveSystemTemporaryPath(string path = null, bool checkExistance = true);
    }
}