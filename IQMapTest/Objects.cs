﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = NUnit.Framework.Assert;
using IQMapTest.Mocks;
using IQMap;
using IQMap.Implementation;

namespace IQMapTest
{
    [TestClass]
    public class Objects
    {
        [TestMethod]
        public void Basic()
        {
            //Assert.Throws<Exception>(() => {
            //    TestObjectBugs testBugs = new TestObjectBugs();
                
            //}, "Can't create instance of primary-keyless object");

            var test = new TestObject();
            Assert.IsFalse(test.DBData().Initialized, "New object isn't initialized yet (lazy dirty tracking coping works)");
            Assert.IsFalse(test.IsDirty(), "New object isn't dirty");
            Assert.IsTrue(test.IsNew(), "New object is new");

            test.FirstName = "Jamie";
            Assert.IsTrue(test.IsDirty("FirstName"), "Changing name dirtied it");
            Assert.IsTrue(test.IsDirty(), "Changing a field dirtied the whole object");

            int dirtyCount=0;
            foreach (var item in test.DirtyFieldNames()) {
                dirtyCount++;
                Assert.AreEqual("FirstName",item,"The field that's dirty is firstname");
            }
            Assert.AreEqual(1, dirtyCount, "There was one dirty field");


        }

        [TestMethod]
        public void DefaultType()
        {

            var test = new TestObjectDefault();
            var data = IQ.DBData(test);

            Assert.IsTrue(data.ClassInfo.PrimaryKey.Name == "DefaultPK");
            Assert.AreEqual(data.ClassInfo.FieldNames.Count, 6, "Has five fields");

            data.SetPrimaryKey(123);
            Assert.AreEqual(123, test.DefaultPK, "PK was set");
       }
        [TestMethod]
        public void DefaultTypeIgnore()
        {

            var test = new TestObjectDefaultIgnore();
            var data = IQ.DBData(test);

            Assert.AreEqual(data.ClassInfo.FieldNames.Count, 5, "Has five fields");
        }
        [TestMethod]

        public void LoadQuery()
        {


            var test = IQ.Query(@"select 1 as PK, 'jamie' as firstname, cast('1/1/2012 12:12:00' as datetime) as updatedate, 
                14 as somenumber, 122.29 as howmuch").MapNext<TestObject>();

            Assert.AreNotEqual(default(int), test.PK);
            Assert.AreNotEqual(default(string), test.FirstName);
            Assert.AreNotEqual(default(DateTime), test.UpdateDate);
            Assert.AreNotEqual(default(long), test.SomeNumber);
            Assert.AreNotEqual(default(float), test.HowMuch);
        }
        [TestMethod]

        // See notes:  http://stackoverflow.com/q/8593871
        // Our implementation is different. We only deal with properties having a public getter (unless explicitly marked)
            
        public void TestAbstractInheritance()
        {
            var order = IQ.Connection.Load<ConcreteOrder>("select 1 Internal,2 ProtectedSet,3 [Public],4 Concrete,5 PrivateGetSet, 6 ProtectedGet");

            Assert.AreEqual(0, order.Internal);
            Assert.AreEqual(2, order.ProtectedVal);
            Assert.AreEqual(3, order.Public);
            Assert.AreEqual(4, order.Concrete);
            Assert.AreEqual(0, order.PrivateGetSetVal);
            Assert.AreEqual(0, order.ProtectedGetVal);

        }
    }
}