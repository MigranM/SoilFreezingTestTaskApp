using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTaskApp.Model
{
    public interface IDataProvider<T>
    {
        T Read(out bool result, string[] args = null);
        void Write(T data, string[] args = null);
    }
}
