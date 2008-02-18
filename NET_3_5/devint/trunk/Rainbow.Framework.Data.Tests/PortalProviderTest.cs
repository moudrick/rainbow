using Rainbow.Framework.Data.DataSources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rainbow.Framework.Data.Entities;
using System.Collections.Generic;
using System;

namespace Rainbow.Framework.Data.Tests
{
    
    
    /// <summary>
    ///This is a test class for PortalProviderTest and is intended
    ///to contain all PortalProviderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PortalProviderTest
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
            PortalProvider target = CreatePortalProvider(); // TODO: Initialize to an appropriate value
            IPortal Portal = null; // TODO: Initialize to an appropriate value
            target.Update(Portal);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Remove
        ///</summary>
        [TestMethod()]
        public void RemoveTest()
        {
            PortalProvider target = CreatePortalProvider(); // TODO: Initialize to an appropriate value
            IPortal Portal = null; // TODO: Initialize to an appropriate value
            target.Remove(Portal);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Instance
        ///</summary>
        [TestMethod()]
        public void InstanceTest()
        {
            PortalProvider expected = null; // TODO: Initialize to an appropriate value
            PortalProvider actual;
            actual = PortalProvider.Instance();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetById
        ///</summary>
        [TestMethod()]
        public void GetByIdTest()
        {
            PortalProvider target = CreatePortalProvider(); // TODO: Initialize to an appropriate value
            Guid Id = new Guid(); // TODO: Initialize to an appropriate value
            IPortal expected = null; // TODO: Initialize to an appropriate value
            IPortal actual;
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
            PortalProvider target = CreatePortalProvider(); // TODO: Initialize to an appropriate value
            IEnumerable<IPortal> expected = null; // TODO: Initialize to an appropriate value
            IEnumerable<IPortal> actual;
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
            PortalProvider target = CreatePortalProvider(); // TODO: Initialize to an appropriate value
            IPortal expected = null; // TODO: Initialize to an appropriate value
            IPortal actual;
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
            PortalProvider target = CreatePortalProvider(); // TODO: Initialize to an appropriate value
            target.CommitChanges();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        internal virtual PortalProvider CreatePortalProvider()
        {
            // TODO: Instantiate an appropriate concrete class.
            PortalProvider target = null;
            return target;
        }

        /// <summary>
        ///A test for Add
        ///</summary>
        [TestMethod()]
        public void AddTest()
        {
            PortalProvider target = CreatePortalProvider(); // TODO: Initialize to an appropriate value
            IPortal newPortal = null; // TODO: Initialize to an appropriate value
            IPortal newPortalExpected = null; // TODO: Initialize to an appropriate value
            target.Add(newPortal);
            Assert.AreEqual(newPortalExpected, newPortal);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
