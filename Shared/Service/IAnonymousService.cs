using DataObjects.NET;

namespace SoftwareMind.Shared.Service
{
    public interface IAnonymousService
    {
        Session Session { get; set; }
    }
}