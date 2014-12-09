using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.UI.Input
{
    public interface IAsyncTask
    {
        void ModifyTask(string p_strTitre, string p_strDescription);
        void EndTask();
    }
}
