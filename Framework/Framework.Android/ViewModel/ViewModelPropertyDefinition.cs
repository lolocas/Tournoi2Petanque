using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Windows.MVVM
{
    public class ViewModelPropertyDefinition<TViewModel, TValue> : IViewModelPropertyDefinition<TViewModel, TValue>
    where TViewModel : ViewModel
    {
        public ViewModelPropertyDefinition(Expression<Func<TViewModel, TValue>> l_objPropertyExpression, Expression<Func<TViewModel, TValue>> l_objFieldExpression)
        {
            PropertyExpression = l_objPropertyExpression;
            _PropertyInfo = PropertyExpression.GetProperty<TViewModel, TValue>().PropertyInfo;
            var res = l_objFieldExpression.GetField();
            SetValue = res.GetAction<TViewModel, TValue>();
            GetValue = res.GetFunc<TViewModel, TValue>();
        }

        /// <summary>
        /// Nom de la propriété.
        /// </summary>
        public string PropertyName
        {
            get { return _PropertyInfo.Name; }
        }

        private System.Reflection.PropertyInfo _PropertyInfo;
        /// <summary>
        /// Informations sur la propriété.
        /// </summary>
        public System.Reflection.PropertyInfo PropertyInfo
        {
            get { return _PropertyInfo; }
        }

        /// <summary>
        /// Délégué de lecture de valeur. Ce délégué est auto déduit à l'initialisation.
        /// </summary>
        private Func<TViewModel, TValue> GetValue { get; set; }

        /// <summary>
        /// Délégué d'écriture de valeur. Ce délégué est auto déduit à l'initialisation.
        /// </summary>
        private Action<TViewModel, TValue> SetValue { get; set; }

        /// <summary>
        /// Délégué exécuté à chaque changement de valeur de la propriété.
        /// </summary>
        public Action<TViewModel, TValue, TValue> ValueChanged { get; set; }

        public Expression<Func<TViewModel, TValue>> PropertyExpression { get; set; }

        public TValue GetPropertyValue(TViewModel VM)
        {
            if (GetValue != null)
            {
                return GetValue(VM);
            }
            return default(TValue);
        }

        public void SetPropertyValue(TViewModel VM, TValue NewValue)
        {
            if (SetValue != null)
            {
                SetValue(VM, NewValue);
                if (ValueChanged != null)
                    ValueChanged(VM, GetPropertyValue(VM), NewValue);
            }
        }
    }

    public interface IViewModelPropertyDefinition<TViewModel, TValue> : IViewModelPropertyDefinition
    where TViewModel : class, IViewModel
    {
        Expression<Func<TViewModel, TValue>> PropertyExpression { get; set; }

        TValue GetPropertyValue(TViewModel VM);

        void SetPropertyValue(TViewModel VM, TValue NewValue);
    }

    public interface IViewModelPropertyDefinition
    {
        /// <summary>
        /// Nom de la propriété.
        /// </summary>
        string PropertyName { get; }

        /// <summary>
        /// Informations sur la propriété.
        /// </summary>
        PropertyInfo PropertyInfo { get; }
    }
}
