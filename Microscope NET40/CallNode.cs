using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Microscope
{
    public class CallNode : Node
    {
        public CallNode()
        {
            this.Arguments = new List<string>();
        }

        public override ExpressionType ExpressionType
        {
            get
            {
                return ExpressionType.Call;
            }
            set { }
        }

        public String MethodName { get; set; }
        public List<String> Arguments { get; set; }

        public override string ToString()
        {
            return String.Format("{0}({1})", 
                this.MethodName, 
                String.Join(",", this.Arguments.Select(_ => String.Format("'{0}'", _)).ToArray())
            );
        }
    }
}