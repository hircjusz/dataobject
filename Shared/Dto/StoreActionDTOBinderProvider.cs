using System;
using System.Web.Mvc;

namespace SoftwareMind.Shared.Dto
{
    public class StoreActionDTOBinderProvider : IModelBinderProvider
    {
        private static readonly IModelBinder StoreActionDTOBinder = new StoreActionDTOBinder();

        public IModelBinder GetBinder(Type modelType)
        {
            if (!typeof(IStoreActionDTO).IsAssignableFrom(modelType))
            {
                return null;
            }

            /*var interfaces = modelType.GetInterfaces();
            if (interfaces.Count(i => i.IsGenericType && i.GetGenericTypeDefinition().FullName == typeof(IStoreActionDTO<>).FullName) == 0)
            {
                return null;
            }*/

            return StoreActionDTOBinder;
        }
    }
}
