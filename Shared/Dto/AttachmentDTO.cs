using System;
using System.Collections.Generic;

namespace SoftwareMind.Shared.Dto
{
    [Serializable]
    public class DocumentDTO
    {
        public long? DocumentId { get; set; }
        public long? DocDefId { get; set; }
        public long? ContextId { get; set; }
        public long? ContextSourceId { get; set; }
    }

    [Serializable]
    public class LocalFileDTO
    {
        public string Name { get; set; }

        // path to file on server; should be uploaded via silverlight earlier
        public string SourcePath { get; set; }
    }

    [Serializable]
    public class DocuwareFileDTO
    {
        public string Name { get; set; }

        public int DocuwareId { get; set; }
    }

    [Serializable]
    public class LexmarkFileDTO
    {
        public string Name { get; set; }

        public string FullName { get; set; }
    }

    [Serializable]
    public class LocalAttachmentDTO
    {
        public long ObjectId { get; set; }
        public IEnumerable<LocalFileDTO> Files { get; set; }
        public IEnumerable<DocumentDTO> Documents { get; set; }
    }

    [Serializable]
    public class DocuwareAttachmentDTO
    {
        public long ObjectId { get; set; }
        public IEnumerable<DocuwareFileDTO> Files { get; set; }
        public IEnumerable<DocumentDTO> Documents { get; set; }
    }

    [Serializable]
    public class LexmarkAttachmentDTO
    {
        public long ObjectId { get; set; }
        public IEnumerable<LexmarkFileDTO> Files { get; set; }
        public IEnumerable<DocumentDTO> Documents { get; set; }
    }
}
