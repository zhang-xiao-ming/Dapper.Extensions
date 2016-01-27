using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel;
using System.Data;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Dapper.Extensions.UnitTest
{

    [TestClass]
    public class DapperExtensionsTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (IDbContext db = new DbContext())
            {
                UserEntity entity = new UserEntity
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Name = "test",
                    CreatedTime = DateTime.Now,
                    LastModifyTime = DateTime.Now
                };
                bool flag1 = db.Insert<UserEntity>(entity);
                Assert.IsTrue(flag1);

                entity.Name = "test2";
                bool flag2 = db.Update<UserEntity>(entity);
                Assert.IsTrue(flag2);

                UserEntity entity2 = db.Get<UserEntity>(entity.Id);
                Assert.IsNotNull(entity2);
                Assert.AreEqual(entity.Id, entity2.Id);
                Assert.AreEqual(entity.Name, entity2.Name);
                Assert.AreEqual(entity.CreatedTime.ToString(CultureInfo.InvariantCulture), entity2.CreatedTime.ToString(CultureInfo.InvariantCulture));
                Assert.AreEqual(entity.LastModifyTime.ToString(CultureInfo.InvariantCulture), entity2.LastModifyTime.ToString(CultureInfo.InvariantCulture));

                bool flag3 = db.Delete<UserEntity>(entity2);
                Assert.IsTrue(flag3);

                UserEntity entity3 = db.Get<UserEntity>(entity2.Id);
                Assert.IsNull(entity3);
            }
        }

        [TestMethod]
        public void TestMethod2()
        {
            using (IDbContext db = new DbContext())
            {
                string tableName = db.SqlGenerator.DbProvider.QuoteString("User");
                int result1 = db.Execute("delete from " + tableName);
                Assert.IsTrue(result1 >= 0);
                for (int i = 0; i < 10; i++)
                {
                    UserEntity user = new UserEntity
                    {
                        Age = i,
                        CreatedTime = DateTime.Now,
                        Id = Guid.NewGuid().ToString("N"),
                        LastModifyTime = DateTime.Now,
                        Name = "Name" + i,
                        RoleId = i
                    };
                    bool flag3 = db.Insert(user);
                    Assert.IsTrue(flag3);
                }

                Dictionary<string, object> dic = new Dictionary<string, object> { { "Age", -1 } };
                IList<UserEntity> list = db.List<UserEntity>("User", "Age>@Age", "RoleId ASC", dic, 0, 5);
                Assert.IsNotNull(list);
                Assert.AreEqual(5, list.Count);
                Assert.AreEqual(0, list[0].Age);
                Assert.AreEqual(4, list[4].Age);
                list = db.List<UserEntity>("User", "Age>@Age Order By RoleId ASC", dic);
                Assert.IsNotNull(list);
                Assert.AreEqual(10, list.Count);
                Assert.AreEqual(0, list[0].Age);
                Assert.AreEqual(9, list[9].Age);
            }

        }

        [TestMethod]
        public void TestMethod3()
        {
            using (IDbContext db = new DbContext())
            {
                string tableName = db.SqlGenerator.DbProvider.QuoteString("User");
                int result1 = db.Execute("delete from " + tableName);
                Assert.IsTrue(result1 >= 0);
                PagingResult<UserEntity> result2 = db.Paging<UserEntity>("", "", (DynamicParameters)null, 1, 3);
                Assert.IsNotNull(result2);
                Assert.IsNotNull(result2.List);
                Assert.AreEqual(0, result2.List.Count);
                Assert.AreEqual(0, result2.TotalPages);
                Assert.AreEqual(0, result2.TotalRecords);
                for (int i = 0; i < 10; i++)
                {
                    UserEntity user = new UserEntity
                    {
                        Age = 10,
                        CreatedTime = DateTime.Now,
                        Id = Guid.NewGuid().ToString("N"),
                        LastModifyTime = DateTime.Now,
                        Name = "Name" + i,
                        RoleId = i
                    };
                    bool flag3 = db.Insert(user);
                    Assert.IsTrue(flag3);
                }
                result2 = db.Paging<UserEntity>("", "RoleId asc", (DynamicParameters)null, 1, 3);
                Assert.IsNotNull(result2);
                Assert.IsNotNull(result2.List);
                Assert.AreEqual(3, result2.List.Count);
                Assert.AreEqual(4, result2.TotalPages);
                Assert.AreEqual(10, result2.TotalRecords);
                for (int i = 1; i <= result2.TotalPages; i++)
                {
                    result2 = db.Paging<UserEntity>("", "RoleId asc", (DynamicParameters)null, i, 3);
                    Assert.IsNotNull(result2);
                    Assert.IsNotNull(result2.List);
                    if (i == 4)
                    {
                        Assert.AreEqual(1, result2.List.Count);
                        Assert.AreEqual(9, result2.List[0].RoleId);
                    }
                    else
                    {
                        Assert.AreEqual(3, result2.List.Count);
                    }
                }
            }
        }

        [TestMethod]
        public void TestMethod4()
        {
            using (IDbContext db = new DbContext())
            {
                IdentityKey entity = new IdentityKey { Value = Guid.NewGuid().ToString() };
                bool flag = db.Insert<IdentityKey>(entity);
                Assert.IsTrue(flag);
                Assert.IsTrue(entity.Id > 0);

                IdentityKey result = db.Get<IdentityKey>(entity.Id);
                Assert.IsNotNull(result);
                Assert.AreEqual(entity.Value, result.Value);

                bool flag2 = db.Delete<IdentityKey>(entity);
                Assert.IsTrue(flag2);
                result = db.Get<IdentityKey>(entity.Id);
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestMethod5()
        {
            using (IDbContext db = new DbContext())
            {
                Multikey entity = new Multikey
                {
                    Key1 = Guid.NewGuid().ToString(),
                    Key2 = Guid.NewGuid().ToString(),
                    Value = Guid.NewGuid().ToString()
                };
                bool flag = db.Insert<Multikey>(entity);
                Assert.IsTrue(flag);

                Multikey result = db.Get<Multikey>(new { Key1 = entity.Key1, Key2 = entity.Key2 });
                Assert.IsNotNull(result);
                Assert.AreEqual(entity.Value, result.Value);

                bool flag2 = db.Delete<Multikey>(new { Key1 = entity.Key1, Key2 = entity.Key2 });
                Assert.IsTrue(flag2);
                result = db.Get<Multikey>(new { Key1 = entity.Key1, Key2 = entity.Key2 });
                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public void TestMethod6()
        {
            using (IDbContext db = new DbContext())
            {
                DataType entity = new DataType
                {
                    Id = Guid.NewGuid().ToString(),
                    TInt = 10,
                    TLong = 1000000L,
                    TDecimal = 343432.2321M,
                    TFloat = 34.34F,
                    TDouble = 3234.32D,
                    TBool = true,
                    TDateTime = DateTime.Now,
                    TString = Guid.NewGuid().ToString(),
                    TBypes = Guid.NewGuid().ToByteArray(),
                    TEnum = DbType.DateTime,
                    TSingle = 433.34F
                };
                bool flag = db.Insert<DataType>(entity);
                Assert.IsTrue(flag);

                DataType result = db.Get<DataType>(entity.Id);
                Assert.IsNotNull(result);
                Assert.AreEqual(entity.Id, result.Id);
                Assert.AreEqual(entity.TInt, result.TInt);
                Assert.AreEqual(entity.TLong, result.TLong);
                Assert.AreEqual(entity.TDecimal, result.TDecimal);
                Assert.AreEqual(entity.TFloat, result.TFloat);
                Assert.AreEqual(entity.TDouble, result.TDouble);
                Assert.AreEqual(entity.TBool, result.TBool);
                Assert.AreEqual(entity.TDateTime.ToString(CultureInfo.InvariantCulture), result.TDateTime.ToString(CultureInfo.InvariantCulture));
                Assert.AreEqual(entity.TString, result.TString);
                Assert.AreEqual(0, MemoryCompare(entity.TBypes, result.TBypes));
                Assert.AreEqual(entity.TEnum, result.TEnum);
                Assert.AreEqual(entity.TSingle, result.TSingle);
            }
        }

        [TestMethod]
        public void TestMethod7()
        {
            using (IDbContext db = new DbContext())
            {
                NullableDataType entity = new NullableDataType { Id = Guid.NewGuid().ToString() };
                bool flag = db.Insert<NullableDataType>(entity);
                Assert.IsTrue(flag);

                NullableDataType result = db.Get<NullableDataType>(entity.Id);
                Assert.IsNotNull(result);
                Assert.AreEqual(entity.Id, result.Id);
                Assert.AreEqual(entity.TInt, result.TInt);
                Assert.AreEqual(entity.TLong, result.TLong);
                Assert.AreEqual(entity.TDecimal, result.TDecimal);
                Assert.AreEqual(entity.TFloat, result.TFloat);
                Assert.AreEqual(entity.TDouble, result.TDouble);
                Assert.AreEqual(entity.TBool, result.TBool);
                Assert.AreEqual(entity.TDateTime.ToString(), result.TDateTime.ToString());
                Assert.AreEqual(entity.TString, result.TString);
                Assert.AreEqual(0, MemoryCompare(entity.TBypes, result.TBypes));
                Assert.AreEqual(entity.TEnum, result.TEnum);
                Assert.AreEqual(entity.TSingle, result.TSingle);

            }
        }

        [TestMethod]
        public void TestMethod8()
        {
            using (IDbContext db = new DbContext())
            {
                Alias entity = new Alias
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "tom",
                    Age = 10,
                    CreatedTime = DateTime.Now
                };
                bool flag = db.Insert<Alias>(entity);
                Assert.IsTrue(flag);

                Alias result = db.Get<Alias>(entity.Id);
                Assert.IsNotNull(result);
                Assert.AreEqual(entity.Id, result.Id);
                Assert.AreEqual(entity.Name, result.Name);
                Assert.AreEqual(entity.Age, result.Age);
                Assert.AreEqual(entity.CreatedTime.ToString(CultureInfo.InvariantCulture), result.CreatedTime.ToString(CultureInfo.InvariantCulture));
                //参数的写法1
                //IDictionary<string, object> parameters = new Dictionary<string, object>();
                //parameters.Add("Name", "tom");
                //IList<Alias> list = db.List<Alias>("#Name=@Name", parameters);
                //参数的写法2
                //IList<Alias> list = db.List<Alias>("#Name=@Name", new { Name = "tom" });

                //参数的写法3
                DynamicParameters dynamicParameters = new DynamicParameters();
                //dynamicParameters.Add("Name", "tom");
                dynamicParameters.Add("Name", "tom", DbType.AnsiString, ParameterDirection.Input, 50);
                IList<Alias> list = db.List<Alias>("#Name=@Name", dynamicParameters);
                Assert.IsNotNull(list);
            }
        }

        [TestMethod]
        public void TestMethod9()
        {
            string id = Guid.NewGuid().ToString();
            try
            {
                using (IDbContext db = new DbContext())
                {
                    Alias alias = new Alias
                    {
                        Id = id,
                        Name = "tom",
                        Age = 10,
                        CreatedTime = DateTime.Now
                    };
                    bool flag = db.Insert<Alias>(alias);
                    Assert.IsTrue(flag);

                    DataType dataType = new DataType
                    {
                        Id = id,
                        TInt = 10,
                        TLong = 1000000L,
                        TDecimal = 343432.2321M,
                        TFloat = 34.34F,
                        TDouble = 3234.32D,
                        TBool = true,
                        TDateTime = DateTime.Now,
                        TString = Guid.NewGuid().ToString() + Guid.NewGuid().ToString(),
                        TBypes = Guid.NewGuid().ToByteArray(),
                        TEnum = DbType.DateTime,
                        TSingle = 433.34F
                    };
                    bool flag2 = db.Insert<DataType>(dataType);
                    Assert.IsTrue(flag2);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            using (IDbContext db = new DbContext())
            {
                Alias getAlias = db.Get<Alias>(id);
                Assert.IsNotNull(getAlias);

                DataType getDataType = db.Get<DataType>(id);
                Assert.IsNull(getDataType);
            }

        }

        [TestMethod]
        public void TestMethod10()
        {
            string id = Guid.NewGuid().ToString();
            try
            {
                using (IDbContext db = new DbContext().UseDbTransaction())
                {
                    Alias alias = new Alias
                    {
                        Id = id,
                        Name = "tom",
                        Age = 10,
                        CreatedTime = DateTime.Now
                    };
                    bool flag = db.Insert<Alias>(alias);
                    Assert.IsTrue(flag);

                    DataType dataType = new DataType
                    {
                        Id = id,
                        TInt = 10,
                        TLong = 1000000L,
                        TDecimal = 343432.2321M,
                        TFloat = 34.34F,
                        TDouble = 3234.32D,
                        TBool = true,
                        TDateTime = DateTime.Now,
                        TString = Guid.NewGuid().ToString() + Guid.NewGuid().ToString(),
                        TBypes = Guid.NewGuid().ToByteArray(),
                        TEnum = DbType.DateTime,
                        TSingle = 433.34F
                    };
                    bool flag2 = db.Insert<DataType>(dataType);
                    Assert.IsTrue(flag2);
                    db.Commit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            using (IDbContext db = new DbContext())
            {
                Alias getAlias = db.Get<Alias>(id);
                Assert.IsNull(getAlias);

                DataType getDataType = db.Get<DataType>(id);
                Assert.IsNull(getDataType);
            }
        }

        [TestMethod]
        public void TestMethod11()
        {
            string id = Guid.NewGuid().ToString();
            try
            {
                using (IDbContext db = new DbContext().UseDbTransaction())
                {
                    Alias alias = new Alias
                    {
                        Id = id,
                        Name = "tom",
                        Age = 10,
                        CreatedTime = DateTime.Now
                    };
                    bool flag = db.Insert<Alias>(alias);
                    Assert.IsTrue(flag);

                    DataType dataType = new DataType
                    {
                        Id = id,
                        TInt = 10,
                        TLong = 1000000L,
                        TDecimal = 343432.2321M,
                        TFloat = 34.34F,
                        TDouble = 3234.32D,
                        TBool = true,
                        TDateTime = DateTime.Now,
                        TString = Guid.NewGuid().ToString(),
                        TBypes = Guid.NewGuid().ToByteArray(),
                        TEnum = DbType.DateTime,
                        TSingle = 433.34F
                    };
                    bool flag2 = db.Insert<DataType>(dataType);
                    Assert.IsTrue(flag2);
                    db.Commit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            using (IDbContext db = new DbContext())
            {
                Alias getAlias = db.Get<Alias>(id);
                Assert.IsNotNull(getAlias);

                DataType getDataType = db.Get<DataType>(id);
                Assert.IsNotNull(getDataType);
            }
        }

        [TestMethod]

        public void TestMethod12()
        {
            using (IDbContext db = new DbContext())
            {
                UserEntity entity = new UserEntity
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Name = "test",
                    Age = 10,
                    CreatedTime = DateTime.Now,
                    LastModifyTime = DateTime.Now
                };
                bool flag1 = db.Insert<UserEntity>(entity);
                Assert.IsTrue(flag1);

                DataTable dt = db.GetDataTable("select * from [User]");
                Assert.IsNotNull(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    string id = dr.GetValue<string>("Id");
                    int age = dr.GetValue<int>("Age");
                }
            }
        }

        [TestMethod]
        public void TestMethod13()
        {
            using (IDbContext db = new DbContext())
            {
                RoleEntity entity1 = new RoleEntity
                {
                    Name = "jack",
                    CreatedTime = DateTime.Now,
                    LastModifyTime = DateTime.Now
                };
                bool flag1 = db.Insert<RoleEntity>(entity1);
                Assert.IsTrue(flag1);

                RoleEntity entity2 = db.Get<RoleEntity>(entity1.Id);
                Assert.IsNotNull(entity2);
                entity2.Name = "tom";
                entity2.LastModifyTime = DateTime.Now;
                bool flag2 = db.Update<RoleEntity>(entity2);
                Assert.IsTrue(flag2);

                RoleEntity entity3 = db.Get<RoleEntity>(entity1.Id);
                Assert.IsNotNull(entity3);
                Assert.AreEqual(entity2.Name, entity3.Name);
                Assert.AreEqual(entity1.Version + 1, entity3.Version);
            }
        }

        [TestMethod]
        public void TestMethod14()
        {
            using (IDbContext db = new DbContext())
            {
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("Value", Guid.NewGuid().ToString());
                int result = db.Execute("IdentityKey_Insert", parameter, null, CommandType.StoredProcedure);
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestMethod15()
        {
            using (IDbContext db = new DbContext())
            {
                IdentityKey entity = new IdentityKey { Value = Guid.NewGuid().ToString() };
                bool flag = db.Insert<IdentityKey>(entity);
                Assert.IsTrue(flag);
                Assert.IsTrue(entity.Id > 0);
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("Id", entity.Id);
                IdentityKey result = db.Query<IdentityKey>("IdentityKey_Select_Single", parameter, true, null, CommandType.StoredProcedure).FirstOrDefault();
                Assert.IsNotNull(result);
            }
        }

        [TestMethod]
        public void TestMethod16()
        {
            using (IDbContext db = new DbContext())
            {
                RoleEntity entity1 = new RoleEntity
                {
                    Name = "jack",
                    CreatedTime = DateTime.Now,
                    LastModifyTime = DateTime.Now
                };
                bool flag1 = db.Insert<RoleEntity>(entity1);
                Assert.IsTrue(flag1);

                UserEntity entity2 = new UserEntity
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Name = "test",
                    CreatedTime = DateTime.Now,
                    LastModifyTime = DateTime.Now,
                    RoleId = entity1.Id
                };
                bool flag2 = db.Insert<UserEntity>(entity2);
                Assert.IsTrue(flag2);

                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("RoleId", entity1.Id);
                parameter.Add("TotalRecords", null, DbType.Int32, ParameterDirection.Output);
                IList<UserEntity> result = db.Query<UserEntity>("User_Select_ResultValue", parameter, true, null, CommandType.StoredProcedure);
                Assert.IsNotNull(result);
                Assert.IsTrue(result.Count >= 1);
                int totalRecords = parameter.Get<int>("TotalRecords");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestMethod17()
        {
            using (IDbContext db = new DbContext())
            {
                IdentityKey entity = new IdentityKey { Value = Guid.NewGuid().ToString() };
                bool flag = db.Save<IdentityKey>(entity);
                Assert.IsTrue(flag);
                Assert.IsTrue(entity.Id > 0);
            }
        }

        [TestMethod]
        public void TestMethod18()
        {
            using (IDbContext db = new DbContext())
            {
                RoleEntity entity1 = new RoleEntity
                {
                    Name = "jack",
                    CreatedTime = DateTime.Now,
                    LastModifyTime = DateTime.Now
                };
                bool flag1 = db.Save<RoleEntity>(entity1);
                Assert.IsTrue(flag1);

                RoleEntity entity2 = db.Get<RoleEntity>(entity1.Id);
                Assert.IsNotNull(entity2);
                Assert.AreEqual(true, entity2.IsPersisted);
                entity2.Name = "tom";
                entity2.LastModifyTime = DateTime.Now;
                bool flag2 = db.Save<RoleEntity>(entity2);
                Assert.IsTrue(flag2);

                RoleEntity entity3 = db.Get<RoleEntity>(entity1.Id);
                Assert.IsNotNull(entity3);
                Assert.AreEqual(true, entity3.IsPersisted);
                Assert.AreEqual(entity2.Name, entity3.Name);
                Assert.AreEqual(entity1.Version + 1, entity3.Version);
            }
        }

        [TestMethod]
        public void TestMethod19()
        {
            IList<RoleEntity> entities = new List<RoleEntity>();
            for (int i = 0; i < 10; i++)
            {
                RoleEntity entity = new RoleEntity
                {
                    Name = "jack" + i.ToString(),
                    CreatedTime = DateTime.Now,
                    LastModifyTime = DateTime.Now
                };
                entities.Add(entity);
            }

            using (IDbContext db = new DbContext())
            {
                bool flag = db.Insert<RoleEntity>(entities);
                Assert.IsTrue(flag);
            }
        }

        [TestMethod]
        public void TestMethod20()
        {
            IList<RoleEntity> entities = new List<RoleEntity>();
            for (int i = 0; i < 10; i++)
            {
                RoleEntity entity = new RoleEntity
                {
                    Name = "jack" + i.ToString(),
                    CreatedTime = DateTime.Now,
                    LastModifyTime = DateTime.Now
                };
                entities.Add(entity);
            }

            using (IDbContext db = new DbContext())
            {
                IClassMapper classMapper = ClassMapperFactory.GetMapper<RoleEntity>();
                string tableName = classMapper.TableName;
                int result1 = db.Execute("delete from " + tableName);
                Assert.IsTrue(result1 >= 0);

                bool flag = db.Insert<RoleEntity>(entities);
                Assert.IsTrue(flag);

                string condition = "LastModifyTime<=@Now";
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("Name", "tom");
                dynamicParameters.Add("CreatedTime", new DateTime(1900, 1, 1));
                dynamicParameters.Add("Now", DateTime.Now);
                bool flag1 = db.Update(tableName, new List<string>() { "Name", "CreatedTime" }, condition, dynamicParameters);
                Assert.IsTrue(flag1);
                IList<RoleEntity> list = db.List<RoleEntity>(null, (object)null);
                Assert.IsNotNull(list);
                Assert.AreEqual(10, list.Count);

                foreach (RoleEntity roleEntity in list)
                {
                    Assert.IsNotNull(roleEntity);
                    Assert.AreEqual("tom", roleEntity.Name);
                    Assert.AreEqual(new DateTime(1900, 1, 1), roleEntity.CreatedTime);
                }
            }
        }

        [TestMethod]
        public void TestMethod21()
        {
            using (IDbContext db = new DbContext())
            {
                PropertyChangedModel model = new PropertyChangedModel
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Name = "tom",
                    Age=10
                };

                bool flag1 = db.Insert(model);
                Assert.IsTrue(flag1);

                PropertyChangedModel model1 = db.Get<PropertyChangedModel>(model.Id);
                Assert.IsNotNull(model1);

                model1.Name = "jack";
                model1.Age = 20;
                Assert.AreEqual(1, model1.PropertyChangedList.Count);
                Assert.IsTrue(model1.PropertyChangedList.Contains("Name"));
                bool flag2 = db.Update(model1);

                PropertyChangedModel model2 = db.Get<PropertyChangedModel>(model.Id);
                Assert.IsNotNull(model2);
                Assert.AreEqual(model.Id, model2.Id);
                Assert.AreEqual("jack", model2.Name);
                Assert.AreEqual(model.Age, model2.Age);

            }
        }

        /// <summary>
        /// 比较字节数组
        /// </summary>
        /// <param name="bytes1">字节数组1</param>
        /// <param name="bytes2">字节数组2</param>
        /// <returns>如果两个数组相同，返回0；如果数组1小于数组2，返回小于0的值；如果数组1大于数组2，返回大于0的值。</returns>
        private int MemoryCompare(byte[] bytes1, byte[] bytes2)
        {
            int result = 0;
            if (bytes1 == null && bytes2 == null)
                return result;
            if (bytes1.Length != bytes2.Length)
                result = bytes1.Length - bytes2.Length;
            else
            {
                for (int i = 0; i < bytes1.Length; i++)
                {
                    if (bytes1[i] != bytes2[i])
                    {
                        result = (int)(bytes1[i] - bytes2[i]);
                        break;
                    }
                }
            }
            return result;
        }
    }
}
