using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Android.App;
using System.Windows.MVVM;
using Android.Views;
using Android.Widget;
using System.Reflection;
using System.Windows.Input;
using System.Collections;
using Framework.ViewModel;
using Android.Graphics.Drawables;
using Android.Graphics;

namespace Framework.ViewModel
{
    public class XmTabView : TabActivity, IGestionBinding, IDisposable, IView
    {
        public Dictionary<string, object> CurrentArguments { get; set; }

        public XmTabView()
        {
            Services.CurrentActivity = this;
            this.CurrentArguments = Services.CurrentArguments;
        }

        //Sur le focus de la vue on affecte la currentView
        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);
            if (hasFocus)
                Services.CurrentActivity = this;
        }

        IViewModel _DataContext;
        public IViewModel DataContext
        {
            get { return _DataContext; }
            set
            {
                _DataContext = value;
                this.InitDataContext();
                OnPrepareBidings();
                (_DataContext as IViewModel).AttachView(this);
            }
        }

        void IDisposable.Dispose()
        {
            if (_DataContext != null)
            {
                this.DisposeDataContext();
            }
        }

        protected virtual void OnPrepareBidings()
        {
        }
    }

    public abstract class XmView : Activity, IGestionBinding, IDisposable, IView
    {
        public Dictionary<string, object> CurrentArguments { get; set; }

        private List<BindingDescription> _lstBinding = new List<BindingDescription>();

        public XmView()
        {
            Services.CurrentActivity = this;
            this.CurrentArguments = Services.CurrentArguments;
        }

        protected override void OnCreate(Android.OS.Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            OnCreate(Services.CurrentArguments);
        }

        protected virtual void OnCreate(Dictionary<string, object> p_dicArguments)
        {

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (this.DataContext != null)
                this.DataContext.DettachView(this);
        }

        //Sur le focus de la vue on affecte la currentView
        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);
            if (hasFocus)
                Services.CurrentActivity = this;
        }

        public override async void OnBackPressed()
        {
            if (_DataContext != null)
            {
               if (await _DataContext.CallCancel())
                    base.OnBackPressed();
            }
            else
                base.OnBackPressed();
        }

        protected override void OnStop()
        {
            base.OnStop();
            if (DataContext != null)
            {
                (DataContext as IViewModel).CallHomeButton();
            }
        }

        IViewModel _DataContext;
        public IViewModel DataContext 
        {
            get { return _DataContext; }
            set 
            {
                _DataContext = value;
                this.InitDataContext();
                this.AddBindings();
                (_DataContext as IViewModel).AttachView(this);
            }
        }

        void IDisposable.Dispose()
        {
            if (_DataContext != null)
            {
                this.DisposeDataContext();
            }
        }

        public abstract void AddBindings();
    }
}
