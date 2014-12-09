using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
#if __ANDROID__
using Android.Views;
#endif

using System.Reflection;
using System.Windows.Data;

namespace System.Windows.MVVM
{
    public enum BindingMode
    {
        OneWay = 0,
        TwoWay
    }

    public enum ActionProperty
    {
        BackgroundColor,
        Checked,
        Count,
        Enabled,
        Hint,
        ItemsSource,
        Max,
        Progress,
        Text,
        TextColor,
        SelectedItem,
        Visibility
    }

    public enum XmVisibility : byte
    {
        Visible,
        Collapsed
    }
    #if __ANDROID__ || __IOS__
    public class BindingDescription
    {
        public BindingDescription()
        {

        }
        #if __ANDROID__
        public BindingDescription(Type typeView, View view, int viewId, ActionProperty action, string sourcePropertyPath, BindingMode mode = BindingMode.OneWay,
                                  IValueConverter converter = null, object converterParamater = null, string stringFormat = "")
        #elif __IOS__
        public BindingDescription(MonoTouch.Foundation.NSObject view, ActionProperty action, string sourcePropertyPath, BindingMode mode = BindingMode.OneWay,
                                  IValueConverter converter = null, object converterParamater = null, string stringFormat = "")
        #endif
        {
            #if __ANDROID__
            TypeView = typeView;
            View = view;
            ViewId = viewId;
            #elif __IOS__
            TypeView = view.GetType();
            View = view;
            #endif
            Converter = converter;
            ConverterParamater = converterParamater;
            StringFormat = stringFormat;
            Action = action;
            SourcePropertyPath = sourcePropertyPath;
            Mode = mode;
        }

        public Type TypeView { get; set; }
        #if __ANDROID__
        public View View { get; set; }
        public int ViewId { get; set; }
        public DataContextDescription SubDataContext { get; set; }
        #elif __IOS__
        public MonoTouch.Foundation.NSObject View { get; set; }
        #endif
        public ActionProperty Action { get; set; }
        public string SourcePropertyPath { get; set; }
        public System.Windows.Data.IValueConverter Converter { get; set; }
        public object ConverterParamater { get; set; }
        public string StringFormat { get; set; }
        public BindingMode Mode { get; set; }
    }

    public class DataContextDescription
    {
        public List<BindingDescription> Bindings { get; set; }
        public List<CommandDescription> Commands { get; set; }
        public BindingDescription SubBindingDescription { get; set; }
    }

    public class CommandDescription
    {
        public CommandDescription(Type typeView, int commandViewId, string commandPath, object commandParameter, Action callBack = null)
        {
            TypeView = typeView;
            CommandViewId = commandViewId;
            CommandPath = commandPath;
            CommandParameter = commandParameter;
            Callback = callBack;
        }

        public Type TypeView { get; set; }
        public int CommandViewId { get; set; }
        public string CommandPath { get; set; }
        public object CommandParameter { get; set; }
        public Action Callback { get; set; }
    }
#endif
}
