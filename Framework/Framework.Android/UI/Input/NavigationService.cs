using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.MVVM;

namespace Framework.UI.Input
{
    public class NavigationService
    {
        public void OpenXmView<T>(Dictionary<string, object> p_dicArguments) where T : IView
        {
            Android.Content.Intent l_objXmView = new Android.Content.Intent(Services.CurrentActivity, typeof(T));
            Services.CurrentActivity.StartActivity(l_objXmView);
            Services.CurrentArguments = p_dicArguments;
        }

        public void CloseXmView()
        {
            Services.CurrentActivity.Finish();
        }
    }
}
