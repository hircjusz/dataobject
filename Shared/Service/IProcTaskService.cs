namespace SoftwareMind.Shared.Service
{
    public interface IProcTaskService : IService
    {
        long GetProcessId(long procTaskId);
        void GetProcessModelForApplication(long appId,out long processId,out int model);
    }
}