namespace SoftwareMind.Shared.Dto
{
    public interface IUpdateStoreActionDTO : IModifyStoreActionDTO
    {
    }

    public class UpdateStoreActionDTO<TDto> : ModifyStoreActionDTO<TDto>, IUpdateStoreActionDTO
    {
    }
}