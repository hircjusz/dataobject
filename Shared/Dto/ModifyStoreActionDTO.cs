using System;
using System.Collections.Generic;
using System.Linq;
using SoftwareMind.Shared.Serialization;

namespace SoftwareMind.Shared.Dto
{
    public interface IModifyStoreActionDTO : IStoreActionDTO
    {
        bool ValidateOnly { get; set; }

        IDictionary<string, object>[] Data { get; set; }
    }

    public abstract class ModifyStoreActionDTO<TDto> : IStoreActionDTO<TDto>, IModifyStoreActionDTO
    {
        public Type Entity { get; set; }

        public bool ValidateOnly { get; set; }

        public IDictionary<string, object>[] Data { get; set; }

        internal void SetData(string data)
        {
            if (data == null)
            {
                this.Data = null;
                return;
            }

            this.Data = JsonSerializerHelper.Deserialize<IDictionary<string, object>[]>(data);
        }

        public override string ToString()
        {
            return string.Format(
                "{0}<{1}> (Data: {2})",
                this.GetType().GetGenericTypeDefinition().FullName,
                this.GetType().GetGenericArguments().Single().FullName,
                this.Data
            );
        }
    }
}