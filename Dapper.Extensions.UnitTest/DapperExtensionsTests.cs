using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using DomainModel;
using System.Data;
using System.Globalization;

namespace Dapper.Extensions.UnitTest
{
    [TestFixture]
    public class DapperExtensionsTests
    {
        [Test]
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

        [Test]
        public void TestMethod2()
        {
            IList<UserEntity> list = null;
            using (IDbContext db = new DbContext())
            {
                Dictionary<string, object> dic = new Dictionary<string, object> { { "Age", -1 } };
                list = db.List<UserEntity>("User", "Age>@Age", dic, 0, 10);
            }
            Assert.NotNull(list);
        }

        [Test]
        public void TestMethod3()
        {
            using (IDbContext db = new DbContext())
            {
                PagingResult<UserEntity> result = db.Paging<UserEntity>("User", "", null, 1, 3);
                Assert.NotNull(result);
            }
        }

        [Test]
        public void TestMethod4()
        {
            using (IDbContext db = new DbContext())
            {
                IdentityKey entity = new IdentityKey { Value = Guid.NewGuid().ToString() };
                bool flag = db.Insert<IdentityKey>(entity);
                Assert.IsTrue(flag);
                Assert.IsTrue(entity.Id > 0);

                IdentityKey result = db.Get<IdentityKey>(entity.Id);
                Assert.NotNull(result);
                Assert.AreEqual(entity.Value, result.Value);

                bool flag2 = db.Delete<IdentityKey>(entity);
                Assert.IsTrue(flag2);
                result = db.Get<IdentityKey>(entity.Id);
                Assert.IsNull(result);
            }
        }

        [Test]
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
                Assert.NotNull(result);
                Assert.AreEqual(entity.Value, result.Value);

                bool flag2 = db.Delete<Multikey>(new { Key1 = entity.Key1, Key2 = entity.Key2 });
                Assert.IsTrue(flag2);
                result = db.Get<Multikey>(new { Key1 = entity.Key1, Key2 = entity.Key2 });
                Assert.IsNull(result);
            }
        }

        [Test]
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
                Assert.NotNull(result);
                Assert.AreEqual(entity.Id, result.Id);
                Assert.AreEqual(entity.TInt, result.TInt);
                Assert.AreEqual(entity.TLong, result.TLong);
                Assert.AreEqual(entity.TDecimal, result.TDecimal);
                Assert.AreEqual(entity.TFloat, result.TFloat);
                Assert.AreEqual(entity.TDouble, result.TDouble);
                Assert.AreEqual(entity.TBool, result.TBool);
                Assert.AreEqual(entity.TDateTime.ToString(CultureInfo.InvariantCulture), result.TDateTime.ToString(CultureInfo.InvariantCulture));
                Assert.AreEqual(entity.TString, result.TString);
                Assert.AreEqual(entity.TBypes, result.TBypes);
                Assert.AreEqual(entity.TEnum, result.TEnum);
                Assert.AreEqual(entity.TSingle, result.TSingle);
            }
        }

        [Test]
        public void TestMethod7()
        {
            using (IDbContext db = new DbContext())
            {
                NullableDataType entity = new NullableDataType { Id = Guid.NewGuid().ToString() };
                bool flag = db.Insert<NullableDataType>(entity);
                Assert.IsTrue(flag);

                NullableDataType result = db.Get<NullableDataType>(entity.Id);
                Assert.NotNull(result);
                Assert.AreEqual(entity.Id, result.Id);
                Assert.AreEqual(entity.TInt, result.TInt);
                Assert.AreEqual(entity.TLong, result.TLong);
                Assert.AreEqual(entity.TDecimal, result.TDecimal);
                Assert.AreEqual(entity.TFloat, result.TFloat);
                Assert.AreEqual(entity.TDouble, result.TDouble);
                Assert.AreEqual(entity.TBool, result.TBool);
                Assert.AreEqual(entity.TDateTime.ToString(), result.TDateTime.ToString());
                Assert.AreEqual(entity.TString, result.TString);
                Assert.AreEqual(entity.TBypes, result.TBypes);
                Assert.AreEqual(entity.TEnum, result.TEnum);
                Assert.AreEqual(entity.TSingle, result.TSingle);

            }
        }

        [Test]
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
                Assert.NotNull(result);
                Assert.AreEqual(entity.Id, result.Id);
                Assert.AreEqual(entity.Name, result.Name);
                Assert.AreEqual(entity.Age, result.Age);
                Assert.AreEqual(entity.CreatedTime.ToString(CultureInfo.InvariantCulture), result.CreatedTime.ToString(CultureInfo.InvariantCulture));
                //IDictionary<string, object> parameters = new Dictionary<string, object>();
                //parameters.Add("Name", "tom");
                IList<Alias> list = db.List<Alias>("#Name=@Name", new { Name = "tom" });
                Assert.NotNull(list);
            }
        }


        [Test]
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
                Assert.NotNull(getAlias);

                DataType getDataType = db.Get<DataType>(id);
                Assert.Null(getDataType);
            }

        }


        [Test]
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
                Assert.Null(getAlias);

                DataType getDataType = db.Get<DataType>(id);
                Assert.Null(getDataType);
            }
        }

        [Test]
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
                Assert.NotNull(getAlias);

                DataType getDataType = db.Get<DataType>(id);
                Assert.NotNull(getDataType);
            }
        }

        [Test]

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
                Assert.NotNull(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    string id = dr.GetValue<string>("Id");
                    int age = dr.GetValue<int>("Age");
                }
            }
        }

        [Test]
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
                Assert.NotNull(entity2);
                entity2.Name = "tom";
                entity2.LastModifyTime = DateTime.Now;
                bool flag2 = db.Update<RoleEntity>(entity2);
                Assert.IsTrue(flag2);

                RoleEntity entity3 = db.Get<RoleEntity>(entity1.Id);
                Assert.NotNull(entity3);
                Assert.AreEqual(entity2.Name, entity3.Name);
                Assert.AreEqual(entity1.Version + 1, entity3.Version);
            }
        }

        [Test]
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

        [Test]
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
                Assert.NotNull(result);
            }
        }

        [Test]
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
                Assert.NotNull(result);
                Assert.IsTrue(result.Count >= 1);
                int totalRecords = parameter.Get<int>("TotalRecords");
            }
        }

        [Test]
        [ExpectedException]
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

        [Test]
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
                Assert.NotNull(entity2);
                Assert.AreEqual(true, entity2.IsPersisted);
                entity2.Name = "tom";
                entity2.LastModifyTime = DateTime.Now;
                bool flag2 = db.Save<RoleEntity>(entity2);
                Assert.IsTrue(flag2);

                RoleEntity entity3 = db.Get<RoleEntity>(entity1.Id);
                Assert.NotNull(entity3);
                Assert.AreEqual(true, entity3.IsPersisted);
                Assert.AreEqual(entity2.Name, entity3.Name);
                Assert.AreEqual(entity1.Version + 1, entity3.Version);
            }
        }


        [Test]
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
    }
}
