using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microscope.Explorer
{
    public class TestCase
    {
        public TestCase()
        {
            this.Id = System.Guid.NewGuid();
        }

        public TestCase(String contents)
        {
            this.Id = System.Guid.NewGuid();
            this.Contents = contents;
        }

        public Guid Id { get; set; }
        public String Contents { get; set; }
        public Boolean? Passed { get; set; }
        public Boolean? Errored { get; set; }
        public Exception Exception { get; set; }
    }
}
