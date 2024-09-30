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

using SbsSW.SwiPlCs;
using SbsSW.SwiPlCs.Exceptions;

namespace TestSwiPl
{
	/// <summary>
	/// TestF�lle zu 'SWI-cs' dem SWI prolog interface in CSharp
	/// </summary>
    public class TestPlException : BasePlInit
	{
        /// <summary>
        /// Sample from the documentation PlFrame
        /// </summary>
        [Test]
        public void KomplexPlException()
		{
            const int size = 4;
            const int index = 6;
            PlTerm term = PlTerm.PlCompound("error",
                            new PlTermV(PlTerm.PlCompound("domain_error",
                                                    new PlTermV(PlTerm.PlCompound("argv", new PlTermV(size)), new PlTerm(index))
			                                ),
	        		        PlTerm.PlVar())
			                );
            var ex = new PlException(term);
            //Assert.AreEqual("Domain error: `argv(_G1, _G2, _G3, _G4)' expected, found `6'", ex.Message);
            // since swi-prolog version 5.7.6
            // Assert.AreEqual("Domain error: `argv(_G60, _G61, _G62, _G63)' expected, found `6'", ex.Message);
            // since swi-prolog version 5.7.10
            // Assert.AreEqual("Domain error: `argv(_G1, _G2, _G3, _G4)' expected, found `6'", ex.Message);
            // since swi-prolog version 5.9.10
           
#if _PL_X64
            Assert.AreEqual("Domain error: `argv(_G53,_G54,_G55,_G56)' expected, found `6'", ex.Message);
#else
            Assert.AreEqual("Domain error: `argv(_G1,_G2,_G3,_G4)' expected, found `6'", ex.Message);
#endif

        }
        
        
        #region prolog throw c# catch Samples _doc

        [Test]
        #region prolog_exception_sample_doc
        public void prolog_exception_sample()
        {
            const string exceptionText = "test_exception";
            Assert.IsTrue(PlQuery.PlCall("assert( (test_throw :- throw(" + exceptionText + ")) )"));
            try
            {
                Assert.IsTrue(PlQuery.PlCall("test_throw"));
            }
            catch (PlException ex)
            {
                Assert.AreEqual(exceptionText, ex.Term.ToString());
                Assert.AreEqual("Unknown message: " + exceptionText, ex.Message);
            }
        }
        #endregion prolog_exception_sample_doc

        [Test]
        #region prolog_type_exception_sample_doc
        public void prolog_type_exception_sample()
        {
            try
            {
                Assert.IsTrue(PlQuery.PlCall("sumlist([1,error],L)"));
            }
            catch (PlTypeException ex)
            {
                Assert.AreEqual("is/2: Arithmetic: `error/0' is not a function", ex.Message);
            }
        }
        #endregion prolog_type_exception_sample_doc

        [Test]
        #region prolog_domain_exception_sample_doc
        public void prolog_domain_exception_sample()
        {
            try
            {
                Assert.IsTrue(PlQuery.PlCall("open(temp_kill, nonsens, F)"));
            }
            catch (PlDomainException ex)
            {
                Assert.AreEqual("open/3: Domain error: `io_mode' expected, found `nonsens'", ex.Message);
            }
        }
        #endregion prolog_domain_exception_sample_doc


        #endregion prolog throw c# catch


        [Test]
        public void t_prolog_init_exception()
        {
            PlEngine.PlCleanup();
            Assert.IsFalse(PlEngine.IsInitialized);
            // this throw a PlLibException
            String[] param = { "-q", "-g", "member(A,[a," };  // -q suppressing informational and banner messages
            try
            {
                PlEngine.Initialize(param);
                Assert.Fail();
            }
            catch (PlLibException ex)
            {
                // It would be nice to have something that throw a PlException on initialitzation
                Assert.AreEqual("failed to initialize", ex.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail("Wrong Exception: " + ex.Message);
            }
            Assert.IsTrue(PlEngine.IsInitialized);
        }


        #region prolog throw c# catch

        [Test]
        public void t_prolog_exception()
        {
            Assert.IsTrue(PlQuery.PlCall("assert( (test_throw :- throw(error)) )"));
            Assert.Throws<PlException>(() => PlQuery.PlCall("test_throw"),"Unknown message: error" );
        }

        [Test]
        public void t_prolog_exception_2()
        {
            Assert.Throws<PlTypeException>(() =>PlQuery.PlCall("sumlist([1,error],L)"), "is/2: Arithmetic: `error/0' is not a function");
        }

        [Test]
        public void t_prolog_exception_3()
        {
            Assert.Throws<PlDomainException>(() => PlQuery.PlCall("open(temp_kill, nonsens, F)"), "open/3: Domain error: `io_mode' expected, found `nonsens'");
        }


        [Test]
        // [ExpectedException(typeof(PlTypeException), "`list' expected, found `[a,b,c]'")]
        public void TestException_in_a_query_2()
        {
            var plq = new PlQuery("atomic_list_concat(L, A)");
            Assert.IsTrue(plq.Variables["L"].Unify("[a,b,c]"));
            //Assert.IsTrue(plq.Variables["L"].Unify(new PlTerm("[a,b,c]")));
            foreach (PlQueryVariables vars in plq.SolutionVariables)
            {
                Assert.AreEqual("abc", vars["A1"].ToString());
            }
        }


        #endregion prolog throw c# catch


    } // test class T_PlException
}
