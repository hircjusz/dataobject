using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using DataObjects.NET;
using SoftwareMind.Shared.Serialization;

namespace SoftwareMind.Shared.Dto
{
    public interface IAccessStoreActionDTO : IStoreActionDTO
    {
        ICollection<ColumnFilterDTO> ColumnsFilters { get; }

        ICollection<string[]> NamedFilters { get; }

        void SetNamedFilters(string filters);

        void SetColumnFilters(string filters);
    }

    public abstract class AccessStoreActionDTO<TDto> : IStoreActionDTO<TDto>, IAccessStoreActionDTO
    {
        public Type Entity { get; set; }

        public ICollection<ColumnFilterDTO> ColumnsFilters { get; private set; }

        public ICollection<string[]> NamedFilters { get; private set; }

        public void SetNamedFilters(string filters)
        {
            if (filters == null)
            {
                this.NamedFilters = null;
                return;
            }

            var array = JsonSerializerHelper.Deserialize<object[]>(filters);
            var result = new List<string[]>(
                array.Select(o => o is IEnumerable<object> ? ((IEnumerable<object>)o).Select(p => (string)p).ToArray() : new[] { (string)o })
            );

            this.NamedFilters = result;
        }

        public void SetColumnFilters(string filters)
        {
            if (filters == null)
            {
                this.ColumnsFilters = null;
                return;
            }

            var array = (object[])JsonSerializerHelper.Deserialize<object>(filters);
            var fields = typeof(TDto) == typeof(object) ? null : typeof (TDto).GetProperties();
            var result = new List<ColumnFilterDTO>();

            foreach (Dictionary<string, object> f in array)
            {
                ColumnFilterDTO.FilterComparison comparison = ColumnFilterDTO.FilterComparison.None;
                string comparisonStr = f.ContainsKey("comparison") ? (string)f["comparison"] : null;
                string field = (string)f["field"];
                string render = (string)(f.ContainsKey("render") ? f["render"] : null);
                string type = (string)f["type"];

                if (fields != null && fields.Count(p => p.Name == field) == 0)
                {
                    throw new InvalidOperationException(string.Format("Cannot find {0} property in {1} dto", field, typeof(TDto).FullName));
                }
                if (comparisonStr != null && !Enum.TryParse(comparisonStr, true, out comparison))
                {
                    throw new KeyNotFoundException(string.Format("Value {0} were not found in FilterComparison enum", comparisonStr));
                }

                switch (type)
                {
                    case "boolean":
                        {
                            bool value = Convert.ToBoolean(f["value"]);

                            result.Add(new BooleanColumnFilterDTO
                            {
                                Field = field,
                                Render = render,
                                Value = value
                            });
                        }
                        break;

                    case "date":
                        {
                            string[] formats = { "dd/MM/yyyy", "yyyy-MM-dd HH:mm:ss", "dd/MM/yyyy HH:mm:ss", "yyyyMMdd HH:mm:ss", "dd-mm-yyyy" };
                            DateTime formattedDate = DateTime.ParseExact(Convert.ToString(f["value"]), formats, Thread.CurrentThread.CurrentCulture, DateTimeStyles.None);
                            
                            result.Add(new DateColumnFilterDTO
                            {
                                Comparison = comparison,
                                Field = field,
                                Render = render,
                                Value = formattedDate
                            });
                        }
                        break;

                    case "numeric":
                        {
                            decimal value = Convert.ToDecimal(f["value"]);

                            result.Add(new NumericColumnFilterDTO
                            {
                                Comparison = comparison,
                                Field = field,
                                Render = render,
                                Value = value
                            });
                        }
                        break;

                    case "string":
                        {
                            string value = Convert.ToString(f["value"]);

                            result.Add(new StringColumnFilterDTO
                            {
                                Comparison = comparison,
                                Field = field,
                                Render = render,
                                Value = value
                            });
                        }
                        break;

                    default:
                        throw new NotImplementedException(); // Jeœli zajdzie potrzeba to trzeba doimplementowac kolejne typy
                }
            }

            this.ColumnsFilters = result;
        }
    }
}