using System.Collections.Generic;

namespace System.Windows.MVVM
{
    public interface IView
    {
        Dictionary<string, object> CurrentArguments { get; set; }
    }
}