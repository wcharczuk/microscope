using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microscope
{
    public class BinaryNode : Node
    {
        public Node Left { get; set; }
        public Node Right { get; set; }

        public override string ToString()
        {
            return string.Format("({0} {1} {2})", this.Left.ToString(), this.ExpressionType.ToString().ToUpper(), this.Right.ToString());
        }
    }
}
