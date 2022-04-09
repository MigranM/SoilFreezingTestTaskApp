using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTaskApp.Model
{
    public interface ICalculatedData<T>
    {
        T GetCalculatedDataValue();
    }
}
