/*********************************************************
 *
 *  Author:        Uwe Lesta
 *  Copyright (C): 2008-2014, Uwe Lesta SBS-Softwaresysteme GmbH
 *
 *  Unit-Tests for the interface from C# to Swi-Prolog - SwiPlCs
 *
 *  This library is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 2.1 of the License, or (at your option) any later version.
 *
 *  This library is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public
 *  License along with this library; if not, write to the Free Software
 *  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
 *
 *********************************************************/

using System.Reflection;
using SbsSW.SwiPlCs;

namespace TestSwiPl
{
    /// <summary>
    /// Summary description for BasePlInit
    /// </summary>
    public class BasePlInit
    {


        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes

        static private readonly String[] EmptyParam = { "-q" };  // suppressing informational and banner messages
        //        static public String[] EmptyParam = { "-nosignals" };
        //        static public String[] EmptyParam = { "--quit"};
        static private void InitializePlEngine()
        {
            if (!PlEngine.IsInitialized)
            {
                PlEngine.Initialize(EmptyParam);
            }
        }

        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}

        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}

        //Use TestInitialize to run code before running each test
        [SetUp]
        virtual public void MyTestInitialize()
        {
            InitializePlEngine();
        }

        //Use TestCleanup to run code after each test has run
        [TearDown]
        virtual public void MyTestCleanup()
        {
            PlEngine.PlCleanup();
        }
        #endregion


        #region helper

        protected MethodInfo PrivateObject(object sut, string methodName)
        {
            if (string.IsNullOrWhiteSpace(methodName))
                Assert.Fail("methodName cannot be null or whitespace");

            var method = sut.GetType()
                .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (method == null)
                Assert.Fail(string.Format("{0} method not found", methodName));

            return method;
        }
        
        protected int list_length(PlTerm list)
        {
            var listLen = PlTerm.PlVar();
            var args = new PlTermV(list, listLen);
            Assert.IsTrue(PlQuery.PlCall("length", args));
            return (int)args[1];
        }
        #endregion




    }
}
