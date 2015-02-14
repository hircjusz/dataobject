namespace SoftwareMind.Shared.Service
{
    public interface IProcessService : IService
    {
        void AssigneToProcessRole(long employeeId, long processId, string roleCode);
    }
}