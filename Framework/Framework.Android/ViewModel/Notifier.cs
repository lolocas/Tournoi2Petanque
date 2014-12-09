using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

namespace System.Windows.MVVM
{
    [DataContract] //DataContract pour pouvoir sérialiser des classes qui dérivent de Notifier
    public abstract class Notifier : INotifier, INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<ValidationErrorEventArgs> ValidationError;
        public UpdatingState updatingState { get; set; }

        public enum UpdatingState
        {
            UpdatingSource,
            None            
        }

        //Force l'update même si UpdatingState = None 
        public void Update(IViewModelPropertyDefinition p_objViewModelPropertyDefinition)
        {
            DoNotifyPropertyChanged(p_objViewModelPropertyDefinition.PropertyName);
        }

        protected void DoNotifyPropertyChanged<TViewModel, TValue>(Expression<Func<TViewModel, TValue>> propertyExpression)
            where TViewModel : ViewModel
        {
            if (updatingState == UpdatingState.None)
            {
                //updatingState = UpdatingState.UpdatingSource;
                return;
            }
   
            var propertyName = ExtractPropertyName(propertyExpression);
            DoNotifyPropertyChanged(propertyName);
        }

        protected string ExtractPropertyName<TViewModel, TValue>(Expression<Func<TViewModel, TValue>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }

            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException("ExtractPropertyName_NotAMember", "propertyExpression");
            }

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
            {
                throw new ArgumentException("ExtractPropertyName_NotAProperty", "propertyExpression");
            }

            var getMethod = property.GetGetMethod(true);

            if (getMethod == null)
            {
                // this shouldn't happen - the expression would reject the property before reaching this far
                throw new ArgumentException("ExtractPropertyName_NoGetter", "propertyExpression");
            }

            if (getMethod.IsStatic)
            {
                throw new ArgumentException("ExtractPropertyName_Static", "propertyExpression");
            }

            return memberExpression.Member.Name;
        }

        protected void DoNotifyPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            if (updatingState == UpdatingState.None)
            {
                //updatingState = UpdatingState.UpdatingSource;
                return;
            }
            var propertyName = ExtractPropertyName(propertyExpression);
            DoNotifyPropertyChanged(propertyName);
        }

        private void DoNotifyPropertyChanged(string PropertyName)
        {
            VerifyPropertyName(PropertyName);

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        /// <summary>
        /// Fonction permettant de vérifier que le nom de la propriété existe
        /// Cette méthode est appelé avant que la propriété soit utilisée.
        /// Cela permet d'éviter les erreurs lorsque l'on change le nom de la propriété
        /// sans changer son PropertyName
        /// <para>Cette méthode n'est active qu'en mode DEBUG</para>
        /// </summary>
        /// <param name="propertyName"></param>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        private void VerifyPropertyName(string propertyName)
        {
            var myType = this.GetType();
            if (myType.GetProperty(propertyName) == null)
            {
                throw new ArgumentException("Property not found", propertyName);
            }
        }

        protected string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }

            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException("ExtractPropertyName_NotAMember", "propertyExpression");
            }

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
            {
                throw new ArgumentException("ExtractPropertyName_NotAProperty", "propertyExpression");
            }

            var getMethod = property.GetGetMethod(true);

            if (getMethod == null)
            {
                // this shouldn't happen - the expression would reject the property before reaching this far
                throw new ArgumentException("ExtractPropertyName_NoGetter", "propertyExpression");
            }

            if (getMethod.IsStatic)
            {
                throw new ArgumentException("ExtractPropertyName_Static", "propertyExpression");
            }

            return memberExpression.Member.Name;
        }

        #region INotifyDataErrorInfo

        protected void DoValidateError<T>(Expression<Func<T>> propertyExpression, string errorMessage)
        {
            var propertyName = ExtractPropertyName(propertyExpression);
            VerifyPropertyName(propertyName);

            if (!m_lstValidationError.ContainsKey(propertyName))
            {
                List<string> lst = new List<string>();
                lst.Add(errorMessage);
                m_lstValidationError.Add(propertyName, lst);
            }

            if (ValidationError != null)
                ValidationError(this, new ValidationErrorEventArgs(propertyName, errorMessage));
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        Dictionary<String, List<String>> m_lstValidationError = new Dictionary<string, List<string>>();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        //Propriété déclenchée par ErrorsChanged et qui renvoit la valeur en erreur et son message
        public Collections.IEnumerable GetErrors(string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName) || !m_lstValidationError.ContainsKey(propertyName))
                return null;
            return m_lstValidationError[propertyName];
        }

        public bool HasErrors
        {
            get { return this.m_lstValidationError.Count > 0; }
        }

        #endregion
    }

    public interface INotifier
    {
        System.Windows.MVVM.Notifier.UpdatingState updatingState {get; set;}
        event EventHandler<ValidationErrorEventArgs> ValidationError;
        event PropertyChangedEventHandler PropertyChanged;
    }

    public class ValidationErrorEventArgs : EventArgs
    {
        public ValidationErrorEventArgs(string propertyName, string errorMessage)
        {
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
        }

        public string PropertyName { get; private set; }
        public string ErrorMessage { get; private set; }

    }
}
