#Microscope#

Microscope is a flexible yet minimal text evaluation language that compiles to native lambdas. It is useful in evaluating large numbers of strings (URLs, large chunks of text, etc.)

Why do this?

Regular expressions are pretty bad at testing if text has certain elements, but doesn't contain other elements. Microscope acts as a layer above regular expressions that lets you combine operations logically.

###Installation###

```bash
> install-package microscope
```

Or download the source and compile the binary yourself and add it as a reference.

###Examples###

Example Queries:

Text that contains "product" and doesn't have any numbers in it.

```microscope
contains("product") and not matches("[0-9]+")
```

Text that contains "product" or "service" and doesn't contain "google":

```microscope
(contains("product") or contains("service")) and not contains("google")
```

If dates are greater than "2012-07-04":

```microscope
greaterthan("2012-07-04", "{0:d}")
```

Example Usage:

```C#

var evaluator = new Microscope.QueryEvaluator("contains('product') and not matches('[0-9]+')");

Assert.True(evaluator.Evaluate("http://something.com/product/"));
Assert.False(evaluator.Evaluate("http://something.com/"));
Assert.False(evaluator.Evaluate("http://something.com/product/50"));
//etc.
```

###Supported Functions###

* matches(expr) : an alias to C# System.Text.RegularExpressions.Regex.IsMatch(the_string, expr)
* matchesi(expr) : an alias to C# System.Text.RegularExpressions.Regex.IsMatch(the string, expr) with RegexOptions.IgnoreCase
* contains(expr) : an alias to C# the_string.Contains(expr)
* containsi(expr) : an alias to C# the_string.IndexOf(expr, StringComparison.InvariantCultureIgnoreCase) >= 0;
* startswith(expr) : an alias to C# the_string.StartsWith(expr)
* startswithi(expr) : an alias to C# the_string.StartsWith(expr, StringComparison.InvariantCultureIgnoreCase)
* endswith(expr) : an alias to C# the_string.EndsWith(expr)
* endswithi(expr) : an alias to C# the_string.EndsWith(expr, StringComparison.InvariantCultureIgnoreCase)
* equals(expr) : an alias to C# the_string.Equals(expr)
* equalsi(expr) : an alias to C# the_string.Equals(expr, StringComparison.InvariantCultureIgnoreCase) 
* isempty() : an alias to C# String.IsNullOrEmpty(the_string)
* equals(expr, format) : returns the_string.Equals(String.Format(format, expr))
* greaterthan(expr, format) : returns the_string.CompareTo(String.Format(format, expr)) > 0;
* lessthan(expr, format) : returns the_string.CompareTo(String.Format(format, expr)) < 0;

Logical Operators:

* and : Binary operator equivalent to C# '&&'
* or : Binary operator equivalent to C# '||'
* not : Unary operator equivalent to C# '!'

###Future Plans###

The main plans for future versions are the ability to mutate the evaluated string and pass that as an argument to other functions.

Example mutators would be:

* Substring(index), Substring(index, length) : Standard the_string.Substring() operations.
* Extract(expr) : Would run the expression against the_string and would return the first match.
* ToLower(), ToUpper() : the casing operations

###Pull Requests / Contributions###
Keep them coming.

