using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Microscope
{
    /// <summary>
    /// Evaluates a query of the form:
    ///     matches("(www.clotheshor.se)") and not contains("admin") --contains www.clotheshor.se and doesn't contain admin
    ///     contains("www.clotheshor.se") and matches("[0-9]+") --contains www.clotheshor.se and contains numbers
    ///     (contains("something") and contains("something2")) and not contains("filter") --contains something and something2 but not filter
    /// </summary>
    public class QueryEvaluator
    {
        public QueryEvaluator() { }

        public QueryEvaluator(String query)
        {
            this._query = query;
            this.ParseAndCompile(query);
        }

        public static HashSet<string> ValidKeywords = new HashSet<string>()
        {
            "and",
            "or",
            "not",
            "matches",
            "matchesi",
            "contains",
            "containsi",
            "startswith",
            "startswithi",
            "endswith",
            "endswithi",
            "equals",
            "equalsi",
            "isempty",
        };

        public static HashSet<String> LogicKeywords = new HashSet<string>()
        {
            "and",
            "or",
            "not"
        };

        private String _query = null;
        private Func<String, Boolean> _compiledDelegate = null;

        public String Query { get { return _query; } }

        public Boolean Evaluate(string corpus)
        {
            if (_compiledDelegate == null) { throw new InvalidOperationException("Cannot evaluate without parsing+compiling a query first."); }

            return _compiledDelegate(corpus);
        }

        public void ParseAndCompile(string query)
        {
            if (String.IsNullOrEmpty(query))
            {
                throw new ArgumentNullException("query");
            }

            var node = Parse(query);
            _compiledDelegate = Compile(node);
        }

        public Node Parse(string query)
        {
            return _parse_block(query);
        }

        private Node _parse_block(string query)
        {
            Node root_node = null;
            var nodes = new Dictionary<String, Node>();

            #region Blocks and Calls
            var reduced_query = new String(query.ToArray());
            int state = 0;
            for (int x = 0; x < query.Length; x++)
            {
                char c = query[x];
                switch (state)
                {
                    case 0:
                        {
                            if (c == '@')
                            {
                                state = 3;
                                continue;
                            }
                            else if (char.IsWhiteSpace(c))
                            {
                                continue;
                            }
                            else if (char.IsLetter(c))
                            {
                                x--;
                                state = 1;
                                continue;
                            }
                            else if (c == '(')
                            {
                                state = 2;
                                continue;
                            }
                        }
                        continue;
                    case 1:
                        {
                            //read until 'word boundary'
                            string word = string.Empty;
                            string function_name = string.Empty;
                            char quote = default(char);
                            int brace_count = 0;
                            for (; x < query.Length; x++)
                            {
                                c = query[x];

                                if (quote != default(char))
                                {
                                    word += c;

                                    if (c == quote)
                                    {
                                        quote = default(char);
                                    }
                                    continue;
                                }
                                else
                                {
                                    if (c == '\'' || c == '"') { quote = c; word += c; continue; }
                                    else if (c == '(')
                                    {
                                        word += c;
                                        brace_count = 1;
                                    }
                                    else if (char.IsWhiteSpace(c) && brace_count == 0)
                                    {
                                        if (!LogicKeywords.Contains(function_name))
                                        {
                                            throw new ParserException(String.Format("Whitespace before () in function call {0} at [{1}]", function_name, x));
                                        }
                                        state = 0;
                                        break;
                                    }
                                    else if (brace_count == 1 && char.IsLetter(c))
                                    {
                                        throw new ParserException(String.Format("Non-whitespace character before closing ) or quoted argument in function call {0} at [{1}]", function_name, x));
                                    }
                                    else if (brace_count == 1 && x == query.Length) { throw new ParserException(String.Format("Reached the end of the sub query looking for a closing ')' for the function call {0}", function_name)); }
                                    else if (brace_count == 0 && c == ')') { throw new ParserException(String.Format("Unbalanced braces at [{0}]", x)); }
                                    else if (c == ')' && brace_count == 1)
                                    {
                                        word += c;

                                        if (c == ')') { brace_count = 0; }

                                        if (!LogicKeywords.Contains(word))
                                        {
                                            var node = _parse_call(word);
                                            var node_id = _get_id();
                                            nodes.Add(node_id, node);

                                            reduced_query = reduced_query.Replace(word, node_id);
                                        }

                                        state = 0;
                                        break;
                                    }
                                    else if (c == ')' && brace_count != 1)
                                    {
                                        throw new ParserException(String.Format("Unbalanced braces at [{0}]", x));
                                    }
                                    else { function_name += c; word += c; }
                                }
                            }

                            if (quote != default(char) && x == query.Length) { throw new ParserException(String.Format("Reached the end of the sub query looking for a closing '{0}' for the function call", quote)); }
                            if (brace_count == 1 && x == query.Length) { throw new ParserException("Reached the end of the sub query looking for a closing ')' for the function call"); }
                        }
                        continue;
                    case 2: //pull the block out ...
                        {
                            int brace_count = 1;

                            string block = string.Empty;

                            char quote = default(char);

                            for (; x < query.Length; x++)
                            {
                                c = query[x];

                                if (x == query.Length) { throw new ParserException("Invalid Syntax: Reached the end of the query without finding the closing ')' for a block."); }

                                if (quote != default(char))
                                {
                                    block += c;

                                    if (c == quote)
                                    {
                                        quote = default(char);
                                    }
                                }
                                else
                                {
                                    if (c == '\'' || c == '"') { quote = c; block += c; continue; }
                                    if (c == '(') { brace_count++; }
                                    else if (c == ')') { brace_count--; }

                                    if (c == ')' && brace_count == 0)
                                    {
                                        var new_node = _parse_block(block);
                                        var node_id = _get_id();
                                        nodes.Add(node_id, new_node);

                                        reduced_query = reduced_query.Replace(String.Format("({0})", block), node_id);
                                        state = 0;
                                        break;
                                    }
                                    else { block += c; }
                                }
                            }
                        }
                        continue;
                    case 3: //we're in a variable name
                        if (char.IsWhiteSpace(c))
                        {
                            state = 0;
                        }
                        continue;
                }
            }
            #endregion

            #region Unary (Not) Operations

            var further_reduced_query = new String(reduced_query.ToArray());
            state = 0;

            int unary_block_start = 0;
            UnaryNode working_unary_node = null;

            //go thorugh and handle unary operations, replace with nodes ...
            for (int x = 0; x < further_reduced_query.Length; x++)
            {
                char c = further_reduced_query[x];
                switch (state)
                {
                    case 0:
                        {
                            if (c == '@')
                            {
                                if (working_unary_node != null)
                                {
                                    state = 3;
                                }
                                else
                                {
                                    state = 1;
                                }
                                x--;
                            }
                            else if (char.IsLetter(c))
                            {
                                state = 2;
                                x--;
                            }
                        }
                        continue;
                    case 1: //go through the node name until the next
                        {
                            unary_block_start = x;
                            for (; x < further_reduced_query.Length; x++)
                            {
                                c = further_reduced_query[x];
                                if (char.IsWhiteSpace(c))
                                {
                                    state = 0;
                                    break;
                                }
                            }
                        }
                        continue;
                    case 2:
                        {
                            var word = string.Empty;
                            unary_block_start = x;
                            for (; x < further_reduced_query.Length; x++)
                            {
                                c = further_reduced_query[x];
                                if (char.IsWhiteSpace(c))
                                {
                                    state = 0;
                                    break;
                                }
                                else { word = word + c; }
                            }

                            if (word == "not")
                            {
                                working_unary_node = new UnaryNode();
                            }
                        }
                        continue;
                    case 3:
                        {
                            string node_id = string.Empty;
                            for (; x < further_reduced_query.Length; x++)
                            {
                                c = further_reduced_query[x];

                                if (char.IsWhiteSpace(c) || x == further_reduced_query.Length - 1)
                                {
                                    if (x == further_reduced_query.Length - 1) { node_id += c; }

                                    var node = nodes[node_id];
                                    var new_node_id = _get_id();
                                    working_unary_node.Node = node;
                                    nodes.Add(new_node_id, working_unary_node);
                                    working_unary_node = null;

                                    further_reduced_query = further_reduced_query.Substring(0, unary_block_start) + new_node_id + further_reduced_query.Substring(x + 1);

                                    x = 0;
                                    state = 0;
                                    break;
                                }
                                else { node_id += c; }
                            }
                        }
                        continue;
                }
            }

            #endregion

            #region Binary Collection

            Node left = null;
            BinaryNode working_binary_node = null;

            state = 0;
            for (int x = 0; x < further_reduced_query.Length; x++)
            {
                char c = further_reduced_query[x];
                switch (state)
                {
                    case 0:
                        {
                            if (c == '@')
                            {
                                if (working_binary_node != null)
                                {
                                    state = 2; //pull into 'right' of previous node
                                }
                                else
                                {
                                    state = 1; //pull into 'left' of new node
                                }
                                x--;
                            }
                            else if (char.IsLetter(c))
                            {
                                if (working_binary_node != null)
                                {
                                    state = 4;
                                }
                                else
                                {
                                    state = 3; //get a new node (based on what the word is).
                                }
                                x--;
                            }
                        }
                        continue;
                    case 1:
                        {
                            var node_id = String.Empty;
                            for (; x < further_reduced_query.Length; x++)
                            {
                                c = further_reduced_query[x];

                                if (char.IsWhiteSpace(c) || x == further_reduced_query.Length - 1)
                                {
                                    if (x == further_reduced_query.Length - 1) { node_id += c; }

                                    var node = nodes[node_id];

                                    left = node;
                                    state = 0;
                                    break;
                                }
                                else { node_id += c; }
                            }
                        }
                        continue;
                    case 2:
                        {
                            var node_id = String.Empty;
                            for (; x < further_reduced_query.Length; x++)
                            {
                                c = further_reduced_query[x];

                                if (char.IsWhiteSpace(c) || x == further_reduced_query.Length - 1)
                                {

                                    if (x == further_reduced_query.Length - 1) { node_id += c; }

                                    var node = nodes[node_id];

                                    working_binary_node.Right = node;
                                    left = null;
                                    state = 0;
                                    break;
                                }
                                else { node_id += c; }
                            }
                        }
                        continue;
                    case 3:
                        {
                            var logic_op = String.Empty;
                            for (; x < further_reduced_query.Length; x++)
                            {
                                c = further_reduced_query[x];

                                if (char.IsWhiteSpace(c))
                                {
                                    working_binary_node = new BinaryNode();
                                    working_binary_node.Left = left;
                                    switch (logic_op.ToLower())
                                    {
                                        case "and":
                                            working_binary_node.ExpressionType = ExpressionType.AndAlso;
                                            break;
                                        case "or":
                                            working_binary_node.ExpressionType = ExpressionType.OrElse;
                                            break;
                                        default:
                                            throw new ParserException(String.Format("Unknown Logic Keyword: {0}", logic_op));
                                    }

                                    left = null;
                                    state = 0;
                                    break;
                                }
                                else { logic_op += c; }
                            }
                        }
                        continue;
                    case 4:
                        {
                            var logic_op = String.Empty;
                            for (; x < further_reduced_query.Length; x++)
                            {
                                c = further_reduced_query[x];

                                if (char.IsWhiteSpace(c))
                                {
                                    var old_working_binary_node = working_binary_node;
                                    var new_working_binary_node = new BinaryNode();
                                    new_working_binary_node.Left = old_working_binary_node;
                                    working_binary_node = new_working_binary_node;

                                    switch (logic_op.ToLower())
                                    {
                                        case "and":
                                            working_binary_node.ExpressionType = ExpressionType.AndAlso;
                                            break;
                                        case "or":
                                            working_binary_node.ExpressionType = ExpressionType.OrElse;
                                            break;
                                        default:
                                            throw new ParserException(String.Format("Unknown Logic Keyword: {0}", logic_op));
                                    }

                                    left = null;
                                    state = 0;
                                    break;
                                }
                                else { logic_op += c; }
                            }
                        }
                        continue;
                }
            }

            #endregion

            #region Resolve Root Node

            if (working_binary_node != null) { root_node = working_binary_node; }
            else
            {
                var words = further_reduced_query.Split(' ');
                for (int x = 0; x < words.Length; x++)
                {
                    var word = words[x];
                    if (word[0] == '@')
                    {
                        var node = nodes[word];
                        root_node = node;
                        break;
                    }
                }
            }

            #endregion

            return root_node;
        }

        private string _get_id()
        {
            return String.Format("@{0}", System.Guid.NewGuid().ToString("N"));
        }

        private CallNode _parse_call(string sub_query)
        {
            var call_node = new CallNode();

            var method = string.Empty;
            var argument = string.Empty;

            char start_quote_char = default(char);

            int state = 0;

            for (var x = 0; x < sub_query.Length; x++)
            {
                char c = sub_query[x];

                switch (state)
                {
                    case 0:
                        {
                            if (char.IsLetter(c))
                            {
                                method = method + c;
                                continue;
                            }
                            else if (c == '(')
                            {
                                state = 1;
                                continue;
                            }
                        }
                        continue;
                    case 1: //start arg parse
                        {
                            if (c == '"' || c == '\'')
                            {
                                start_quote_char = c;
                                state = 2;
                                continue;
                            }
                            else if (c == ')')
                            {
                                state = -1;
                                continue;
                            }
                            else { continue; }
                        }
                    case 2:
                        {
                            if (c.Equals(start_quote_char))
                            {
                                state = 3;
                                continue;
                            }
                            else
                            {
                                argument = argument + c;
                                continue;
                            }
                        }
                    case 3:
                        {
                            if (x == sub_query.Length) { throw new ParserException("Reached the end of the sub query looking for a closing ')' for the function call"); }

                            if (c == ')')
                            {
                                state = -1;
                                continue;
                            }
                        }
                        continue;
                    default:
                        continue;
                }
            }

            if (!ValidKeywords.Contains(method.ToLower())) { throw new ParserException("Invalid Function: " + method); }

            call_node.MethodName = String.Format("_{0}", method.ToLower());
            call_node.Argument = argument;

            return call_node;
        }

        public Func<String, Boolean> Compile(Node rootNode)
        {
            var the_string = Expression.Parameter(typeof(String), "the_string");
            var expr = Visit(rootNode, the_string);
            var lambda = Expression.Lambda<Func<String, Boolean>>(expr, the_string);
            return lambda.Compile();
        }

        private Expression Visit(Node node, ParameterExpression the_string)
        {
            switch (node.ExpressionType)
            {
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                    return VisitBinary(node as BinaryNode, the_string);
                case ExpressionType.Not:
                    return VisitUnary(node as UnaryNode, the_string);
                case ExpressionType.Call:
                    return VisitCall(node as CallNode, the_string);
                default:
                    return null;
            }
        }

        private Expression VisitBinary(BinaryNode node, ParameterExpression the_string)
        {
            var left_expr = Visit(node.Left, the_string);
            var right_expr = Visit(node.Right, the_string);

            if (node.ExpressionType == ExpressionType.AndAlso)
            {
                return Expression.AndAlso(left_expr, right_expr);
            }
            else
            {
                return Expression.OrElse(left_expr, right_expr);
            }
        }

        private Expression VisitCall(CallNode node, ParameterExpression the_string)
        {
            if (String.IsNullOrWhiteSpace(node.Argument))
            {
                return Expression.Call(typeof(QueryEvaluator), node.MethodName, null, the_string);
            }
            else
            {
                return Expression.Call(typeof(QueryEvaluator), node.MethodName, null, the_string, Expression.Constant(node.Argument));
            }
        }

        private Expression VisitUnary(UnaryNode node, ParameterExpression the_string)
        {
            var expr = Visit(node.Node, the_string);
            return Expression.Not(expr);
        }

        private static bool _isEmpty(string corpus)
        {
            return String.IsNullOrWhiteSpace(corpus);
        }

        private static bool _contains(string corpus, string expression)
        {
            return corpus.Contains(expression);
        }

        private static bool _containsi(string corpus, string expression)
        {
			return corpus.IndexOf(expression, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }
        
        private static bool _startswith(string corpus, string expression)
        {
            return corpus.StartsWith(expression);
        }

        private static bool _startswithi(string corpus, string expression)
        {
            return corpus.StartsWith(expression, StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool _endswith(string corpus, string expression)
        {
            return corpus.EndsWith(expression);
        }

        private static bool _endswithi(string corpus, string expression)
        {
            return corpus.EndsWith(expression, StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool _matches(string corpus, string expression)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(corpus, expression);
        }

        private static bool _matchesi(string corpus, string expression)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(corpus, expression, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        private static bool _equals(string corpus, string expression)
        {
            return corpus.Equals(expression);
        }

        private static bool _equalsi(string corpus, string expression)
        {
            return corpus.Equals(expression, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}

