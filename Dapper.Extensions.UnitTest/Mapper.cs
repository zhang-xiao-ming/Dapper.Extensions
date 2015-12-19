using DomainModel;
using System;

namespace Dapper.Extensions.UnitTest
{
    public sealed class UserEntityMapper : ClassMapper<UserEntity>
    {
        public UserEntityMapper()
        {
            TableName = "User";
            AutoMap();
        }
    }

    public sealed class MultikeyMapper : ClassMapper<Multikey>
    {
        public MultikeyMapper()
        {
            MapProperty(p => p.Key1).Key(KeyType.Assigned);
            MapProperty(p => p.Key2).Key(KeyType.Assigned);
            AutoMap();
        }
    }

    public sealed class IdentityKeyMapper : ClassMapper<IdentityKey>
    {
        public IdentityKeyMapper()
        {
            MapProperty(p => p.Id).Key(KeyType.Identity);
            AutoMap();
        }
    }

    public sealed class NullableDataTypeMapper : ClassMapper<NullableDataType>
    {
        public NullableDataTypeMapper()
        {
            TableName = "DataType";
            MapProperty(p => p.Id).Key(KeyType.Assigned);
            AutoMap();
        }
    }

    public sealed class AliasMapper : ClassMapper<Alias>
    {
        public AliasMapper()
        {
            TableName = "Alias";
            MapProperty(p => p.Id).Column("AliasId").Key(KeyType.Assigned);
            MapProperty(p => p.Name).Column("sName");
            MapProperty(p => p.Age).Column("iAge");
            MapProperty(p => p.CreatedTime).Column("dCreatedTime");
            AutoMap();
        }
    }

    public sealed class RoleEntityMapper : ClassMapper<RoleEntity>
    {
        public RoleEntityMapper()
        {
            TableName = "Role";
            BeforeSaveAction = (entity) =>
            {
                Console.WriteLine("Role,BeforeSave,Name={0}", entity.Name);
            };
            AfterSaveAction = (entity) =>
            {
                Console.WriteLine("Role,AfterSave,Name={0}", entity.Name);
            };
            MapProperty(p => p.Id).Key(KeyType.Identity);
            MapProperty(p => p.Version).Version();
            MapProperty(p => p.IsPersisted).Persisted();
            AutoMap();
        }
    }
}
