#Microscope#

Microscope is a flexible yet minimal text evaluation language that compiles to native lambdas. It is useful in evaluating large numbers of strings (URLs, dates, etc.)

###When would I use this?###

When you're storing regular expressions as strings, and changing them over time, and are running into limits in their functionality or having maintainability problems. 

We use them to filter urls mostly, and the filters are updated over time in the database so that we don't have to constantly patch code.

Did we mention it's relatively fast once compiled?

###What does this not do?###

**It doesn't replace regexes**

Pure and simple, Microscope is an extension to regexes *when they're used to validate or filter strings*. It doesn't cover use cases such as extraction.

If you can get the job done with just regexes, keep using them.

###Installation###


```bash
pm> install-package microscope
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

If numbers are less than "3.14":

```microscope
lessthan("3.14", "float")
```
Note: in this example we provide a type to tell the evaluator how to handle the comparison.

If dates are after May 1st, 2014:

```microscope
greaterthan("2014-05-01", "datetime", "yyyy-MM-dd");
```
Note: the format tells the parser how to evaluate the argument date and the input date. If that format fails for either, it'll try a regular parse.

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
* equals(expr, type) : returns the_string parsed as the specified type compared to expr parsed as the specified type
* equals(expr, type, format) : returns the_string parsed as the specified type compared to expr parsed as the specified type with the specified format
* greaterthan(expr) : returns the_string.CompareTo(expr) > 0;
* greaterthan(expr, type) : returns the_string parsed as the specified type, compared to expr, parsed as the specified type 
* greaterthan(expr, type, format) : returns the_string parsed as the specified type, compared to expr, parsed as the specified type with the specified format
* lessthan(expr) : returns the_string.CompareTo(expr) < 0;
* lessthan(expr, type) : returns the_string parsed as the specified type, compared to expr, parsed as the specified type
* lessthan(expr, type, format) : returns the_string parsed as the specified type, compared to expr, parsed as the specified type with the specified format

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

