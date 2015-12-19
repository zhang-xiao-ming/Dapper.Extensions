using System;
using System.Collections.Generic;

namespace Dapper.Extensions
{
    public interface IClassMapper
    {
        string ConnectionName { get; set; }
        string SchemaName { get; set; }
        string TableName { get; set; }
        IList<IPropertyMap> Properties { get; }
        Type EntityType { get; }
        IPropertyMap GetPropertyMapByName(string name);
        string GetColumnName(string name);
        string GetPropertyName(string columnName);
        IPropertyMap GetPropertyMapByColumnName(string columnName);
        IList<IPropertyMap> GetKeys();
        IPropertyMap GetVersionMap();
        IPropertyMap GetPersistedMap();
    }

    public interface IClassMapper<T> : IClassMapper where T : class
    {
        void BeforeSave(T entity);
        void AfterSave(T entity);
    }
}
