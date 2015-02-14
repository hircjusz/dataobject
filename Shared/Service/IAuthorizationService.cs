namespace SoftwareMind.Shared.Service
{
    public interface IAuthorizationService : IService
    {
        bool HasCustomerRights(long customerId);
        bool HasPageRights(string pageId);
        bool HasProcessRights(long processId);
        bool HasProcTaskRights(long procTaskId);
        bool HasTaskRights(long taskId);
    }
}