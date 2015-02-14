using System;
using System.Collections.Generic;
using SoftwareMind.Shared.Dto;
using SoftwareMind.Shared.Infrastructure;

namespace SoftwareMind.Shared.Service
{
    public interface IDictionaryService : IService
    {
        IContainer GetDictionaryItems<T>(AccessStoreActionDTO<T> action, string dictionaryAlias, string usedAliases, string filteredAliases, string language, string parentCode = null);
        IContainer GetMathingDictionaryItems<T>(AccessStoreActionDTO<T> action, string dictionaryAlias, string usedAliases,string filteredAliases, string language, string businessRule, IDictionary<string, object> filters);
        IContainer GetMathingDictionaryItemsSpecifiedColumn<T>(AccessStoreActionDTO<T> action, string dictionaryAlias, string usedAliases, string filteredAliases, string language, string businessRule, IDictionary<string, object> filters, int columnIndex);
        IContainer GetMathingDictionaryItemsForCategories<T>(AccessStoreActionDTO<T> action, string dictionaryAlias, string usedAliases, string filteredAliases, string language, string businessRule, IDictionary<string, object> filters, Array codes);
    }
}
