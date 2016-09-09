using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary
{
    /// <summary>
    /// A thread safe container for accessing a list.
    /// </summary>
    public class SyncList<T>
    {

        private List<T> _files;


        public SyncList()
        {
            _files = new List<T>();
        }


        public void Add(T obj)
        {
            lock (_files)
            {
                _files.Add(obj);
            }
        }

        public void Remove(T obj)
        {
            lock (_files)
            {
                _files.Remove(obj);
            }
        }

        public void Clear()
        {
            lock (_files)
            {
                _files.Clear();
            }
        }

        public T GetFile(int index)
        {
            lock (_files)
            {
                return _files.ElementAt(index);
            }
        }

        public int Count()
        {
            lock (_files)
            {
                return _files.Count;
            }
        }

        public override string ToString()
        {
            String temp = "";

            lock (_files)
            {
                foreach (T f in _files)
                {
                    temp = temp + f.ToString() + "\n";
                }
            }

            return temp;
        }


        /// <summary>
        /// Returns copy of the internal list. May be faster than using the get and add methods when lots of threads are using them.
        /// </summary>
        /// <returns>Copy of internal list.</returns>
        public List<T> CopyOfList()
        {
            lock (_files)
            {
                return new List<T>(_files);
            }
        }


    }
}
