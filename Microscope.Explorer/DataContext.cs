using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microscope.Explorer
{
    public delegate void DataContextChangedHandler(object sender, DataContextChangedEventArgs e);

    public class DataContextChangedEventArgs : EventArgs
    {
        public DataContextAction Action { get; set; }
        public TestCase TestCase { get; set; }
    }

    public enum DataContextAction
    {
        Add = 1,
        Edit = 2,
        Remove = 3
    }

    public class DataContext : IEnumerable<TestCase>
    {
        #region Static Singleton

        private static DataContext _default = null;
        public static DataContext Default
        {
            get
            {
                if (_default == null)
                {
                    _default = new DataContext();
                }

                return _default;
            }
        }

        #endregion

        #region Private Storage

        private Dictionary<Guid, TestCase> _testCaseLookup = new Dictionary<Guid, TestCase>();
        private List<TestCase> _testCases = new List<TestCase>();

        #endregion

        public DateTime? LastRun { get; set; }

        public void Add(String contents)
        {
            var testCase = new TestCase(contents);
            _testCases.Add(testCase);
            _testCaseLookup.Add(testCase.Id, testCase);
            this.OnChanged(this, new DataContextChangedEventArgs() { Action = DataContextAction.Add, TestCase = testCase });
        }

        public void Remove(Guid id)
        {
            var testCase = _testCaseLookup[id];

            _testCaseLookup.Remove(id);
            _testCases = _testCases.Where(_ => _.Id != id).ToList();

            this.OnChanged(this, new DataContextChangedEventArgs() { Action = DataContextAction.Remove, TestCase = testCase });
        }

        public void Edit(Guid id, String contents)
        {
            var testCase = _testCaseLookup[id];
            testCase.Contents = contents;

            this.OnChanged(this, new DataContextChangedEventArgs() { Action = DataContextAction.Edit, TestCase = testCase });
        }

        public void RunQuery(String query)
        {
            var queryEvaluator = new Microscope.QueryEvaluator(query);

            foreach(var testCase in _testCases)
            {
                try
                {
                    var result = queryEvaluator.Evaluate(testCase.Contents);
                    testCase.Passed = result;
                    testCase.Errored = false;
                    testCase.Exception = null;
                }
                catch(Exception e)
                {
                    testCase.Errored = true;
                    testCase.Exception = e;
                }
            }

            this.LastRun = DateTime.Now;
        }

        public event DataContextChangedHandler Changed;

        public void OnChanged(object sender, DataContextChangedEventArgs e)
        {
            if(this.Changed != null)
            {
                this.Changed(sender, e);
            }
        }

        #region IEnumerable<>

        public IEnumerator<TestCase> GetEnumerator()
        {
            return _testCases.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _testCases.GetEnumerator();
        }

        #endregion
    }
}
