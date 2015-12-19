using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

namespace Dapper.Extensions
{
    public class ClassMapper<T> : IClassMapper<T> where T : class
    {

        public string ConnectionName { get; set; }
        public string SchemaName { get; set; }
        public string TableName { get; set; }

        public Action<T> BeforeSaveAction { get; set; }

        public Action<T> AfterSaveAction { get; set; }

        public IList<IPropertyMap> Properties { get; private set; }

        public Type EntityType
        {
            get { return typeof(T); }
        }

        public ClassMapper()
        {
            PropertyTypeKeyTypeMapping = new Dictionary<Type, KeyType>
                                             {
                                                 { typeof(byte), KeyType.Identity }, { typeof(byte?), KeyType.Identity },
                                                 { typeof(sbyte), KeyType.Identity }, { typeof(sbyte?), KeyType.Identity },
                                                 { typeof(short), KeyType.Identity }, { typeof(short?), KeyType.Identity },
                                                 { typeof(ushort), KeyType.Identity }, { typeof(ushort?), KeyType.Identity },
                                                 { typeof(int), KeyType.Identity }, { typeof(int?), KeyType.Identity },
                                                 { typeof(uint), KeyType.Identity}, { typeof(uint?), KeyType.Identity },
                                                 { typeof(long), KeyType.Identity }, { typeof(long?), KeyType.Identity },
                                                 { typeof(ulong), KeyType.Identity }, { typeof(ulong?), KeyType.Identity },
                                                 { typeof(BigInteger), KeyType.Identity }, { typeof(BigInteger?), KeyType.Identity },
                                                 { typeof(Guid), KeyType.Assigned }, { typeof(Guid?), KeyType.Assigned }
                                             };

            Properties = new List<IPropertyMap>();
            TableName = typeof(T).Name;
        }

        protected Dictionary<Type, KeyType> PropertyTypeKeyTypeMapping { get; private set; }



        protected virtual void AutoMap()
        {
            AutoMap(null);
        }

        protected virtual void AutoMap(Func<Type, PropertyInfo, bool> canMap)
        {
            Type type = typeof(T);
            bool hasDefinedKey = Properties.Any(p => p.KeyType != KeyType.NotAKey);
            PropertyMap keyMap = null;
            foreach (var propertyInfo in type.GetProperties())
            {
                if (Properties.Any(p => p.Name.Equals(propertyInfo.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    continue;
                }

                if ((canMap != null && !canMap(type, propertyInfo)))
                {
                    continue;
                }

                PropertyMap map = MapProperty(propertyInfo);
                if (!hasDefinedKey)
                {
                    if (string.Equals(map.PropertyInfo.Name, "id", StringComparison.InvariantCultureIgnoreCase))
                    {
                        keyMap = map;
                    }

                    if (keyMap == null && map.PropertyInfo.Name.EndsWith("id", true, CultureInfo.InvariantCulture))
                    {
                        keyMap = map;
                    }
                }
            }

            if (keyMap != null)
                keyMap.Key(PropertyTypeKeyTypeMapping.ContainsKey(keyMap.PropertyInfo.PropertyType)
                    ? PropertyTypeKeyTypeMapping[keyMap.PropertyInfo.PropertyType]
                    : KeyType.Assigned);
        }

        protected PropertyMap MapProperty(Expression<Func<T, object>> expression)
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return MapProperty(propertyInfo);
        }

        protected PropertyMap MapProperty(PropertyInfo propertyInfo)
        {
            PropertyMap result = new PropertyMap(propertyInfo);
            GuardForDuplicatePropertyMap(result);
            Properties.Add(result);
            return result;
        }

        private void GuardForDuplicatePropertyMap(PropertyMap result)
        {
            if (Properties.Any(p => p.Name.Equals(result.Name)))
            {
                throw new ArgumentException(string.Format("属性{0}发现多个映射配置。 ", result.Name));
            }
        }

        public IPropertyMap GetPropertyMapByName(string name)
        {
            return Properties.SingleOrDefault(m => m != null && string.Compare(m.Name, name, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public IPropertyMap GetPropertyMapByColumnName(string columnName)
        {
            return Properties.SingleOrDefault(m => m != null && string.Compare(m.ColumnName, columnName, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public IList<IPropertyMap> GetKeys()
        {
            return Properties.Where(p => p.KeyType != KeyType.NotAKey).ToList();
        }

        public IPropertyMap GetVersionMap()
        {
            return Properties.SingleOrDefault(p => p.IsVersion);
        }

        public IPropertyMap GetPersistedMap()
        {
            return Properties.SingleOrDefault(p => p.IsPersisted);
        }

        public string GetColumnName(string name)
        {
            IPropertyMap map = GetPropertyMapByName(name);
            return map != null ? map.ColumnName : null;
        }

        public string GetPropertyName(string columnName)
        {
            IPropertyMap map = GetPropertyMapByColumnName(columnName);
            return map != null ? map.Name : null;
        }

        public virtual void BeforeSave(T entity)
        {
            if (BeforeSaveAction != null) BeforeSaveAction.Invoke(entity);
        }

        public virtual void AfterSave(T entity)
        {
            if (AfterSaveAction != null) AfterSaveAction.Invoke(entity);
        }
    }
}