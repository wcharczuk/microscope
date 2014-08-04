using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microscope
{
    public class ParserException : Exception
    {
        public ParserException() : base() { }
        public ParserException(String message) : base(message) { }
    }
}