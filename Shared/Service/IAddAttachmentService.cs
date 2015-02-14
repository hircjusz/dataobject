using SoftwareMind.Shared.Dto;
using SoftwareMind.Shared.Infrastructure;

namespace SoftwareMind.Shared.Service
{
    public interface IAddAttachmentService : IService
    {
        void AddLocalFile(LocalAttachmentDTO attachment);
        void AddDocuwareFiles(DocuwareAttachmentDTO attachment);
        void AddLexmarkFiles(LexmarkAttachmentDTO attachment);

        IContainer GetCustomers<T>(AccessStoreActionDTO<T> action, long processId);
    }
}
