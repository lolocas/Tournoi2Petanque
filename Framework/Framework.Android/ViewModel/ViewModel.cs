#if __ANDROID__
using Android.App;
#endif

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
namespace System.Windows.MVVM
{
    public abstract class ViewModel : Notifier, IViewModel
    {
        protected bool IsNew { get; set; }
        protected bool IsInEdit { get; set; }
        public bool EventLoaded { get; set; }
        #if __ANDROID__ || __IOS__
        public List<BindingDescription> Bindings { get; set; }
        #endif

        public ViewModel()
        {
            #if __ANDROID__ || __IOS__
            Bindings = new List<BindingDescription>();
            #endif
        }

        public event EventHandler<ViewModelEventArgs> AfterCancelEdit;
        public event EventHandler<ViewModelEventArgs> AfterEndEdit;

        protected virtual void OnAfterEndEdit()
        {
        }

        protected void OnAfterCancelEdit()
        {
            if (AfterCancelEdit != null)
                AfterCancelEdit(this, new ViewModelEventArgs(this));
        }

        protected void RunOnUiThread(Action action)
        {
            #if __ANDROID__
            Services.CurrentActivity.RunOnUiThread(action);
            #elif __IOS__
                Services.CurrentActivity.BeginInvokeOnMainThread(() => action());
            #endif
        }

        /// <summary>
        /// Renvoit la valeur de la ressource
        /// </summary>
        /// <param name="id">L'id de la ressource</param>
        /// <returns>La valeur de la ressource</returns>
        protected string GetStringRessources(object id)
        {
            #if __ANDROID__
                return Services.CurrentActivity.GetString(Convert.ToInt32(id));
            #else
                return (id as string);
            #endif
        }

        #region === Lecture / Ecriture de propriétés ===
        protected TValue GetValue<TViewModel, TValue>(IViewModelPropertyDefinition<TViewModel, TValue> Property)
            where TViewModel : ViewModel
        {
            return Property.GetPropertyValue(this as TViewModel);
        }
        protected void SetValue<TViewModel, TValue>(IViewModelPropertyDefinition<TViewModel, TValue> Property, TValue Value)
            where TViewModel : ViewModel
        {
            Property.SetPropertyValue(this as TViewModel, Value);
            DoNotifyPropertyChanged(Property.PropertyExpression); 
        }
        #endregion === Lecture / Ecriture de propriétés ===

        public void AttachView(IView View)
        {
            OnAttachView(View);
        }

        protected virtual void OnAttachView(IView View)
        {
        }

        public void DettachView(IView View)
        {
            OnDettachView(View);
        }

        protected virtual void OnDettachView(IView View)
        {
        }

        public override string ToString()
        {
            return string.Format("{0}#{1}", this.GetType().Name, this.GetHashCode());
        }

        #region === COMMANDS ===========================================================

        #region === Property : ValidateCommand ===
        /// <summary>
        /// Property : ValidateCommand
        /// </summary>
        private ICommand _ValidateCommand;
        public ICommand ValidateCommand
        {
            get
            {
                return _ValidateCommand = _ValidateCommand ?? new RelayCommand(OnValidate, () => CanValidateCommand);
            }
        }

        private bool _CanValidateCommand = true;
        public bool CanValidateCommand
        {
            private get
            {
                return _CanValidateCommand;
            }
            set
            {
                if (_CanValidateCommand != value)
                {
                    _CanValidateCommand = value;
                    ((RelayCommand)ValidateCommand).RaiseCanExecuteChanged();
                }
            }
        }
        #endregion

        #region === Property : CancelCommand ===
        /// <summary>
        /// Property : CancelCommand
        /// </summary>
        private ICommand _CancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                return _CancelCommand = _CancelCommand ?? new RelayCommand(OnCancel, () => CanCancelCommand);
            }
        }

        private bool _CanCancelCommand = true;
        public bool CanCancelCommand
        {
            private get
            {
                return _CanCancelCommand;
            }
            set
            {
                if (_CanCancelCommand != value)
                {
                    _CanCancelCommand = value;
                    ((RelayCommand)CancelCommand).RaiseCanExecuteChanged();
                }
            }
        }
        #endregion

        protected void OnValidate(object parameter)
        {
            if (OnBeforeValidate(parameter))
            {
                if (AfterEndEdit != null)
                    AfterEndEdit(this, new ViewModelEventArgs(this));
                OnAfterEndEdit();
            } 
        }

        protected virtual bool OnBeforeValidate(object parameter)
        {
            return true;
        }

        protected async void OnCancel(object parameter)
        {
            await CallCancel();
        }

        public async Task<bool> CallCancel()
        {
            if (await OnBeforeCancel())
            {
                OnAfterCancelEdit();
                return true;
            }
            return false;
        }

        protected virtual async Task<bool> OnBeforeCancel()
        {
            return await Task<bool>.Factory.StartNew(() => true);
        }

        public void CallHomeButton()
        {
            OnHomeButton();
        }

        public bool CallVolumeUpButton()
        {
            return OnVolumeUpButton();
        }

        protected virtual void OnHomeButton()
        {
        }

        protected virtual bool OnVolumeUpButton()
        {
            return true;
        }
        #endregion === COMMANDS ========================================================
    }

    public class ViewModelEventArgs : EventArgs
    {
        public ViewModelEventArgs(IViewModel ViewModel)
        {
            this.ViewModel = ViewModel;
        }
        public IViewModel ViewModel { get; protected set; }
    }

    public interface IViewModel : INotifier
    {
        void AttachView(IView View);
        void DettachView(IView View);

        event EventHandler<ViewModelEventArgs> AfterCancelEdit;
        event EventHandler<ViewModelEventArgs> AfterEndEdit;
        Task<bool> CallCancel();
        void CallHomeButton();
        bool CallVolumeUpButton();
        #if __ANDROID__ ||__IOS__
        List<BindingDescription> Bindings { get; set; }
        #endif

    }
}
