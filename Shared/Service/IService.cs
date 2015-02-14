using SoftwareMind.Shared.Infrastructure;

namespace SoftwareMind.Shared.Service
{
    public interface IService
    {
        IContext Context { get; }
    }
}