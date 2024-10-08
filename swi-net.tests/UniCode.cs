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
using System.Text;

namespace TestSwiPl
{

    /// <summary>
    /// TestFälle zu 'SWI-cs' dem SWI prolog interface in CSharp
    /// Unicode
    /// This file is stored in UniCode
    /// </summary>
    public class TestUniCode : BasePlInit
    {

        #region SetUp & TearDown

        PlFrame _frame;

        static string _plFilenameUtf8;
        static string _plFilenameUnicode;
        const string StrUnicode = "ąę eşek";
        const string StrLatin1 = "öäüß";
        const string StrAscii = "abc";

        static string CreateTmpFile(Encoding encoding)
        {
            var plFilename = Path.GetTempFileName();
            plFilename = plFilename.Replace('\\', '/');
            using (var writer = new StreamWriter(plFilename, false, encoding))
            {
                writer.WriteLine("unicode('" + StrUnicode + "').");
                writer.WriteLine("latin1('" + StrLatin1 + "').");
                writer.WriteLine("ascii('" + StrAscii + "').");
                writer.WriteLine("noun(ş, aa).");
                writer.WriteLine("noun(ş, bb).");
            }
            return plFilename;
        }

        //Use ClassInitialize to run code before running the first test in the class
        [OneTimeSetUp]
        public static void MyClassInitialize()
        {
            _plFilenameUtf8 = CreateTmpFile(Encoding.UTF8);
            _plFilenameUnicode = CreateTmpFile(Encoding.Unicode);
        }

        //Use ClassCleanup to run code after all tests in a class have run
        [OneTimeTearDown]
        public static void MyClassCleanup()
        {
            File.Delete(_plFilenameUtf8);
            File.Delete(_plFilenameUnicode);
        }


        [SetUp]	// befor each test
        override public void MyTestInitialize()
        {
            base.MyTestInitialize();
            // mit pl_frame müssen alle queryes (die pl_wchar_to_term benutzen) in ein using bzw. dispose aufrufen
            _frame = new PlFrame();
        }

        [TearDown]	// after each test
        override public void MyTestCleanup()
        {
            _frame.Dispose();
            _frame = null;
            base.MyTestCleanup();
        }
        #endregion


        [Test]
        public void Sample()
        {
            const string unicodeString = "This string contains the unicode character Pi(\u03a0)-  \u0501\u0119   'ąę' noun(eşek)";

            // Create two different encodings.
            Encoding utf8 = Encoding.UTF8;
            Encoding unicode = Encoding.Unicode;

            // Convert the string into a byte[].
            byte[] unicodeBytes = unicode.GetBytes(unicodeString);

            // Perform the conversion from one encoding to the other.
            byte[] utf8Bytes = Encoding.Convert(unicode, utf8, unicodeBytes);

            // Convert the new byte[] into a char[] and then into a string.
            // This is a slightly different approach to converting to illustrate
            // the use of GetCharCount/GetChars.
            var utf8Chars = new char[utf8.GetCharCount(utf8Bytes, 0, utf8Bytes.Length)];
            utf8.GetChars(utf8Bytes, 0, utf8Bytes.Length, utf8Chars, 0);
            var utf8String = new string(utf8Chars);

            // Display the strings created before and after the conversion.
            System.Diagnostics.Trace.WriteLine(String.Format("Original string: {0}", unicodeString) + "\n");
            System.Diagnostics.Trace.WriteLine(String.Format("Ascii converted string: {0}", utf8String) + "\n");
            Assert.AreEqual(unicodeString, utf8String);
        }





        [Test]
        public void CharSetTm()
        {
            var t1 = new PlTerm("'®'");
            Assert.AreEqual("®", t1.ToString());
        }

        [Test]
        public void CharSetEuro()
        {
            var t1 = new PlTerm("'€'");
            Assert.AreEqual("€", t1.ToString());
        }
        [Test]
        public void CharSetEuroEuro()
        {
            var t1 = new PlTerm("'€€'");
            Assert.AreEqual("€€", t1.ToString());
        }

        [Test]
        public void CharSet1()
        {
            var t1 = new PlTerm("'ÄÖÜ'");
            Assert.AreEqual("ÄÖÜ", t1.ToString());
        }

        // test if the characters are read _from_ prolog correct
        #region query prolog from utf8 and unicode file (consult)
        [Test]
        public void QueryPrologUnicode()
        {
            PlQuery.PlCall("consult('" + _plFilenameUtf8 + "')");
            PlTerm t = PlQuery.PlCallQuery("unicode(L)");
            Assert.AreEqual(StrUnicode, t.ToString());
        }
        [Test]
        public void QueryPrologLatin1()
        {
            PlQuery.PlCall("consult('" + _plFilenameUtf8 + "')");
            PlTerm t = PlQuery.PlCallQuery("latin1(L)");
            Assert.AreEqual(StrLatin1, t.ToString());
        }
        [Test]
        public void QueryPrologAscii()
        {
            PlQuery.PlCall("consult('" + _plFilenameUtf8 + "')");
            PlTerm t = PlQuery.PlCallQuery("ascii(L)");
            Assert.AreEqual(StrAscii, t.ToString());
        }
        [Test]
        public void QueryPrologUnicodeSolutions()
        {
            PlQuery.PlCall("consult('" + _plFilenameUtf8 + "')");
            var v = PlTerm.PlVar();
            var q = new PlQuery("noun", new PlTermV(new PlTerm("'ş'"), v));
            Assert.IsTrue(q.NextSolution());
            Assert.AreEqual("aa", v.ToString());
            Assert.IsTrue(q.NextSolution());
            Assert.AreEqual("bb", v.ToString());
            q.Dispose();
        }


        [Test]
        public void QueryPrologUnicode2()
        {
            PlQuery.PlCall("consult('" + _plFilenameUnicode + "')");
            PlTerm t = PlQuery.PlCallQuery("unicode(L)");
            Assert.AreEqual(StrUnicode, t.ToString());
        }
        [Test]
        public void QueryPrologLatin12()
        {
            PlQuery.PlCall("consult('" + _plFilenameUnicode + "')");
            PlTerm t = PlQuery.PlCallQuery("latin1(L)");
            Assert.AreEqual(StrLatin1, t.ToString());
        }
        [Test]
        public void QueryPrologAscii2()
        {
            PlQuery.PlCall("consult('" + _plFilenameUnicode + "')");
            PlTerm t = PlQuery.PlCallQuery("ascii(L)");
            Assert.AreEqual(StrAscii, t.ToString());
        }
        [Test]
        public void QueryPrologUnicodeSolutions2()
        {
            PlQuery.PlCall("consult('" + _plFilenameUnicode + "')");
            var v = PlTerm.PlVar();
            var q = new PlQuery("noun", new PlTermV(new PlTerm("'ş'"), v));
            Assert.IsTrue(q.NextSolution());
            Assert.AreEqual("aa", v.ToString());
            Assert.IsTrue(q.NextSolution());
            Assert.AreEqual("bb", v.ToString());
            q.Dispose();
        }
        #endregion


        #region chars to prolog
        // test if the character are transmitted correct _to_prolog
        [Test]
        public void PlCallToPrologUnicode()
        {
            PlQuery.PlCall("consult('" + _plFilenameUtf8 + "')");
            Assert.IsTrue(PlQuery.PlCall("unicode('" + StrUnicode + "')"));
        }

        [Test]
        public void PlCallToPrologLatin1()
        {
            PlQuery.PlCall("consult('" + _plFilenameUtf8 + "')");
            Assert.IsTrue(PlQuery.PlCall("latin1(" + StrLatin1 + ")"));
        }

        [Test]
        public void PlCallToPrologAscii()
        {
            PlQuery.PlCall("consult('" + _plFilenameUtf8 + "')");
            Assert.IsTrue(PlQuery.PlCall("ascii(" + StrAscii + ")"));
        }

        #endregion


        #region new PlTerm and ToString()
        [Test]
        public void PlTermToStringUnicode()
        {
            var t1 = new PlTerm("'" + StrUnicode + "'");
            Assert.AreEqual(StrUnicode, t1.ToString());
        }

        [Test]
        public void PlTermToStringLatin1()
        {
            var t1 = new PlTerm("'" + StrLatin1 + "'");
            Assert.AreEqual(StrLatin1, t1.ToString());
        }

        [Test]
        public void PlTermToStringAscii()
        {
            var t1 = new PlTerm("'" + StrAscii + "'");
            Assert.AreEqual(StrAscii, t1.ToString());
        }
        #endregion

        [Test]
        public void CharSet_3()
        {
            var t2 = new PlTerm("'ąę'");
            var t1 = new PlTerm(@"'\u0105\u0119'");
            Assert.IsTrue(t1.Unify(t2));
        }

        [Test]
        public void compound_name()
        {
            var t2 = new PlTerm("ąę(eşek)");
            Assert.AreEqual("ąę", t2.Name);
        }

        [Test]
        public void PlString()
        {
            var t = PlTerm.PlString("ąęeşek", 2);
            Assert.AreEqual("ąę", t.ToString());
        }


        #region Queries


        [Test]
        public void unicode_tuerkisch()  // from Önder ("noun(eşek)");"
        {
            PlQuery.PlCall(@"assert(noun('eşek'))");
            Assert.IsTrue(PlQuery.PlCall("noun('eşek')"));
        }

        [Test]
        public void unicode_0()
        {
            PlQuery.PlCall(@"assert(n('\u0105\u0119'))");
            var q = new PlQuery("n(X), name(X,Y)");
            Assert.IsTrue(q.NextSolution());
            Assert.AreEqual("[261,281]", q.Variables["Y"].ToString());
            q.Dispose();
        }


        [Test]
        public void unicode_0_using()
        {
            PlQuery.PlCall(@"assert(n('\u0105\u0119'))");
            using (var q = new PlQuery("n(X), name(X,Y)"))
            {
                Assert.IsTrue(q.NextSolution());
                Assert.AreEqual("[261,281]", q.Variables["Y"].ToString());
            }
        }


        [Test]
        public void unicode_01()
        {
            PlQuery.PlCall(@"assert(n('\u0105\u0119'))");
            using (var q = new PlQuery("n(X)"))
            {
                Assert.IsTrue(q.NextSolution());
                Assert.AreEqual("ąę", q.Variables["X"].ToString());
            }
        }


        [Test]
        public void unicode_1()
        {
            PlQuery.PlCall("assert(n('ąę'))");
            using (var q = new PlQuery("n(X), name(X,Y)"))
            {
                Assert.IsTrue(q.NextSolution());
                Assert.AreEqual("[261,281]", q.Variables["Y"].ToString());
            }
        }


        [Test]
        public void unicode_2()
        {
            PlQuery.PlCall("assert(n('ąę'))");
            var q = new PlQuery("n(X)");
            Assert.IsTrue(q.NextSolution());
            Assert.AreEqual("ąę", q.Variables["X"].ToString());
            q.Dispose();
            var q2 = new PlQuery(@"n('\u0105\u0119')");
            Assert.IsTrue(q2.NextSolution());
            q2.Dispose();
        }


        [Test]
        public void unicode_query_term_oender()
        {
            PlQuery.PlCall(@"assert(noun(ş, myChar))");
            var v = PlTerm.PlVar();
            var q = new PlQuery("noun", new PlTermV(new PlTerm("ş"),v));
            Assert.IsTrue(q.NextSolution());
            Assert.AreEqual("myChar", v.ToString());
            q.Dispose();
        }

        [Test]
        public void unicode_query_atom_oender()
        {
            PlQuery.PlCall(@"assert(noun('ş', myChar))");
            var v = PlTerm.PlVar();
            var q = new PlQuery("noun", new PlTermV(new PlTerm("'ş'"),v));
            Assert.IsTrue(q.NextSolution());
            Assert.AreEqual("myChar", v.ToString());
            q.Dispose();
        }

        [Test]
        public void unicode_query_string_atom_oender()
        {
            PlQuery.PlCall(@"assert(noun('ş',myChar))");
            var q = new PlQuery("noun('ş',C)");
            Assert.IsTrue(q.NextSolution());
            Assert.AreEqual("myChar", q.Variables["C"].ToString());
            q.Dispose();
        }

        [Test]
        public void unicode_query_string_term_oender()
        {
            PlQuery.PlCall(@"assert(noun(ş,myChar))");
            var q = new PlQuery("noun(ş,C)");
            Assert.IsTrue(q.NextSolution());
            Assert.AreEqual("myChar", q.Variables["C"].ToString());
            q.Dispose();
        }



        #endregion Queries



    } // TestUniCode 
}
