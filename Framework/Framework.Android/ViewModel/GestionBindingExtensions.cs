using Android.Widget;
using System;
using System.Reflection;
using System.Windows.Input;
using System.Windows.MVVM;
using System.Linq;
using Android.Views;
using System.ComponentModel;
using Android.Graphics;
using System.Collections;
using Android.Views.InputMethods;
using Android.Content;
using Android.Graphics.Drawables;

namespace Framework.ViewModel
{
    public static class GestionBindingExtensions
    {
        /// <summary>
        /// Application du data context et mise en route de la gestion des événements de binding
        /// </summary>
        /// <param name="gestionBinding"></param>
        /// <param name="dataContext">Le dataContext de type IViewModel</param>
        public static void InitDataContext(this IGestionBinding gestionBinding)
        {
            gestionBinding.DataContext.PropertyChanged += ViewModelPropertyChanged;
            gestionBinding.DataContext.ValidationError += ViewModelValidationError;
        }

        /// <summary>
        /// Permet de décharger le dataContext et ses éléments
        /// </summary>
        /// <param name="gestionBinding"></param>
        public static void DisposeDataContext(this IGestionBinding gestionBinding)
        {
            if (gestionBinding.DataContext != null)
            {
                gestionBinding.DataContext.PropertyChanged -= ViewModelPropertyChanged;
                gestionBinding.DataContext.ValidationError -= ViewModelValidationError;
            }
            gestionBinding.DataContext.Bindings.Clear();
        }

        public static void AddBinding<T>(this IGestionBinding gestionBinding, int viewId, ActionProperty action, IViewModelPropertyDefinition VMPPropertyDefinition, BindingMode mode = BindingMode.OneWay) where T : View
        {
            AddBinding<T>(gestionBinding, new BindingDescription(typeof(T), null, viewId, action, VMPPropertyDefinition.PropertyName, mode));
        }

        public static void AddBinding<T>(this IGestionBinding gestionBinding, BindingDescription bindingDescription) where T : View
        {
            try
            {
                PropertyInfo l_objProperty = gestionBinding.DataContext.GetType().GetProperty(bindingDescription.SourcePropertyPath);
                if (l_objProperty == null)
                    throw new Exception(string.Format("Binding element {0} introuvable", bindingDescription.SourcePropertyPath));

                try
                {
                    bindingDescription.TypeView = typeof(T);
                    if (bindingDescription.ViewId != 0 && bindingDescription.View == null) //On lie directement une View
                        bindingDescription.View = Services.CurrentActivity.FindViewById<T>(bindingDescription.ViewId);
                }
                catch (Exception)
                {
                    throw new Exception(string.Format("{0} introuvable sourcePropertyPath {1}", typeof(T).ToString(), bindingDescription.SourcePropertyPath));
                }

                gestionBinding.DataContext.Bindings.Add(bindingDescription);
                if (bindingDescription.Mode == BindingMode.TwoWay)
                {
                    if (typeof(T) == typeof(EditText))
                    {
                        ((EditText)bindingDescription.View).AfterTextChanged += (sender, e) =>
                        {
                            try
                            {
                                gestionBinding.DataContext.updatingState = Notifier.UpdatingState.None;
                                l_objProperty.SetValue(gestionBinding.DataContext, ((EditText)bindingDescription.View).Text, null);
                            }
                            catch (Exception l_objException)
                            {
                                Services.DialogsService.ShowErrorDialog(l_objException);
                            }
                            finally
                            {
                                gestionBinding.DataContext.updatingState = Notifier.UpdatingState.UpdatingSource;
                            }
                        };
                    }
                    else if (typeof(T) == typeof(CheckBox) || typeof(T) == typeof(ToggleButton))
                    {
                        ((CompoundButton)bindingDescription.View).CheckedChange += (sender, e) =>
                        {
                            try
                            {
                                gestionBinding.DataContext.updatingState = Notifier.UpdatingState.None;
                                l_objProperty.SetValue(gestionBinding.DataContext, ((CompoundButton)bindingDescription.View).Checked, null);
                            }
                            catch (Exception l_objException)
                            {
                                Services.DialogsService.ShowErrorDialog(l_objException);
                            }
                            finally
                            {
                                gestionBinding.DataContext.updatingState = Notifier.UpdatingState.UpdatingSource;
                            }
                        };
                    }
                    else if (typeof(T) == typeof(ListView))
                    {
                        ((ListView)bindingDescription.View).ItemClick += (sender, e) =>
                        {
                            try
                            {
                                gestionBinding.DataContext.updatingState = Notifier.UpdatingState.None;
                                l_objProperty.SetValue(gestionBinding.DataContext, (((ListView)bindingDescription.View).Adapter as BindingAdapter).GetItemAtPosition(e.Position), null);
                            }
                            catch (Exception l_objException)
                            {
                                Services.DialogsService.ShowErrorDialog(l_objException);
                            }
                            finally
                            {
                                gestionBinding.DataContext.updatingState = Notifier.UpdatingState.UpdatingSource;
                            }
                        };
                    }                    else if (typeof(T) == typeof(SeekBar))
                    {
                        (bindingDescription.View as SeekBar).ProgressChanged += (sender, e) =>
                        {
                            try
                            {
                                gestionBinding.DataContext.updatingState = Notifier.UpdatingState.None;
                                l_objProperty.SetValue(gestionBinding.DataContext, ((SeekBar)bindingDescription.View).Progress, null);
                            }
                            catch (Exception l_objException)
                            {
                                Services.DialogsService.ShowErrorDialog(l_objException);
                            }
                            finally
                            {
                                gestionBinding.DataContext.updatingState = Notifier.UpdatingState.UpdatingSource;
                            }
                        };
                    }
                    else if (typeof(T) == typeof(Spinner))
                    {
                        ((Spinner)bindingDescription.View).ItemSelected += (sender, e) =>
                        {
                            try
                            {
                                gestionBinding.DataContext.updatingState = Notifier.UpdatingState.None;
                                l_objProperty.SetValue(gestionBinding.DataContext, ((AdapterView)bindingDescription.View).SelectedItem, null);
                            }
                            catch (Exception l_objException)
                            {
                                Services.DialogsService.ShowErrorDialog(l_objException);
                            }
                            finally
                            {
                                gestionBinding.DataContext.updatingState = Notifier.UpdatingState.UpdatingSource;
                            }
                        };
                    }
                    else
                        throw new Exception(string.Format("Type de composant {0} non géré", typeof(T).ToString()));
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void AddCommand<T>(this IGestionBinding gestionBinding, int viewId, string commandPath, object commandParameter = null) where T : View
        {
            GestionCommand(typeof(T), Services.CurrentActivity.FindViewById(viewId), gestionBinding.DataContext, commandPath, commandParameter);
        }

        public static void GestionCommand(Type typeView, View currentView, object dataContext, string commandPath, object commandParameter, Action callBack = null)
        {
            try
            {
                ICommand l_objCommand = DataContextProperty<ICommand>(dataContext, commandPath);
                if (typeView == typeof(Button) || typeView == typeof(ToggleButton))
                {
                    var button = (Button)currentView;
                    if (button == null)
                        throw new Exception("Bouton Id introuvable");

                    button.Click += (sender, e) =>
                    {
                        try
                        {
                            l_objCommand.Execute(commandParameter);
                            if (callBack != null)
                                callBack();
                        }
                        catch (Exception l_objException)
                        {
                            Services.DialogsService.ShowErrorDialog(l_objException);
                        }
                    };
                    l_objCommand.CanExecuteChanged += (sender, e) =>
                    {
                        button.Enabled = ((ICommand)l_objCommand).CanExecute(commandParameter);
                    };
                }
                else if (typeView == typeof(EditText))
                {
                    EditText l_objEditText = (EditText)currentView;
                    l_objEditText.SetOnEditorActionListener(new OnEditorActionListener(l_objEditText, l_objCommand, commandParameter));
                }
                else
                    throw new Exception(string.Format("Type de composant command {0} non géré", typeView.ToString()));
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void ViewModelValidationError(object sender, ValidationErrorEventArgs e)
        {
            try
            {
                IViewModel _CurrentDataContext = (sender as IViewModel);
                BindingDescription _currentBinding = _CurrentDataContext.Bindings.FirstOrDefault(binding => binding.SourcePropertyPath == e.PropertyName);
                if (_currentBinding == null)
                    return;

                if (_currentBinding.TypeView == typeof(EditText))
                {
                    ((EditText)_currentBinding.View).RequestFocus();
                    ((EditText)_currentBinding.View).SetError(e.ErrorMessage, null);
                }
                else if (_currentBinding.TypeView == typeof(Spinner))
                {
                    Drawable drawable = ((Spinner)_currentBinding.View).Background;
                    drawable.SetColorFilter(Color.Red, PorterDuff.Mode.SrcAtop);
                    ((Spinner)_currentBinding.View).SetBackgroundDrawable(drawable);
                }
                else
                    throw new Exception(string.Format("Type de composant ValidationError {0} non géré", _currentBinding.TypeView.ToString()));
            }
            catch (Exception l_objException)
            {
                Services.DialogsService.ShowErrorDialog(l_objException);
            }
        }
        
        private static void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                IViewModel _CurrentDataContext = (sender as IViewModel);
                var l_lstBinding = _CurrentDataContext.Bindings.Where(binding => binding.SourcePropertyPath == e.PropertyName).ToList();
                if (l_lstBinding.Count() == 0)
                {
                    #if DEBUG
                    //System.Diagnostics.Debug.WriteLine("ViewModelPropertyChanged Binding [" + e.PropertyName + "] introuvable");
                    throw new NullReferenceException("ViewModelPropertyChanged Binding [" + e.PropertyName + "] introuvable");
                    #else
                    return;
                    #endif
                }
                Services.CurrentActivity.RunOnUiThread(() =>
                {
                    l_lstBinding.ForEach(item => LinkBindingToControl(_CurrentDataContext, item));
                });
            }
            catch (Exception l_objException)
            {
                Services.DialogsService.ShowErrorDialog(l_objException);
            }
        }

        //Lien entre le control et les données liées au binding qui vient d'être levé
        public static void LinkBindingToControl(object p_objDataContext, BindingDescription p_objBindingDescription)
        {
            //Action commune aux controles de type View
            if (p_objBindingDescription.Action == ActionProperty.Visibility)
            {
                XmVisibility l_objVisibility;
                if (p_objBindingDescription.Converter != null)
                {
                    object l_objConvertValue = DataContextProperty<object>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                    l_objVisibility = (XmVisibility)p_objBindingDescription.Converter.Convert(l_objConvertValue, p_objBindingDescription.TypeView, p_objBindingDescription.ConverterParamater, null);
                }
                else
                {
                    l_objVisibility = DataContextProperty<XmVisibility>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                }
                ((View)p_objBindingDescription.View).Visibility = (l_objVisibility == XmVisibility.Visible ? ViewStates.Visible : ViewStates.Gone);
            }
            else if (p_objBindingDescription.TypeView == typeof(TextView) || p_objBindingDescription.TypeView == typeof(EditText))
            {
                TextView l_objTextView = (TextView)p_objBindingDescription.View;
                if (p_objBindingDescription.Converter != null)
                {
                    if (string.IsNullOrEmpty(p_objBindingDescription.SourcePropertyPath))
                        l_objTextView.Text = p_objBindingDescription.Converter.Convert(p_objDataContext, typeof(TextView), p_objBindingDescription.ConverterParamater, null).ToString();
                    else
                        l_objTextView.Text = p_objBindingDescription.Converter.Convert(DataContextProperty<object>(p_objDataContext, p_objBindingDescription.SourcePropertyPath), typeof(TextView), p_objBindingDescription.ConverterParamater, null).ToString();
                }
                else
                {
                    switch (p_objBindingDescription.Action)
                    {
                        case ActionProperty.Text:
                        case ActionProperty.Count:
                            var l_objData = DataContextProperty<object>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                            if (l_objData != null)
                            {
                                if (!string.IsNullOrEmpty(p_objBindingDescription.StringFormat))
                                    l_objTextView.Text = string.Format(p_objBindingDescription.StringFormat, l_objData);
                                else
                                    l_objTextView.Text = l_objData.ToString();
                            }
                            else
                                l_objTextView.Text = "";
                            break;
                        case ActionProperty.Hint:
                            l_objTextView.Hint = DataContextProperty<string>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                            break;
                        case ActionProperty.TextColor:
                            string l_strColorText = DataContextProperty<string>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                            if (!string.IsNullOrWhiteSpace(l_strColorText))
                                l_objTextView.SetTextColor(Color.ParseColor(DataContextProperty<string>(p_objDataContext, p_objBindingDescription.SourcePropertyPath)));
                            else
                                l_objTextView.SetTextColor(Color.Transparent);
                            break;
                        case ActionProperty.BackgroundColor:
                            string l_strColorBack = DataContextProperty<string>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                            if (!string.IsNullOrWhiteSpace(l_strColorBack))
                                l_objTextView.SetBackgroundColor(Color.ParseColor(l_strColorBack));
                            else
                                l_objTextView.SetBackgroundColor(Color.Transparent);
                            break;
                        default:
                            throw new Exception(string.Format("Action TextView {0} non gérée", p_objBindingDescription.Action.ToString()));
                    }
                }
            }
            else if (p_objBindingDescription.TypeView == typeof(Button))
            {
                switch (p_objBindingDescription.Action)
                {
                    case ActionProperty.Enabled:
                        ((Button)p_objBindingDescription.View).Enabled = DataContextProperty<bool>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                        break;
                    case ActionProperty.Text:
                        ((Button)p_objBindingDescription.View).Text = DataContextProperty<string>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                        break;
                    default:
                        throw new Exception(string.Format("Action button {0} non gérée", p_objBindingDescription.Action.ToString()));
                }
            }
            else if (p_objBindingDescription.TypeView == typeof(ToggleButton) || p_objBindingDescription.TypeView == typeof(CheckBox))
            {
                switch (p_objBindingDescription.Action)
                {
                    case ActionProperty.Checked:
                        ((CompoundButton)p_objBindingDescription.View).Checked = DataContextProperty<bool>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                        break;
                    case ActionProperty.Enabled:
                        ((CompoundButton)p_objBindingDescription.View).Enabled = DataContextProperty<bool>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                        break;
                    case ActionProperty.Text:
                        ((CompoundButton)p_objBindingDescription.View).Text = DataContextProperty<string>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                        break;
                    default:
                        throw new Exception(string.Format("Action CompoundButton {0} non gérée", p_objBindingDescription.Action.ToString()));
                }
            }
            else if (p_objBindingDescription.TypeView == typeof(LinearLayout))
            {
                var l_objLinearLayout = (LinearLayout)p_objBindingDescription.View;
                switch (p_objBindingDescription.Action)
                {
                    case ActionProperty.BackgroundColor:
                        Color l_objColor;
                        if (p_objBindingDescription.Converter != null)
                        {
                            object l_objConvertValue = DataContextProperty<object>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                            l_objColor = (Color)p_objBindingDescription.Converter.Convert(l_objConvertValue, p_objBindingDescription.TypeView, p_objBindingDescription.ConverterParamater, null);
                        }
                        else
                            l_objColor = DataContextProperty<Color>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);

                        l_objLinearLayout.SetBackgroundColor(l_objColor);
                       break;
                    default:
                       throw new Exception(string.Format("Action LinearLayout {0} non gérée", p_objBindingDescription.Action.ToString()));
                }
            }
            else if (p_objBindingDescription.TypeView == typeof(ListView))
            {
                var l_objListView = (ListView)p_objBindingDescription.View;
                switch (p_objBindingDescription.Action)
                {
                    case ActionProperty.ItemsSource:
                        var l_lstListe = DataContextProperty<IList>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                        l_objListView.Adapter = new BindingAdapter(l_lstListe, p_objBindingDescription);
                        break;
                    case ActionProperty.SelectedItem:
                        object l_objSelectedItem = DataContextProperty<object>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                        int l_intPosition = (l_objListView.Adapter as BindingAdapter).GetItemIndex(l_objSelectedItem);
                        l_objListView.SetSelection(l_intPosition);
                        break;
                    default:
                        throw new Exception(string.Format("Action ListView {0} non gérée", p_objBindingDescription.Action.ToString()));
                }
            }
            else if (p_objBindingDescription.TypeView == typeof(SeekBar))
            {
                switch (p_objBindingDescription.Action)
                {
                    case ActionProperty.Enabled:
                        ((SeekBar)p_objBindingDescription.View).Enabled = DataContextProperty<bool>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                        break;
                    case ActionProperty.Progress:
                        ((SeekBar)p_objBindingDescription.View).Progress = DataContextProperty<int>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                        break;
                    case ActionProperty.Max:
                        ((SeekBar)p_objBindingDescription.View).Max = DataContextProperty<int>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                        break;
                    default:
                        throw new Exception(string.Format("Action SeekBar {0} non gérée", p_objBindingDescription.Action.ToString()));
                }
            }
            else if (p_objBindingDescription.TypeView == typeof(Spinner))
            {
                var l_objSpinner = (Spinner)p_objBindingDescription.View;
                switch (p_objBindingDescription.Action)
                {
                    case ActionProperty.ItemsSource:
                        var l_lstListe = DataContextProperty<IList>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                        l_objSpinner.Adapter = new ComboAdapter(l_lstListe, l_objSpinner.Tag.ToString());
                        break;
                    case ActionProperty.SelectedItem:
                        object l_objSelectedItem = DataContextProperty<object>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                        int l_intPosition = (l_objSpinner.Adapter as ComboAdapter).GetItemIndex(l_objSelectedItem);
                        l_objSpinner.SetSelection(l_intPosition);
                        break;
                    default:
                        throw new Exception(string.Format("Action Spinner {0} non gérée", p_objBindingDescription.Action.ToString()));
                }
            }
            else
                throw new Exception(string.Format("Type de composant {0} non géré", p_objBindingDescription.TypeView.ToString()));
        }

        public static T DataContextProperty<T>(object dataContext, string propertyName)
        {
            PropertyInfo l_objProperty = dataContext.GetType().GetProperty(propertyName);
            if (l_objProperty == null)
                throw new Exception(string.Format("Binding element {0} introuvable", propertyName));
            object l_objValue = l_objProperty.GetValue(dataContext, null);
            if (l_objValue == null) //Valeur de la proprieté à null on renvoit le defaut du type
                return default(T);
            if (!(l_objValue is T))
                throw new Exception(string.Format("Type attendu {0} non correct {1}", l_objValue.GetType().Name, typeof(T).Name));
            return (T)l_objValue;
        }

        private class OnEditorActionListener : Java.Lang.Object, TextView.IOnEditorActionListener
        {
            EditText m_objEditText;
            ICommand m_objCommand;
            object m_objCommandParameter;

            public OnEditorActionListener(EditText p_objEditText, ICommand p_objCommand, object p_objCommandParameter = null)
            {
                m_objEditText = p_objEditText;
                m_objCommand = p_objCommand;
                m_objCommandParameter = p_objCommandParameter;
            }

            public bool OnEditorAction(TextView v, ImeAction actionId, KeyEvent e)
            {
                if (actionId == ImeAction.Search)
                {
                    m_objCommand.Execute(m_objCommandParameter);
                    //On masque le keyboard
                    InputMethodManager imm = (InputMethodManager)Services.CurrentActivity.GetSystemService(Context.InputMethodService);
                    imm.HideSoftInputFromWindow(m_objEditText.WindowToken, 0);

                    return true;
                }
                return false;
            }
        }
    }
}
