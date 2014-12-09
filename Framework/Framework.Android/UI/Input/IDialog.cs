using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Framework.UI.Input
{
    public interface IDialog
    {
        void Show(Action<string> CallBack);
        Task<string> Show();

        bool WithYes { get; set; }
        bool WithNo { get; set; }
        bool WithOk { get; set; }
        bool WithDetails { get; set; }
    }
}
