using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Dapper.Extensions
{
    public static class ClassMapperFactory
    {
        private static IList<Assembly> _mapperAssemblies;
        private static readonly Dictionary<Type, Type> ClassMapperTypes = new Dictionary<Type, Type>();
        private static readonly object SyncRoot = new object();
        #region Mapper

        public static IClassMapper GetMapper(Type entityType, string tableName = null)
        {
            Type classMapperType = GetMapperType(entityType);
            IClassMapper map = Activator.CreateInstance(classMapperType) as IClassMapper;
            if (map == null)
                throw new NullReferenceException(string.Format("{0}对应的ClassMapper为null。", entityType.FullName));
            if (!string.IsNullOrWhiteSpace(tableName))
                map.TableName = tableName;
            else if (string.IsNullOrWhiteSpace(map.TableName))
                map.TableName = entityType.Name;
            return map;
        }

        public static IClassMapper<T> GetMapper<T>(string tableName = null) where T : class
        {
            return GetMapper(typeof(T), tableName) as IClassMapper<T>;
        }

        private static IEnumerable<Assembly> GetMapperAssemblies()
        {
            if (_mapperAssemblies != null)
                return _mapperAssemblies;
            IList<Assembly> assemblys = AppDomain.CurrentDomain.GetAssemblies();
            string path = System.AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo dir = new DirectoryInfo(path);
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                if (fileInfo == null || fileInfo.Extension != ".dll")
                    continue;
                try
                {
                    Assembly assembly = Assembly.LoadFile(fileInfo.FullName);
                    if (assembly == null || assemblys.Contains(assembly))
                        continue;
                    assemblys.Add(assembly);
                }
                catch
                {
                    // ignored
                }
            }
            string nameList = ConfigurationManager.AppSettings["MapperAssemblies"];
            if (!string.IsNullOrWhiteSpace(nameList))
            {
                string[] arrays = nameList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string name in arrays)
                {
                    try
                    {
                        Assembly assembly = Assembly.Load(name);
                        if (assembly == null || assemblys.Contains(assembly))
                            continue;
                        assemblys.Add(assembly);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
            _mapperAssemblies = assemblys.Where(assembly => assembly.FullName != typeof(IClassMapper<>).Assembly.FullName && assembly.GetTypes().FirstOrDefault(m => m != null && m.GetInterface(typeof(IClassMapper<>).FullName) != null) != null).ToList();
            return _mapperAssemblies;
        }

        private static Type CreateMapperType(Type entityType)
        {
            Func<Assembly, Type> getType = a =>
            {
                Type[] types = a.GetTypes();
                return (from type in types
                        let interfaceType = type.GetInterface(typeof(IClassMapper<>).FullName)
                        where
                            interfaceType != null &&
                            interfaceType.GetGenericArguments()[0] == entityType
                        select type).SingleOrDefault();
            };

            Type result = getType(entityType.Assembly);
            if (result != null)
            {
                return result;
            }

            foreach (var mappingAssembly in GetMapperAssemblies())
            {
                result = getType(mappingAssembly);
                if (result != null)
                {
                    return result;
                }
            }

            Type classMapperType = getType(entityType.Assembly);
            return classMapperType ?? typeof(AutoClassMapper<>).MakeGenericType(entityType);
        }

        private static Type GetMapperType(Type entityType)
        {
            Type classMapperType;
            ClassMapperTypes.TryGetValue(entityType, out classMapperType);
            if (classMapperType != null)
                return classMapperType;
            if (!ClassMapperTypes.ContainsKey(entityType))
            {
                lock (SyncRoot)
                {
                    if (!ClassMapperTypes.ContainsKey(entityType))
                    {
                        classMapperType = CreateMapperType(entityType);
                        ClassMapperTypes.Add(entityType, classMapperType);
                    }
                }
            }

            return classMapperType ?? CreateMapperType(entityType);
        }

        #endregion
    }
}
