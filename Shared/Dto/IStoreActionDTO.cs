using System;

namespace SoftwareMind.Shared.Dto
{
    public interface IStoreActionDTO
    {
        Type Entity { get; set; }
    }

    public interface IStoreActionDTO<TDto> : IStoreActionDTO
    {
    }
}
