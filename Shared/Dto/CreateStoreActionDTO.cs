namespace SoftwareMind.Shared.Dto
{
    public interface ICreateStoreActionDTO : IModifyStoreActionDTO
    {
    }

    public class CreateStoreActionDTO<TDto> : ModifyStoreActionDTO<TDto>, ICreateStoreActionDTO
    {
    }
}