using System;
using System.Collections.Generic;
using System.Text;
using Framework.UI.Input;
using Framework.UI;
#if WPF
using System.Windows;
#endif

namespace System
{
    public static class Services
    {
        public static Dictionary<string, object> CurrentArguments { get; set; }

        static public DialogsService DialogsService = new DialogsService();
        static public AsyncTaskService AsyncTaskService = new AsyncTaskService();
        static public NavigationService NavigationService = new NavigationService();
#if __ANDROID__
        static public Android.App.Activity CurrentActivity { get; set; }
#elif __IOS__
        static public MonoTouch.UIKit.UIViewController CurrentActivity { get; set; }
        static public Framework.UI.Input.DialogsService.AlertView CurrentModale { get; set; }
#elif WPF
        static public Window CurrentActivity { get; set; }
        static public WebService WebService = new WebService();
#endif
    }
}
