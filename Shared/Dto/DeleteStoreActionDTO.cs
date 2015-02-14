namespace SoftwareMind.Shared.Dto
{
    public interface IDeleteStoreActionDTO : IModifyStoreActionDTO
    {
    }

    public class DeleteStoreActionDTO<TDto> : ModifyStoreActionDTO<TDto>, IDeleteStoreActionDTO
    {
    }
}