using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DataObjects.NET;
using DataObjects.NET.Attributes;
using log4net;
using SoftwareMind.SimpleWorkflow.Attributes;

namespace ConsoleDataObjects.ObjectsDomain
{
    [Abstract]
    [ShareAncestorTable]
    //[WFCustomSerialization(typeof(DataObjectSimpleWorkflowSerializer))]
    public abstract class BusinessObject : DataObject
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BusinessObject));

        public const string Property_ID = "ID";
        public const string Property_VersionID = "VersionID";
        public const string Property_TypeID = "TypeID";
        
        //private IBusinessLogicDelegate logicDelegate;
        ////        private bool isInCreate = false;

        ////[NonSerialized]
        ////private object oldValue = null;

        //[NotPersistent]
        //[SQLExpression(":p_EntityPermissions_All")]
        //public virtual EntityPermissions EntityPermissions { get { return EntityPermissions.All; } }

        //[DataObjects.NET.Attributes.Transactional(TransactionMode.Disabled)]
        //protected virtual string GetDelegateKey()
        //{
        //    return null;
        //}

        //[DataObjects.NET.Attributes.Transactional(TransactionMode.Disabled)]
        //protected virtual IBusinessLogicDelegate GetLogicDelegate()
        //{
        //    if (logicDelegate != null)
        //        return logicDelegate;
        //    string key = GetDelegateKey();
        //    if (key != null)
        //    {
        //        logicDelegate = ServiceLocator.Current.GetInstance<IBusinessLogicDelegate>(
        //            new KeyValuePair<string, object>("Type", this.Type.SourceType),
        //            new KeyValuePair<string, object>("Alias", key)
        //        );
        //        return logicDelegate;
        //    }
        //    return null;
        //}

        //public virtual void OnValidateProperty(string propertyName, object value)
        //{
        //}

        //protected static Statement FindAllStatement(Type type)
        //{
        //    return new SQLStatement("\"root\"." + SQLStatement.ALL_COLUMNS, MetaInfo.Instance.TypesByType[type].RelatedView.FullQuotedName + " \"root\"", "", "");
        //}

        //protected override void OnCreate()
        //{
        //    //            isInCreate = true;
        //    base.OnCreate();
        //    if (GetLogicDelegate() != null)
        //        GetLogicDelegate().OnCreate(this);
        //}

        //protected override void OnCreated()
        //{
        //    if (GetLogicDelegate() != null)
        //        GetLogicDelegate().OnCreated(this);
        //    OnValidateChanges();
        //    Journal(JournalKind.Create, null, null, null, null, false);
        //    PersistJournal();
        //    base.OnCreated();
        //    //isInCreate = false;
        //}

        //protected virtual void OnCreate(IBusinessObjectModel model)
        //{
        //    OnCreate();

        //    foreach (string propertyName in model.PropertyNames)
        //    {
        //        object newValue = model.GetProperty(propertyName);
        //        this.OnValidateProperty(propertyName, newValue);
        //        this.SetProperty(propertyName, newValue);
        //    }
        //    if (GetLogicDelegate() != null)
        //        GetLogicDelegate().OnCreate(this, model);
        //}

        public virtual void OnValidateChanges()
        {
           
        }

        public virtual void NotifyChange(IEnumerable<object> newValues)
        {
        }

        //protected override void OnPropertyChanged(string name, Culture culture, object value)
        //{
        //    //if (this is IFlowControl)
        //    //    (this as IFlowControl).GetFlow().OnPropertyChanged(this, name, value);
        //    if (GetLogicDelegate() != null)
        //        GetLogicDelegate().OnPropertyChanged(this, name, culture, value);
        //    if (/*!isInCreate &&*/ !this.IsCreating && !IsRemoving)
        //    {
        //        if (CanJournal(JournalKind.Modify, name))
        //        {
        //            string journalName = GetJournalPropertyName(name);
        //            if (!String.IsNullOrEmpty(journalName))
        //            {
        //                var curEmpl = Employee.GetLoggedEmployee(Session);
        //                Journal(JournalKind.Modify, GetJournalPropertyInfo(name, value), GetJournalPropertyCategory(name), journalName, name, false, curEmpl);
        //            }
        //        }
        //    }

        //    base.OnPropertyChanged(name, culture, value);
        //}


        protected virtual string GetJournalPropertyInfo(string propertyName, object value)
        {
            return null;
        }

        protected virtual string GetJournalPropertyCategory(string propertyName)
        {
            return null;
        }

        public virtual string GetJournalPropertyName(string propertyName)
        {
            //return propertyName;
            return null;
        }

        //protected virtual string PropertyValueToString(string propertyName, object value, Type type)
        //{
        //    var pi = type.GetProperty(propertyName);
        //    var attr = (DictionaryAttribute[])pi.GetCustomAttributes(typeof(DictionaryAttribute), false);
        //    if (attr.Length > 0 && pi.PropertyType == typeof(string))
        //    {
        //        return DiCache.Instance.GetDiNameByAlias(attr[0].Dictionary, value.ToString());
        //    }
        //    return PropertyValueToString(propertyName, value);
        //}

        //protected virtual string PropertyValueToString(string propertyName, object value)
        //{
        //    if (value != null)
        //    {
        //        if (value is BusinessObject)
        //        {
        //            BusinessObject obj = value as BusinessObject;
        //            object val = obj.GetDefaultPropertyValue();
        //            if (val != null)
        //                value = val.ToString();
        //        }
        //        else
        //            if (value is DateTime)
        //            {
        //                return ((DateTime)value).ToString("dd-MM-yyyy");
        //            }
        //            else
        //                if (value is Decimal)
        //                {
        //                    return ((Decimal)value).ToString().Replace('.', ',');
        //                }
        //                else
        //                    if (value is Boolean)
        //                    {
        //                        return (Boolean)value ? "Tak" : "Nie";
        //                    }

        //        return value.ToString();
        //    }
        //    return null;
        //}


        //protected override void OnSetProperty(string name, Culture culture, object value)
        //{
        //    if (GetLogicDelegate() != null)
        //        GetLogicDelegate().OnSetProperty(this, name, culture, value);
        //    base.OnSetProperty(name, culture, value);
        //}

        //protected override void OnReference(DataObject referer, string propertyName, Culture culture)
        //{
        //    if (GetLogicDelegate() != null)
        //        GetLogicDelegate().OnReference(this, referer, propertyName, culture);
        //    base.OnReference(referer, propertyName, culture);
        //}

        //protected override void OnDereference(DataObject referer, string propertyName, Culture culture)
        //{
        //    if (GetLogicDelegate() != null)
        //        GetLogicDelegate().OnDereference(this, referer, propertyName, culture);
        //    base.OnDereference(referer, propertyName, culture);
        //}

        public virtual long GetPropertyID(string propertyName)
        {
            long[] IDs = this.GetIDs(propertyName);
            if (IDs != null && IDs.Length == 1)
            {
                return IDs[0];
            }
            else
                return -1;
        }

        //public virtual Dictionary<string, object> Row()
        //{
        //    Dictionary<string, object> result = new Dictionary<string, object>();
        //    foreach (DataObjects.NET.ObjectModel.Field fld in this.Type.Fields)
        //    {
        //        if (!fld.LoadOnDemand && fld.Index != 4 && fld.Index != 3)
        //        {
        //            object val = null;

        //            if (typeof(BusinessObject).IsAssignableFrom(fld.SourceType) || typeof(IDataObject).IsAssignableFrom(fld.SourceType))
        //            {
        //                long[] IDs = this.GetIDs(fld.Name);

        //                if (IDs != null)
        //                {
        //                    if (((ulong)IDs[0] & 0xFFE0000000000000L) != 0)
        //                    {
        //                        log.FatalFormat("ID {0} overflows double precission in JS!!!", IDs[0]);
        //                        throw new OverflowException(string.Format("ID {0} overflows double precission in JS!!!", IDs[0]));
        //                    }

        //                    val = IDs[0];
        //                }
        //            }
        //            else
        //            {
        //                val = this.GetProperty(fld.Name);
        //            }

        //            if (val is BusinessObject)
        //            {
        //                if (((ulong)((BusinessObject)val).ID & 0xFFE0000000000000L) != 0)
        //                {
        //                    log.FatalFormat("ID {0} overflows double precission in JS!!!", ((BusinessObject)val).ID);
        //                    throw new OverflowException(string.Format("ID {0} overflows double precission in JS!!!", ((BusinessObject)val).ID));
        //                }

        //                result.Add(fld.Name, (val as BusinessObject).ID);
        //            }
        //            else
        //            {
        //                result.Add(fld.Name, ValueFromStorage(val));
        //            }
        //        }
        //    }

        //    return result;
        //}

        //protected static Statement PrepareDefaultSQLStatement(Type t)
        //{
        //    return new BusinessSQLStatement(t);
        //}

        public virtual object GetCalculatedPropertyValue(string propertyName)
        {
            foreach (System.Reflection.MemberInfo fld in this.GetType().GetMembers(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                if ((fld.MemberType & System.Reflection.MemberTypes.Property) == System.Reflection.MemberTypes.Property)
                {
                    System.Reflection.PropertyInfo property = (System.Reflection.PropertyInfo)fld;

                    if (property.CanRead && property.Name == propertyName)
                    {
                        return property.GetValue(this, null);
                    }
                }
            }

            log.DebugFormat("Brak właściwości o nazwie: {0}", propertyName);
            return propertyName;
        }

        //public virtual bool HasEntityPermission(EntityPermissions permission)
        //{
        //    return (this.EntityPermissions & permission) != 0;
        //}

        //public virtual bool HasProperty(string propertyName)
        //{
        //    foreach (DataObjects.NET.ObjectModel.Field fld in this.Type.Fields)
        //    {
        //        if (fld.Name == propertyName)
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        public virtual bool HasDefinedProperty(string propertyName)
        {
            PropertyInfo pi = this.Type.SourceType.GetProperty(propertyName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

            if (pi != null)
            {
                return true;
            }

            return false;
        }

        public virtual bool SetDefinedProperty(string propertyName, object value)
        {
            PropertyInfo pi = this.Type.SourceType.GetProperty(propertyName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

            if (pi != null)
            {
                pi.SetValue(this, value == null ? null : Convert.ChangeType(value, pi.PropertyType), null);
            }

            return false;
        }

        private readonly string argumentExceptionMessage = @"Podane property nie istnieje lub nie jest to property DO.NET";


        //protected virtual void SetNestedProperty(string propertyName, object value)
        //{
        //    if (propertyName.IndexOf('.') == -1)
        //    {
        //        if (HasProperty(propertyName))
        //        {
        //            SetProperty(propertyName, value);
        //        }

        //        throw new ArgumentException(argumentExceptionMessage);
        //    }
        //    else
        //    {
        //        string[] propertyNames = propertyName.Split('.');
        //        BusinessObject target = this;
        //        object result = null;

        //        for (int propertyIndex = 0; propertyIndex < propertyNames.Length; propertyIndex++)
        //        {
        //            string name = propertyNames[propertyIndex];

        //            if (target.HasProperty(name))
        //            {
        //                result = target.GetProperty(name);
        //            }
        //            else if (target.HasDefinedProperty(name))
        //            {
        //                target.SetDefinedProperty(name, value);
        //                return;
        //            }
        //            else
        //            {
        //                throw new ArgumentException(argumentExceptionMessage);
        //            }

        //            if (result == null)
        //            {
        //                if (propertyIndex == propertyNames.Length - 1)
        //                {
        //                    target.SetProperty(name, value);
        //                    return;
        //                }

        //                throw new ArgumentException(argumentExceptionMessage);
        //            }
        //            else
        //            {
        //                if (result is BusinessObject)
        //                {
        //                    target = (BusinessObject)result;
        //                }
        //                else
        //                {
        //                    if (propertyIndex == propertyNames.Length - 1)
        //                        target.SetProperty(name, value);
        //                    else
        //                        throw new ArgumentException(argumentExceptionMessage);
        //                }
        //            }
        //        }

        //        //target.SetProperty(propertyName, )
        //    }
        //}

        ////Process.OwnerUnit
        //protected virtual bool HasNestedProperty(string propertyName)
        //{
        //    if (propertyName.IndexOf('.') == -1)
        //    {
        //        return HasProperty(propertyName);
        //    }
        //    else
        //    {
        //        string[] propertyNames = propertyName.Split('.');
        //        BusinessObject target = this;
        //        object result = null;

        //        for (int propertyIndex = 0; propertyIndex < propertyNames.Length; propertyIndex++)
        //        {
        //            string name = propertyNames[propertyIndex];

        //            if (target.HasProperty(name))
        //            {
        //                result = target.GetProperty(name);
        //            }
        //            else if (target.HasDefinedProperty(name) && propertyIndex == propertyNames.Length - 1)
        //            {
        //                return true;
        //            }
        //            else
        //            {
        //                return false;
        //            }

        //            if (result == null)
        //            {
        //                if (propertyIndex != propertyNames.Length - 1)
        //                    return false;

        //                break;
        //            }
        //            else
        //            {
        //                if (result is BusinessObject)
        //                {
        //                    target = (BusinessObject)result;
        //                }
        //                else
        //                {
        //                    if (propertyIndex == propertyNames.Length - 1)
        //                        return true;

        //                    throw new ArgumentException(argumentExceptionMessage);
        //                }
        //            }

        //        }

        //        return true;
        //    }
        //}

        //public virtual object GetDefaultPropertyValue()
        //{
        //    DefaultProperty defaultProperty = MetaInfo.Instance.DefaultProperty(this.Type.SourceType);
        //    if (defaultProperty != null)
        //    {
        //        return this.GetProperty(defaultProperty.PropertyName);
        //    }
        //    return null;
        //}


        //public virtual object GetPropertyValue(string propertyName)
        //{
        //    if (propertyName.IndexOf('.') == -1)
        //    {
        //        if (HasProperty(propertyName))
        //        {
        //            return GetProperty(propertyName);
        //        }

        //        return GetCalculatedPropertyValue(propertyName);
        //    }
        //    else
        //    {
        //        string[] propertyNames = propertyName.Split('.');
        //        BusinessObject target = this;
        //        object result = null;

        //        foreach (string name in propertyNames)
        //        {
        //            if (target.HasProperty(name))
        //            {
        //                result = target.GetProperty(name);
        //            }
        //            else
        //            {
        //                result = target.GetCalculatedPropertyValue(name);
        //            }

        //            if (result == null)
        //            {
        //                break;
        //            }
        //            else
        //            {
        //                if (result is BusinessObject)
        //                {
        //                    target = result as BusinessObject;
        //                }
        //                else
        //                {
        //                    break;
        //                }
        //            }
        //        }

        //        return result;
        //    }
        //}

        //private Query PrepareQuery(string query, IDictionary<string, object> parameters)
        //{
        //    query += " ";

        //    Query q = Session.CreateQuery(query);

        //    if (parameters != null)
        //    {
        //        foreach (string key in parameters.Keys)
        //        {
        //            q.Parameters.Add(key, parameters[key]);
        //        }
        //    }

        //    int startIndex = 0;
        //    while (startIndex < query.Length)
        //    {
        //        int pos = query.IndexOf('@', startIndex);
        //        if (pos >= 0)
        //        {
        //            startIndex = pos;
        //            string parameterName = "";
        //            while (startIndex < query.Length)
        //            {
        //                if (query[startIndex] == ' ')
        //                {
        //                    if (parameters == null || !parameters.ContainsKey(parameterName))
        //                    {
        //                        object val = this.GetParameterValue(parameterName);
        //                        q.Parameters.Add(parameterName, val);
        //                    }

        //                    break;
        //                }
        //                else
        //                {
        //                    parameterName += query[startIndex];
        //                }

        //                startIndex++;
        //            }
        //        }
        //        else
        //        {
        //            break;
        //        }
        //    };

        //    return q;
        //}

        //public virtual DataObject[] ExecuteQuery(string query, IDictionary<string, object> parameters)
        //{
        //    return PrepareQuery(query, parameters).ExecuteArray();
        //}

        //public virtual long ExecuteQueryCount(string query, IDictionary<string, object> parameters)
        //{
        //    return PrepareQuery(query, parameters).ExecuteCount();
        //}

        //public virtual object GetParameterValue(string expression)
        //{
        //    if (!String.IsNullOrEmpty(expression))
        //    {
        //        if (expression[0] == '#')
        //        {
        //            object[] dObj = ExecuteQuery(expression.Substring(1), null);

        //            if (dObj == null || dObj.Length == 0)
        //            {
        //                return null;
        //            }

        //            return dObj[0];
        //        }
        //        else if (expression[0] == '@')
        //        {
        //            expression = expression.Substring(1);

        //            if (expression == "Self")
        //            {
        //                return this;
        //            }
        //            else if (expression == "UserProfile")
        //            {
        //                return Employee.GetCurrentEmployeeProfileID(Session);
        //            }
        //            else if (expression.IndexOf("User") == 0)
        //            {
        //                Employee emp = Employee.GetCurrentEmployee(Session);
        //                expression = expression.Substring("User".Length);

        //                if (!String.IsNullOrEmpty(expression))
        //                {
        //                    return emp.GetPropertyValue(expression);
        //                }

        //                return emp;
        //            }

        //            return this.GetPropertyValue(expression);
        //        }
        //    }
        //    return expression;
        //}

        //public virtual bool IsNull(string propertyName)
        //{
        //    bool isNull = false;
        //    object val = this.GetPropertyValue(propertyName);

        //    if (val == null)
        //    {
        //        isNull = true;
        //    }
        //    else
        //    {
        //        if (val is DateTime)
        //        {
        //            isNull = ((DateTime)val).Year == 1800;
        //        }
        //    }

        //    return isNull;
        //}

        //private IDictionary<string, JournalInfo> journalInfos;
        //protected virtual bool CanJournal(JournalKind kind, string propertyName)
        //{
        //    return false;
        //}

        //protected virtual BusinessObject[] JournalOwners()
        //{
        //    return new BusinessObject[] { this };
        //}

      

        //protected override void OnPersist()
        //{
        //    base.OnPersist();
        //    if (GetLogicDelegate() != null)
        //        GetLogicDelegate().OnPersist(this);
        //}

        //protected override void OnPersisted()
        //{
        //    base.OnPersisted();
        //    if (GetLogicDelegate() != null)
        //        GetLogicDelegate().OnPersisted(this);
        //    if (!IsRemoving)
        //        PersistJournal();
        //}

        //protected override void OnRemove()
        //{
        //    if (GetLogicDelegate() != null)
        //        GetLogicDelegate().OnRemove(this);
        //    if (CanJournal(JournalKind.Remove, null))
        //    {
        //        BusinessObject[] owners = JournalOwners();
        //        if (owners != null && owners.Length > 0)
        //        {
        //            JournalInfo info = OnJournalPersist(JournalKind.Remove, null);
        //            foreach (BusinessObject owner in owners)
        //            {
        //                if (owner == null)
        //                    continue;
        //                List<JournalInfo> infos = new List<JournalInfo>();
        //                infos.Add(info);
        //                owner.OnBeforeJournalPersist(infos);
        //                string desc = null;
        //                if (info != null)
        //                    desc = info.Info;
        //                if (info != null && !string.IsNullOrEmpty(info.InfoPrefix) && owner.HasJournalInfoPrefix(info))
        //                    desc = info.InfoPrefix + desc;
        //                Session.CreateObject<Journal>(owner.ID, owner.TypeID, desc, info != null ? info.Kind : JournalKind.Remove, null, null);
        //            }
        //        }
        //    }

        //    base.OnRemove();
        //}

        protected override void OnRemoved()
        {
            //Powoduje błędy podczas usuwania np. dokumentów
            //if (GetLogicDelegate() != null)
            //    GetLogicDelegate().OnRemoved(this);
            base.OnRemoved();
        }

        //public static object ValueToStorage(Session session, DataObjects.NET.ObjectModel.Field property, object propertyValue)
        //{
        //    if (propertyValue == null)
        //    {
        //        return null;
        //    }

        //    if (property.SourceType.Equals(typeof(String)))
        //    {
        //        if (property.Nullable && String.IsNullOrEmpty((string)propertyValue))
        //        {
        //            propertyValue = null;
        //        }
        //    }
        //    /*
        //     * w przypadku gdy korzystamy z web-services dodawany jest offset,
        //     * który trzeba zniwelować, w przypadku Ext.Direct dane przesyłane są bez offsetu
        //    else if (property.SourceType.Equals(typeof(DateTime)))
        //    {
        //        if (propertyValue != null)
        //        {
        //            propertyValue = ((DateTime)propertyValue).ToLocalTime();
        //        }
        //    }
        //    */
        //    else if (property.SourceType.Equals(typeof(Decimal)))
        //    {
        //        if (propertyValue != null)
        //        {
        //            if (propertyValue is string && string.IsNullOrEmpty((string)propertyValue))
        //                return null;
        //            propertyValue = Convert.ToDecimal(propertyValue);
        //        }
        //    }
        //    else if (typeof(BusinessObject).IsAssignableFrom(property.SourceType) || typeof(IDataObject).IsAssignableFrom(property.SourceType))
        //    {
        //        if (!(propertyValue is BusinessObject))
        //        {
        //            long ID = propertyValue is string && string.IsNullOrEmpty((string)propertyValue) ? 0L : (long)Convert.ChangeType(propertyValue, typeof(long));

        //            if (ID == 0)
        //            {
        //                propertyValue = null;
        //            }
        //            else
        //            {
        //                propertyValue = session[ID];
        //            }
        //        }
        //    }

        //    return propertyValue;
        //}

        //public static object ValueFromStorage(object propertyValue)
        //{
        //    if (propertyValue != null)
        //    {
        //        if (propertyValue is DateTime)
        //        {
        //            if (((DateTime)propertyValue).Year == 1800) //MSSQL DateTime.MinValue
        //            {
        //                propertyValue = null;
        //            }
        //        }
        //        else if (propertyValue is long)
        //        {
        //            return Convert.ToString((long)propertyValue);
        //        }
        //        else if (propertyValue is DBNull)
        //        {
        //            propertyValue = null;
        //        }
        //    }

        //    return propertyValue;
        //}

        /// <summary>
        /// Parses indirect parameter name of form RefEntityName_BaseEntityPropertyName_RefEntityPropertyName
        /// </summary>
        /// <param name="value">indirect parameter name</param>
        /// <param name="baseEntityPropName">out: name of the reference property in the base entity</param>
        /// <param name="refEntityName">out: name of the referenced entity</param>
        /// <param name="refEntityPropName">out: name of the property in the referenced entity</param>
        /// <returns>if not in indirect form: null, else BaseEntityPropertyName</returns>
        public static string ExtractPropertyName(string value, out string baseEntityPropName, out string refEntityName, out string refEntityPropName)
        {
            baseEntityPropName = null;
            refEntityName = null;
            refEntityPropName = null;

            if (value.IndexOf('_') > 0)
            {
                string[] parts = value.Split('_');
                if (parts.Length != 3)
                    return null;
                refEntityName = parts[0];
                baseEntityPropName = parts[1];
                refEntityPropName = parts[2];
                return baseEntityPropName;
            }
            else
                return null;
        }

        //public static DataObject[] Preload(Session session, Type entityType, long[] IDs)
        //{
        //    if (IDs.Length > 0)
        //    {
        //        DataObjects.NET.ObjectModel.Type et = MetaInfo.Instance.TypesByType[entityType];
        //        StringBuilder inPart = new StringBuilder();
        //        foreach (long ID in IDs)
        //        {
        //            inPart.Append(ID.ToString());
        //            inPart.Append(',');
        //        }
        //        inPart.Remove(inPart.Length - 1, 1);
        //        string command = "select " + et.Name + " where {ID} in (" + inPart.ToString() + ")";
        //        return session.CreateQuery(command).ExecuteArray();
        //    }
        //    return null;
        //}

        public static implicit operator long(BusinessObject bo)
        {
            if (bo == null)
                return 0;
            else
                return bo.ID;
        }
    }

    public static class BusinessObjectCopyingHelpers
    {
        public static T CreateCopy<T>(this T source, List<string> excludedFields) where T : BusinessObject
        {
            var result = source.Session.CreateObject<T>();

            source.CopyFieldsTo(result, excludedFields);

            return result;
        }

        public static void CopyFieldsTo<T>(this T source, T target, List<string> excludedFields) where T : BusinessObject
        {
            foreach (PropertyInfo property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.GetCustomAttributes(typeof(NotPersistentAttribute), true).Length == 0 &&
                    !excludedFields.Contains(property.Name))
                {
                    if (typeof(DataObjectCollection).IsAssignableFrom(property.PropertyType))
                    {
                        var collectionFrom = (DataObjectCollection)property.GetValue(source, null);
                        var collectionTo = (DataObjectCollection)property.GetValue(target, null);
                        foreach (DataObject referenced in collectionFrom)
                        {
                            collectionTo.Add(referenced);
                        }
                    }
                    else if (property.GetSetMethod() != null)
                    {
                        target.SetProperty(property.Name, property.GetValue(source, null));
                    }
                }
            }
        }
    }
}
