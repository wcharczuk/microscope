using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Microscope
{
    public abstract class Node
    {
        public static HashSet<ExpressionType> ValidTypes = new HashSet<ExpressionType>()
            {
                ExpressionType.Not,
                ExpressionType.AndAlso,
                ExpressionType.OrElse,
                ExpressionType.Call,
            };

        public virtual ExpressionType ExpressionType { get; set; }
    }
}
