using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.Collections.Generic;
using System.Windows.MVVM;

namespace Framework.ViewModel
{
    public abstract class XmUITableViewController : UITableViewController, IGestionBinding, IView
    {
        public Dictionary<string, object> CurrentArguments { get; set; }

        public XmUITableViewController(IntPtr handle) : base(handle)
        {            
        }

        public override void ViewWillAppear(bool animated)
        {
            Services.CurrentActivity = this;
            this.CurrentArguments = Services.CurrentArguments;
        }

        public override void DidReceiveMemoryWarning()
        {
            this.DisposeDataContext();
        }

        public abstract void AddBinding();

        IViewModel _dataContext;
        public IViewModel DataContext
        {
            get
            {
                return _dataContext;
            }
            set
            {
                _dataContext = value;
                this.InitDataContext();
                this.AddBinding();
                (_dataContext as IViewModel).AttachView(this);
            }
        }
    }

    public abstract class XmUIViewController : UIViewController, IGestionBinding, IView
    {
        public Dictionary<string, object> CurrentArguments { get; set; }

        public XmUIViewController(IntPtr handle)
            : base(handle)
        {            
        }

        public override void ViewWillAppear(bool animated)
        {
            Services.CurrentActivity = this;
            this.CurrentArguments = Services.CurrentArguments;

        }

        public override void DidReceiveMemoryWarning()
        {
            this.DisposeDataContext();
        }

        public abstract void AddBinding();

        public virtual void AddBehavior()
        {

        }

        IViewModel _dataContext;
        public IViewModel DataContext
        {
            get
            {
                return _dataContext;
            }
            set
            {
                _dataContext = value;
                this.InitDataContext();
                this.AddBinding();
                this.AddBehavior();
                (this.DataContext as IViewModel).AttachView(this);
            }
        }
    }

    public abstract class XmUITabBarController : UITabBarController, IGestionBinding, IView
    {
        public Dictionary<string, object> CurrentArguments { get; set; }

        public event EventHandler<EventArgs> ViewDidAppearEvent;

        public XmUITabBarController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewWillAppear(bool animated)
        {
            Services.CurrentActivity = this;
            this.CurrentArguments = Services.CurrentArguments;
        }

        public override void DidReceiveMemoryWarning()
        {
            this.DisposeDataContext();
        }

        public override void ViewDidAppear(bool animated)
        {
            if (ViewDidAppearEvent != null)
                ViewDidAppearEvent(this, new EventArgs());
        }

        public abstract void AddBinding();
        public virtual void AddBehavior()
        {

        }

        IViewModel _dataContext;
        public IViewModel DataContext
        {
            get
            {
                return _dataContext;
            }
            set
            {
                _dataContext = value;
                this.InitDataContext();
                this.AddBinding();
                this.AddBehavior();
                (_dataContext as IViewModel).AttachView(this);
            }
        }
    }

    public class XmUITableViewCell : UITableViewCell, IGestionBinding, IView
    {
        public Dictionary<string, object> CurrentArguments { get; set; }

        public List<BindingDescription> ListeBindings = new List<BindingDescription>();

        public XmUITableViewCell(IntPtr handle) : base(handle)
        {

        }

        public XmUITableViewCell(UITableViewCellStyle style, NSString reuseIdentifier) : base(style, reuseIdentifier)
        {

        }

        public void Link(object p_objDataContext, BindingDescription p_objBindingDescription)
        {
            GestionBindingExtensions.LinkBindingToControl(p_objDataContext, p_objBindingDescription);
        }

        public override void RemoveFromSuperview()
        {
            base.RemoveFromSuperview();
            this.DisposeDataContext();
        }

        public virtual void AddBinding()
        {

        }

        IViewModel _dataContext;
        public IViewModel DataContext
        {
            get
            {
                return _dataContext;
            }
            set
            {
                _dataContext = value;
                this.InitDataContext();
                this.AddBinding();
                (this.DataContext as IViewModel).AttachView(this);
            }
        }
    }
}