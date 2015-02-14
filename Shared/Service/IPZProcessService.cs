using SoftwareMind.Shared.Dto;
using SoftwareMind.Shared.Infrastructure;

namespace SoftwareMind.Shared.Service
{
    public interface IPZProcessService : IService
    {
        IContainer GetProcess(AccessStoreActionDTO<object> action, long processId);
        IContainer UpdateProcess(UpdateStoreActionDTO<object> action, long processId);
    }
}