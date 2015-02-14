using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoftwareMind.Shared.Dto;
using SoftwareMind.Shared.Infrastructure;

namespace SoftwareMind.Shared.Service
{
    public interface IAttachmentsImportService : IService
    {
        //Kopiuje podane pliki i przypina do podanego dokuemntu
        string CopyDocFilesToDocument(long documentId, long processId, long docDefId, long contextId, long contextSourceId, long[] docFilesIds);

        void CopyOneDocFileToNewProcess(long docFileId, long targetDocumentId);
        
        /// <summary>
        /// Znajduje dokumenty danej sprawy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="processId">Id sprawy</param>
        /// <returns></returns>
        IContainer GetProcessDocuments<T>(AccessStoreActionDTO<T> action, long processId);
    }
}
