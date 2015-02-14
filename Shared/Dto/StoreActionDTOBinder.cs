using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using DataObjects.NET;
using SoftwareMind.Shared.Serialization;

namespace SoftwareMind.Shared.Dto
{
    public class StoreActionDTOBinder : IModelBinder
    {
        enum ExtStoreActionType
        {
            Read,
            Create,
            Destroy,
            Update,
            Count
        }

        private static IDictionary<string, MethodInfo> methods;

        static StoreActionDTOBinder()
        {
            methods = typeof(StoreActionDTOBinder)
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(m => m.IsGenericMethod)
                .Where(m => {
                    var p = m.GetParameters();
                    return p.Length == 1 && p[0].ParameterType == typeof(IDictionary<string, object>);
                })
                .ToDictionary(m => m.Name);

        }

        #region Implementation of IModelBinder

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            IDictionary<string, object> values = this.ExtractValues(bindingContext);

            ExtStoreActionType actionType = (ExtStoreActionType)values["actionType"];
            switch (actionType)
            {
                case ExtStoreActionType.Count:
                    return this.CreateCountStoreAction(values);
                case ExtStoreActionType.Create:
                    return this.CreateCreateStoreAction(values);
                case ExtStoreActionType.Read:
                    return this.CreateReadStoreAction(values);
                case ExtStoreActionType.Destroy:
                    return this.CreateDeleteStoreAction(values);
                case ExtStoreActionType.Update:
                    return this.CreateUpdateStoreAction(values);
            }

            throw new InvalidOperationException();
        }

        #region Extracting values

        private IDictionary<string, object> ExtractValues(ModelBindingContext bindingContext)
        {
            try
            {
                IDictionary<string, object> result = new Dictionary<string, object>();
                IValueProvider provider = bindingContext.ValueProvider;
                string prefix = this.ExtractPrefix(bindingContext);

                if (prefix != null)
                {
                    provider = this.ExtractProvider(provider, prefix);
                }

                result["actionType"] = this.ExtractActionType(provider);
                result["dto"] = this.ExtractDto(provider);
                result["entity"] = this.ExtractEntity(provider);

                // Optional parameters
                result["columnFilters"] = this.ExtractGeneric<string>(provider, "columnFilters", true);
                result["columns"] = this.ExtractGeneric<string>(provider, "columns", true);
                result["data"] = this.ExtractGeneric<string>(provider, "List", true, false);
                result["namedFilters"] = this.ExtractGeneric<string>(provider, "namedFilters", true);
                result["sort"] = this.ExtractGeneric<string>(provider, "sort", true);
                result["limit"] = this.ExtractNullable<int>(provider, "limit", true);
                result["limitColumns"] = this.ExtractNullable<bool>(provider, "limitColumns", true) ?? false;
                result["start"] = this.ExtractNullable<int>(provider, "start", true);
                result["validate"] = this.ExtractNullable<bool>(provider, "validate", true);

                return result;
            }
            catch (ArgumentException ex)
            {
                throw new HttpException(403, "Nieprawidłowe żądanie", ex);
            }
        }

        private string ExtractPrefix(ModelBindingContext bindingContext)
        {
            IValueProvider provider = bindingContext.ValueProvider;
            var action = provider.GetValue("xaction");
            var model = provider.GetValue(bindingContext.ModelName);

            if (action == null && model != null)
            {
                return bindingContext.ModelName;
            }

            return null;
        }

        private IValueProvider ExtractProvider(IValueProvider provider, string prefix)
        {
            var value = provider.GetValue(prefix);
            if (value == null)
            {
                throw new ArgumentException(prefix + " is not set", prefix);
            }

            IDictionary<string, object> data = JsonSerializerHelper.Deserialize<IDictionary<string, object>>((string)value.ConvertTo(typeof(string)));
            return new DictionaryValueProvider<object>(data, CultureInfo.InvariantCulture);
        }

        private ExtStoreActionType ExtractActionType(IValueProvider provider)
        {
            var action = provider.GetValue("xaction");
            if (action == null)
            {
                throw new ArgumentException("xaction is not set", "xaction");
            }

            ExtStoreActionType actionType = ExtStoreActionType.Read;
            if (!Enum.TryParse((string)action.ConvertTo(typeof(string)), true, out actionType))
            {
                throw new ArgumentException("xaction has invalid value", "xaction");
            }

            var count = provider.GetValue("count");
            if (count != null && (bool)count.ConvertTo(typeof(bool)))
            {
                actionType = ExtStoreActionType.Count;
            }

            return actionType;
        }

        private Type ExtractDto(IValueProvider provider)
        {
            var dto = provider.GetValue("dto");
            if (dto == null)
            {
                throw new ArgumentException("Dto is not set", "dto");
            }

            Type dtoType = Type.GetType((string)dto.ConvertTo(typeof(string)));
            return dtoType;
        }

        private Type ExtractEntity(IValueProvider provider)
        {
            var entity = provider.GetValue("entity");
            if (entity == null)
            {
                throw new ArgumentException("Entity is not set", "entity");
            }

            string entityName = (string)entity.ConvertTo(typeof(string));
            Type entityType = Type.GetType(entityName);

            if (entityType == null)
            {
                Func<Type, bool> predicate = entityName.Contains(".") ? (Func<Type, bool>)(t => t.FullName == entityName) : (Func<Type, bool>)(t => t.Name == entityName);
                entityType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .SingleOrDefault(t => predicate(t) && (typeof(DataObject).IsAssignableFrom(t) || typeof(IDataObject).IsAssignableFrom(t)));
            }

            if (entityType == null)
            {
                throw new ArgumentException("Cannot resolve entity type", "entity");
            }

            return entityType;
        }

        private T ExtractGeneric<T>(IValueProvider provider, string key, bool optional, bool validate = true) where T : class
        {
            var value = validate ?
                provider.GetValue(key) :
                (provider as IUnvalidatedValueProvider).GetValue(key, true);

            if (!optional && value == null)
            {
                throw new ArgumentException(key + " is not set", key);
            }

            return value == null ? null : (T)value.ConvertTo(typeof(T));
        }

        private T? ExtractNullable<T>(IValueProvider provider, string key, bool optional) where T : struct
        {
            var value = provider.GetValue(key);

            if (!optional && value == null)
            {
                throw new ArgumentException(key + " is not set", key);
            }

            return value == null || string.IsNullOrEmpty(value.AttemptedValue) ? null : new T?((T)value.ConvertTo(typeof(T)));
        }

        #endregion

        #endregion

        private object InvokeMethodByName(string methodName, IDictionary<string, object> values)
        {
            try
            {
                Type dtoType = (Type)values["dto"] ?? typeof(object);
                return methods[methodName].MakeGenericMethod(dtoType).Invoke(this, new object[] { values });
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }
                throw;
            }
        }

        private object CreateCountStoreAction(IDictionary<string, object> values)
        {
            return this.InvokeMethodByName("CreateCountStoreAction", values);
        }

        private object CreateCreateStoreAction(IDictionary<string, object> values)
        {
            return this.InvokeMethodByName("CreateCreateStoreAction", values);
        }

        private object CreateReadStoreAction(IDictionary<string, object> values)
        {
            return this.InvokeMethodByName("CreateReadStoreAction", values);
        }

        private object CreateDeleteStoreAction(IDictionary<string, object> values)
        {
            return this.InvokeMethodByName("CreateDeleteStoreAction", values);
        }

        private object CreateUpdateStoreAction(IDictionary<string, object> values)
        {
            return this.InvokeMethodByName("CreateUpdateStoreAction", values);
        }

        private CountStoreActionDTO<TDto> CreateCountStoreAction<TDto>(IDictionary<string, object> values)
        {
            var action = new CountStoreActionDTO<TDto>();
            action.SetColumnFilters((string)values["columnFilters"]);
            action.SetNamedFilters((string)values["namedFilters"]);

            action.Entity = (Type)values["entity"];

            return action;
        }

        private CreateStoreActionDTO<TDto> CreateCreateStoreAction<TDto>(IDictionary<string, object> values)
        {
            var action = new CreateStoreActionDTO<TDto>();
            action.SetData((string)values["data"]);
            action.ValidateOnly = values["validate"] != null && (bool)values["validate"];

            action.Entity = (Type)values["entity"];

            return action;
        }

        private DeleteStoreActionDTO<TDto> CreateDeleteStoreAction<TDto>(IDictionary<string, object> values)
        {
            var action = new DeleteStoreActionDTO<TDto>();
            action.SetData((string)values["data"]);

            action.Entity = (Type)values["entity"];

            return action;
        }

        private ReadStoreActionDTO<TDto> CreateReadStoreAction<TDto>(IDictionary<string, object> values)
        {
            var action = new ReadStoreActionDTO<TDto>();
            action.SetColumnFilters((string)values["columnFilters"]);
            action.SetNamedFilters((string)values["namedFilters"]);
            action.SetColumns((string)values["columns"]);
            action.SetSort((string)values["sort"]);

            action.Entity = (Type)values["entity"];
            action.Start = (int?)values["start"];
            action.Limit = (int?)values["limit"];
            action.LimitColumns = (bool)values["limitColumns"];

            return action;
        }

        private UpdateStoreActionDTO<TDto> CreateUpdateStoreAction<TDto>(IDictionary<string, object> values)
        {
            var action = new UpdateStoreActionDTO<TDto>();
            action.SetData((string)values["data"]);
            action.ValidateOnly = values["validate"] != null && (bool)values["validate"];

            action.Entity = (Type)values["entity"];

            return action;
        }

    }
}
