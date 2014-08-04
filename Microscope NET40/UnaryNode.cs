using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Microscope
{
    public class UnaryNode : Node
    {
        public override ExpressionType ExpressionType
        {
            get
            {
                return ExpressionType.Not;
            }
            set { }
        }

        public Node Node { get; set; }
    }
}
