using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.UI.Input
{
    public interface IProgressTask
    {
        void EndTask();
        int Progress {get; set;}
    }
}