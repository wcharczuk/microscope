using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Microscope.Test
{
    public class QueryTests
    {
        [Fact]
        public void Parse_NoLogic_Test()
        {
            var query = "matches('(www.clotheshor.se)')";
            var microscope = new QueryEvaluator();

            var query_tree = microscope.Parse(query);
            Assert.NotNull(query_tree);

            microscope.ParseAndCompile(query);

            Assert.True(microscope.Evaluate("http://www.clotheshor.se/product"));
            Assert.False(microscope.Evaluate("http://www.google.com/product"));
        }

        [Fact]
        public void Parse_Numbers_Test()
        {
            var query = "matches('[0-9]+')";
            var microscope = new QueryEvaluator();

            var query_tree = microscope.Parse(query);
            Assert.NotNull(query_tree);

            microscope.ParseAndCompile(query);

            Assert.False(microscope.Evaluate("http://www.clotheshor.se/product"));
            Assert.False(microscope.Evaluate("http://www.google.com/product"));
            Assert.True(microscope.Evaluate("http://www.clotheshor.se/admin/customer/50"));
        }

        [Fact]
        public void Parse_BareUnary_Test()
        {
            var query = "not isEmpty()";
            var microscope = new QueryEvaluator();

            var query_tree = microscope.Parse(query);
            Assert.NotNull(query_tree);

            microscope.ParseAndCompile(query);

            Assert.True(microscope.Evaluate("http://www.clotheshor.se/product"));
            Assert.True(microscope.Evaluate("http://www.google.com/product"));
            Assert.False(microscope.Evaluate(""));
        }

        [Fact]
        public void Parse_Basic_Test()
        {
            var query = "matches('(www.clotheshor.se)') and not contains('admin')";
            var microscope = new QueryEvaluator();

            var query_tree = microscope.Parse(query);
            Assert.NotNull(query_tree);

            microscope.ParseAndCompile(query);

            Assert.True(microscope.Evaluate("http://www.clotheshor.se/product"));
            Assert.False(microscope.Evaluate("http://www.clotheshor.se/admin"));
            Assert.False(microscope.Evaluate("http://www.google.com/product"));
        }

        [Fact]
        public void Parse_Blocks_Test()
        {
            var query = "(  matches('(www.clotheshor.se)') and contains('product')  ) and not contains('admin')";
            var microscope = new QueryEvaluator();

            var query_tree = microscope.Parse(query);
            Assert.NotNull(query_tree);

            microscope.ParseAndCompile(query);
            Assert.True(microscope.Evaluate("http://www.clotheshor.se/product"));
            Assert.False(microscope.Evaluate("http://www.clotheshor.se/admin/product"));
            Assert.False(microscope.Evaluate("http://google.com/admin/product"));
            Assert.False(microscope.Evaluate("http://google.com/product"));
        }

        [Fact]
        public void Parse_MultipleBlocks_Test()
        {
            var query = "(matches('(www.clotheshor.se)') and contains('product')) and not (contains('admin') or contains('team'))";
            var microscope = new QueryEvaluator();

            var query_tree = microscope.Parse(query);
            Assert.NotNull(query_tree);

            microscope.ParseAndCompile(query);
            Assert.True(microscope.Evaluate("http://www.clotheshor.se/product"));
            Assert.True(microscope.Evaluate("http://www.clotheshor.se/test/product"));
            Assert.False(microscope.Evaluate("http://www.clotheshor.se/admin/product"));
            Assert.False(microscope.Evaluate("http://www.clotheshor.se/team"));
            Assert.False(microscope.Evaluate("http://www.clotheshor.se/product/team"));
            Assert.False(microscope.Evaluate("http://google.com/admin/product"));
            Assert.False(microscope.Evaluate("http://google.com/product"));
        }

        [Fact]
        public void Parse_InvalidSyntax_Test()
        {
            var microscope = new QueryEvaluator();

            var did_throw = false;
            try
            {
                var query_tree = microscope.Parse("(matches('(.*)') and contains('hello')"); //missing brace on block
            }
            catch (ParserException)
            {
                did_throw = true;
            }
            Assert.True(did_throw);

            did_throw = false;
            try
            {
                var query_tree = microscope.Parse("contains('TEST'"); //missing brace on function
            }
            catch (ParserException)
            {
                did_throw = true;
            }
            Assert.True(did_throw);

            did_throw = false;
            try
            {
                var query_tree = microscope.Parse("somefunctionthatdoesntexist('hello')"); //invalid function
            }
            catch (ParserException)
            {
                did_throw = true;
            }

            Assert.True(did_throw);

            did_throw = false;
            try
            {
                var query_tree = microscope.Parse("matches ('(this?)')"); //invalid whitespace whoopsie.
            }
            catch (ParserException)
            {
                did_throw = true;
            }

            Assert.True(did_throw);

            did_throw = false;
            try
            {
                var query_tree = microscope.Parse("matches(THIS)"); //invalid no quotes around argument
            }
            catch (ParserException)
            {
                did_throw = true;
            }

            Assert.True(did_throw);
        }

        [Fact]
        public void Compile_Test()
        {
            var node = new BinaryNode()
            {
                ExpressionType = System.Linq.Expressions.ExpressionType.AndAlso
                ,
                Left = new CallNode() { MethodName = "_matches", Argument = "(www.clotheshor.se)" }
                ,
                Right = new UnaryNode()
                {
                    Node = new CallNode()
                    {
                        MethodName = "_contains"
                        ,
                        Argument = "admin"
                    }
                }
            };

            var microscope = new QueryEvaluator();
            var fun = microscope.Compile(node);

            var should_be_true = fun("http://www.clotheshor.se/team");
            var should_be_false = fun("http://www.clotheshor.se/admin");
            var should_also_be_false = fun("http://google.com");

            Assert.True(should_be_true);

            Assert.False(should_be_false);
            Assert.False(should_also_be_false);
        }

        [Fact]
        public void Performance_Test()
        {
            var query = "(matches('(www.clotheshor.se)') and contains('product')) and not (contains('admin') or contains('team'))";

            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            for (int x = 0; x < 1024; x++)
            {
                var scope1 = new QueryEvaluator();
                scope1.ParseAndCompile(query);
                var result = scope1.Evaluate("http://www.clotheshor.se/product");
            }
            sw.Stop();

            var recompile_time = sw.ElapsedMilliseconds;

            sw.Reset();

            sw.Start();
            var scope2 = new QueryEvaluator();
            scope2.ParseAndCompile(query);
            for (int x = 0; x < 1024; x++)
            {
                var result = scope2.Evaluate("http://www.clotheshor.se/product");
            }
            sw.Stop();

            var precompile_time = sw.ElapsedMilliseconds;

            Assert.True(recompile_time > precompile_time, "Recompiling every time should be much slower.");
        }

        [Fact]
        public void Matches_Test()
        {
            var q = new QueryEvaluator("matches('[0-9]+')");

            Assert.True(q.Evaluate("09091230"));
            Assert.False(q.Evaluate("this should be false"));

            var q2 = new QueryEvaluator("matches('(TEST)')");
            Assert.True(q2.Evaluate("TEST"));
            Assert.False(q2.Evaluate("test"));
        }

        [Fact]
        public void Matchesi_Test()
        {
            var q = new QueryEvaluator("matchesi('(TEST)')");

            Assert.True(q.Evaluate("TEST"));
            Assert.True(q.Evaluate("test"));
            Assert.False(q.Evaluate("not_that_word"));
        }

        [Fact]
        public void Contains_Test()
        {
            var q = new QueryEvaluator("contains('word')");

            Assert.True(q.Evaluate("there is a word"));
            Assert.False(q.Evaluate("there is not"));
            Assert.False(q.Evaluate("there is not a WORD"));
        }

        [Fact]
        public void Containsi_Test()
        {
            var q = new QueryEvaluator("containsi('word')");

            Assert.True(q.Evaluate("there is a word"));
            Assert.False(q.Evaluate("there is not"));
            Assert.True(q.Evaluate("there is not a WORD"));
        }

        [Fact]
        public void StartsWith_Test()
        {
            var q = new QueryEvaluator("startswith('word')");

            Assert.True(q.Evaluate("word is a bird"));
            Assert.False(q.Evaluate("there is not a word"));
            Assert.False(q.Evaluate("WORD is a bird"));
        }

        [Fact]
        public void StartsWithi_Test()
        {
            var q = new QueryEvaluator("startswithi('word')");

            Assert.True(q.Evaluate("word is a bird"));
            Assert.False(q.Evaluate("there is not a word"));
            Assert.True(q.Evaluate("WORD is a bird"));
        }

        [Fact]
        public void EndsWith_Test()
        {
            var q = new QueryEvaluator("endswith('bird')");

            Assert.True(q.Evaluate("word is a bird"));
            Assert.False(q.Evaluate("bird is not a word"));
            Assert.False(q.Evaluate("word is a BIRD"));
        }

        [Fact]
        public void EndsWithi_Test()
        {
            var q = new QueryEvaluator("endswithi('bird')");

            Assert.True(q.Evaluate("word is a bird"));
            Assert.False(q.Evaluate("bird is not a word"));
            Assert.True(q.Evaluate("word is a BIRD"));
        }

        [Fact]
        public void Equals_Test()
        {
            var q = new QueryEvaluator("equals('test')");

            Assert.True(q.Evaluate("test"));
            Assert.False(q.Evaluate("not test"));
            Assert.False(q.Evaluate("TEST"));
        }

        [Fact]
        public void Equalsi_Test()
        {
            var q = new QueryEvaluator("equalsi('test')");

            Assert.True(q.Evaluate("test"));
            Assert.False(q.Evaluate("not test"));
            Assert.True(q.Evaluate("TEST"));
        }

        [Fact]
        public void IsEmpty_Test()
        {
            var q = new QueryEvaluator("isempty()");

            Assert.True(q.Evaluate(""));
            Assert.False(q.Evaluate("not empty"));
        }

        [Fact]
        public void And_Test()
        {
            var q = new QueryEvaluator("contains('test') and contains('word')");

            Assert.True(q.Evaluate("test word"));
            Assert.False(q.Evaluate("word"));
            Assert.False(q.Evaluate("test"));
            Assert.False(q.Evaluate("something random"));
        }

        [Fact]
        public void Or_Test()
        {
            var q = new QueryEvaluator("contains('test') or contains('word')");

            Assert.True(q.Evaluate("test word"));
            Assert.True(q.Evaluate("word"));
            Assert.True(q.Evaluate("test"));
            Assert.False(q.Evaluate("something random"));
        }
    }
}
