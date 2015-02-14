using System.Collections.Generic;
using System.Linq;
using SoftwareMind.Shared.Serialization;

namespace SoftwareMind.Shared.Dto
{
    public interface IReadStoreActionDTO : IAccessStoreActionDTO
    {
        int? Start { get; set; }
        int? Limit { get; set; }
        bool IsPaged { get; }
        bool LimitColumns { get; set; }
        ISet<string> Columns { get; set; }
        ICollection<SortDTO> Sort { get; set; }
    }

    public class ReadStoreActionDTO<TDto> : AccessStoreActionDTO<TDto>, IReadStoreActionDTO
    {
        public int? Start { get; set; }
        public int? Limit { get; set; }
        public bool IsPaged { get { return this.Start.HasValue && this.Limit.HasValue; } }
        public bool LimitColumns { get; set; }
        public ISet<string> Columns { get; set; }
        public ICollection<SortDTO> Sort { get; set; }

        internal void SetColumns(string columns)
        {
            if (columns == null)
            {
                this.Columns = null;
                return;
            }

            var array = (object[])JsonSerializerHelper.Deserialize<object>(columns);

            this.Columns = new HashSet<string>(
                array.Cast<string>()
                    .Union(new[] { "ID", "VersionID" })
                    .Distinct()
            );
        }

        internal void SetSort(string sort)
        {
            if (sort == null)
            {
                this.Sort = null;
                return;
            }

            var array = (object[])JsonSerializerHelper.Deserialize<object>(sort);
            var result = new List<SortDTO>();

            foreach (Dictionary<string, object> s in array)
            {
                string field = (string)s["field"];
                string render = (string)(s.ContainsKey("render") ? s["render"] : null);
                SortDirection direction = ((string)s["direction"]).ToLower() == "asc" ? SortDirection.Ascending : SortDirection.Descending;

                result.Add(new SortDTO
                {
                    Field = field,
                    Render = render,
                    Direction = direction
                });
            }

            this.Sort = result;
        }

        public override string ToString()
        {
            return string.Format(
                "{0}<{1}> (ColumnFilters: {2}; NamedFilters: {3}; Start: {4}; Limit: {5}; LimitColumns: {6}; Columns: {7}; Sort: {8})",
                this.GetType().GetGenericTypeDefinition().FullName,
                this.GetType().GetGenericArguments().Single().FullName,
                this.ColumnsFilters == null ? "(null)" : string.Join(", ", this.ColumnsFilters),
                this.NamedFilters == null ? "(null)" : string.Join(", ", this.NamedFilters.Select(f => string.Format("[{0}]", string.Join(", ", f)))),
                this.Start == null ? "(null)" : (object)this.Start,
                this.Limit == null ? "(null)" : (object)this.Limit,
                this.LimitColumns,
                this.Columns == null ? "(null)" : string.Join(", ", this.Columns),
                this.Sort == null ? "(null)" : string.Join(", ", this.Sort)
            );
        }
    }
}