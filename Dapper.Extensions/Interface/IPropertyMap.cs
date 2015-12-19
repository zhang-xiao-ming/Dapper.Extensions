using System.Reflection;

namespace Dapper.Extensions
{
    public interface IPropertyMap
    {
        string Name { get; }
        string ColumnName { get; }
        bool IsIgnored { get; }
        bool IsReadOnly { get; }
        KeyType KeyType { get; }
        PropertyInfo PropertyInfo { get; }
        bool IsVersion { get; }
        bool IsPersisted { get; }
    }
}
