using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Dapper.Extensions
{
    public class PropertyMap : IPropertyMap
    {
        public PropertyMap(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");
            PropertyInfo = propertyInfo;
            ColumnName = PropertyInfo.Name;
        }

        public string Name
        {
            get { return PropertyInfo.Name; }
        }

        public string ColumnName { get; private set; }

        public KeyType KeyType { get; private set; }

        public bool IsIgnored { get; private set; }

        public bool IsVersion { get; private set; }

        public bool IsReadOnly { get; private set; }

        public bool IsPersisted { get; private set; }

        public bool IsPropertyChangedList { get; private set; }

        public PropertyInfo PropertyInfo { get; private set; }

        public PropertyMap Column(string columnName)
        {
            ColumnName = columnName;
            return this;
        }

        public PropertyMap Key(KeyType keyType)
        {
            if (IsIgnored)
            {
                throw new ArgumentException(string.Format("'{0}' 是一个被忽略的字段不能被用来作为主键字段。", Name));
            }

            if (IsReadOnly)
            {
                throw new ArgumentException(string.Format("'{0}' 是一个只读的字段不能被用来作为主键字段。 ", Name));
            }

            KeyType = keyType;
            return this;
        }

        public PropertyMap Ignore()
        {
            if (KeyType != KeyType.NotAKey)
            {
                throw new ArgumentException(string.Format("'{0}' 是一个主键字段不能被忽略。", Name));
            }

            IsIgnored = true;
            return this;
        }

        public PropertyMap Version()
        {
            if (KeyType != KeyType.NotAKey)
            {
                throw new ArgumentException(string.Format("'{0}' 是一个主键字段不能被作为一个并发锁列。", Name));
            }
            if (!PropertyInfo.PropertyType.Equals(typeof(int)))
            {
                throw new ArgumentException(string.Format("'{0}' 的类型必须是整型。", Name));
            }
            IsVersion = true;
            return this;
        }

        public PropertyMap Persisted()
        {
            if (KeyType != KeyType.NotAKey)
            {
                throw new ArgumentException(string.Format("'{0}' 是一个主键字段不能被作为一个已持久化的标识列。", Name));
            }
            if (!PropertyInfo.PropertyType.Equals(typeof(bool)))
            {
                throw new ArgumentException(string.Format("'{0}' 的类型必须是bool型。", Name));
            }
            IsIgnored = true;
            IsPersisted = true;
            return this;
        }

        public PropertyMap ReadOnly()
        {
            if (KeyType != KeyType.NotAKey)
            {
                throw new ArgumentException(string.Format("'{0}' 是一个主键字段，不能被标识为只读列。", Name));
            }

            IsReadOnly = true;
            return this;
        }

        public PropertyMap PropertyChangedList()
        {
            if (KeyType != KeyType.NotAKey)
            {
                throw new ArgumentException(string.Format("'{0}' 是一个主键字段不能被作为一个原始值字典属性。", Name));
            }
            if (!PropertyInfo.PropertyType.Equals(typeof(IList<string>)))
            {
                throw new ArgumentException(string.Format("'{0}' 的类型必须是IList<string>型。", Name));
            }
            IsIgnored = true;
            IsPropertyChangedList = true;
            return this;
        }
    }

    public enum KeyType
    {
        NotAKey = 0,

        Identity = 1,

        Assigned = 2
    }
}