using Rainbow.Framework.Data.DataSources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rainbow.Framework.Data.Entities;
using System.Collections.Generic;
using System;

namespace Rainbow.Framework.Data.Tests
{
    
    
    /// <summary>
    ///This is a test class for PageProviderTest and is intended
    ///to contain all PageProviderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PageProviderTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Update
        ///</summary>
        [TestMethod()]
        public void UpdateTest()
        {
            PageProvider target = CreatePageProvider(); // TODO: Initialize to an appropriate value
            IPage Page = null; // TODO: Initialize to an appropriate value
            target.Update(Page);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Remove
        ///</summary>
        [TestMethod()]
        public void RemoveTest()
        {
            PageProvider target = CreatePageProvider(); // TODO: Initialize to an appropriate value
            IPage Page = null; // TODO: Initialize to an appropriate value
            target.Remove(Page);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for GetById
        ///</summary>
        [TestMethod()]
        public void GetByIdTest()
        {
            PageProvider target = CreatePageProvider(); // TODO: Initialize to an appropriate value
            Guid Id = new Guid(); // TODO: Initialize to an appropriate value
            IPage expected = null; // TODO: Initialize to an appropriate value
            IPage actual;
            actual = target.GetById(Id);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetAll
        ///</summary>
        [TestMethod()]
        public void GetAllTest()
        {
            PageProvider target = CreatePageProvider(); // TODO: Initialize to an appropriate value
            IEnumerable<IPage> expected = null; // TODO: Initialize to an appropriate value
            IEnumerable<IPage> actual;
            actual = target.GetAll();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreateNew
        ///</summary>
        [TestMethod()]
        public void CreateNewTest()
        {
            PageProvider target = CreatePageProvider(); // TODO: Initialize to an appropriate value
            IPage expected = null; // TODO: Initialize to an appropriate value
            IPage actual;
            actual = target.CreateNew();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CommitChanges
        ///</summary>
        [TestMethod()]
        public void CommitChangesTest()
        {
            PageProvider target = CreatePageProvider(); // TODO: Initialize to an appropriate value
            target.CommitChanges();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        internal virtual PageProvider CreatePageProvider()
        {
            // TODO: Instantiate an appropriate concrete class.
            PageProvider target = null;
            return target;
        }

        /// <summary>
        ///A test for Add
        ///</summary>
        [TestMethod()]
        public void AddTest()
        {
            PageProvider target = CreatePageProvider(); // TODO: Initialize to an appropriate value
            IPage newPage = null; // TODO: Initialize to an appropriate value
            IPage newPageExpected = null; // TODO: Initialize to an appropriate value
            target.Add(newPage);
            Assert.AreEqual(newPageExpected, newPage);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Instance
        ///</summary>
        [TestMethod()]
        public void InstanceTest()
        {
            PageProvider expected = null; // TODO: Initialize to an appropriate value
            PageProvider actual;
            actual = PageProvider.Instance();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
