using SoftwareMind.Shared.Dto;
using SoftwareMind.Shared.Infrastructure;

namespace SoftwareMind.Shared.Service
{
    public interface IDecisionLvlService : IService
    {
        IContainer GetDecisionLevelsInProcess<T>(AccessStoreActionDTO<T> action, long processId, long taskId);
    }
}
