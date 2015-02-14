using System.Linq;

namespace SoftwareMind.Shared.Dto
{
    public interface ICountStoreActionDTO : IAccessStoreActionDTO
    {
    }

    public class CountStoreActionDTO<TDto> : AccessStoreActionDTO<TDto>, ICountStoreActionDTO
    {
        public override string ToString()
        {
            return string.Format(
                "{0}<{1}> (ColumnFilters: {2}; NamedFilters: {3})",
                this.GetType().GetGenericTypeDefinition().FullName,
                this.GetType().GetGenericArguments().Single().FullName,
                this.ColumnsFilters == null ? "(null)" : string.Join(", ", this.ColumnsFilters),
                this.NamedFilters == null ? "(null)" : string.Join(", ", this.NamedFilters.Select(f => string.Format("[{0}]", string.Join(", ", f))))
            );
        }
    }
}