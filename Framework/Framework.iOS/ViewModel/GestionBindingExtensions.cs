using Framework.Controls;
using Framework.Extensions;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.MVVM;

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

        /// <summary>
        /// Ajout d'un binding au DataContext
        /// </summary>
        /// <typeparam name="T">Le type de control</typeparam>
        /// <param name="gestionBinding"></param>
        /// <param name="viewId">L'ID de la vue</param>
        /// <param name="action">L'action du controle à binder</param>
        /// <param name="sourcePropertyPath">Le property path</param>
        /// <param name="mode">Mode OneWay ou TwoWay</param>
        /// <param name="converter">Le converter</param>
        /// <param name="converterParamater">Le paramètre de converter</param>
        public static void AddBinding(this IGestionBinding gestionBinding, UIView view, ActionProperty action, string sourcePropertyPath, BindingMode mode = BindingMode.OneWay, IValueConverter converter = null, object converterParamater = null)
        {            
            AddBinding(gestionBinding, new BindingDescription(view, action, sourcePropertyPath, mode, converter, converterParamater));
        }
        
        public static void AddBinding(this IGestionBinding gestionBinding, NSObject view, ActionProperty action, IViewModelPropertyDefinition VMPPropertyDefinition, BindingMode mode = BindingMode.OneWay, IValueConverter converter = null, object converterParamater = null)
        {
            AddBinding(gestionBinding, new BindingDescription(view, action, VMPPropertyDefinition.PropertyName, mode, converter, converterParamater));
        }
        
        public static void AddBinding(this IGestionBinding gestionBinding, BindingDescription p_objBindingDescription)
        {
            try
            {
                if (p_objBindingDescription == null)
                    throw new Exception("Classe BindingDescription non renseignée");
                if (gestionBinding.DataContext == null)
                    throw new Exception("DataContext non renseigné");
                if (p_objBindingDescription.View == null)
                    throw new Exception("View non renseigné");
                if (p_objBindingDescription.TypeView == null)
                    p_objBindingDescription.TypeView = p_objBindingDescription.View.GetType();

                PropertyInfo l_objProperty = gestionBinding.DataContext.GetType().GetProperty(p_objBindingDescription.SourcePropertyPath);
                if (l_objProperty == null)
                    throw new Exception(string.Format("Binding element [{0}] introuvable", p_objBindingDescription.SourcePropertyPath));

                gestionBinding.DataContext.Bindings.Add(p_objBindingDescription);
                if (p_objBindingDescription.Mode == BindingMode.TwoWay)
                {
                    if (p_objBindingDescription.TypeView == typeof(UIButton))
                    {
                        var button = (UIButton)p_objBindingDescription.View;

                        button.TouchUpInside += (sender, e) =>
                        {
                            try
                            {
                                gestionBinding.DataContext.updatingState = Notifier.UpdatingState.None;
                                button.Selected = !button.Selected;

                                l_objProperty.SetValue(gestionBinding.DataContext, button.Selected, null);
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
                    else if (p_objBindingDescription.TypeView == typeof(ComboBox))
                    {
                        ((ComboBox)p_objBindingDescription.View).SelectionChanged += (sender, e) =>
                        {
                            try
                            {
                                gestionBinding.DataContext.updatingState = Notifier.UpdatingState.None;
                                l_objProperty.SetValue(gestionBinding.DataContext, ((ComboBox)p_objBindingDescription.View).SelectedItem, null);
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
                    else if (p_objBindingDescription.TypeView == typeof(UITableView))
                    {
                        var l_objTabView = (UITableView)p_objBindingDescription.View;
                        //Le BindingTable est mis à jour au Raise de la liste c'est pour ça qu'on l'instancie ici car on y passe forcément avant
                        if (l_objTabView.Source == null)
                            l_objTabView.Source = new BindingTable();

                        (l_objTabView.Source as BindingTable).SelectedItemChanged += (s, e) =>
                        {
                            try
                            {
                                gestionBinding.DataContext.updatingState = Notifier.UpdatingState.None;
                                l_objProperty.SetValue(gestionBinding.DataContext, (l_objTabView.Source as BindingTable).SelectedItem, null);
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
                        #region GESTURE
                        //l_objTabView.AddGestureRecognizer(new UITapGestureRecognizer((gesture)  =>
                        //    {
                        //        try
                        //        {
                        //            gestionBinding.DataContext.updatingState = Notifier.UpdatingState.None;
                        //            //On utilise les coordonnées plutot que le SelectedItem, car l'événement qui déclanche le rowselected se passe après cet événement du coup on récupère le précédent SelectedItem
                        //            NSIndexPath indexPath = l_objTabView.IndexPathForRowAtPoint(gesture.LocationInView(l_objTabView));
                        //            gesture.CancelsTouchesInView = false; //Permet de libérer l'action sur le click (sinon le BindingTable.SelectedItem n'est pas renvoyé)
                        //            l_objProperty.SetValue(gestionBinding.DataContext, (l_objTabView.Source as BindingTable).GetItem<object>(indexPath.Row), null);
                        //        }
                        //        catch (Exception l_objException)
                        //        {
                        //            Services.DialogsService.ShowErrorDialog(l_objException);
                        //        }
                        //        finally
                        //        {
                        //            gestionBinding.DataContext.updatingState = Notifier.UpdatingState.UpdatingSource;
                        //        }
                        //    }));
                        //l_objTabView.UserInteractionEnabled = true;
                        #endregion

                    }
                    else if (p_objBindingDescription.TypeView == typeof(UITextField))
                    {
                        ((UITextField)p_objBindingDescription.View).EditingChanged += (sender, e) =>
                        {
                            try
                            {
                                gestionBinding.DataContext.updatingState = Notifier.UpdatingState.None;
                                //Type Nullable + text à vide on force le null
                                if (Nullable.GetUnderlyingType(l_objProperty.PropertyType) != null &&
                                    ((UITextField)p_objBindingDescription.View).Text == "")
                                    l_objProperty.SetValue(gestionBinding.DataContext, null, null);
                                else
                                {
                                    //Convertion automatique selon le type
                                    Object value = Convert.ChangeType(((UITextField)p_objBindingDescription.View).Text, Nullable.GetUnderlyingType(l_objProperty.PropertyType) ?? l_objProperty.PropertyType);
                                    l_objProperty.SetValue(gestionBinding.DataContext, value, null);
                                }
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
                    else if (p_objBindingDescription.TypeView == typeof(UITextView))
                    {
                        ((UITextView)p_objBindingDescription.View).Changed += (sender, e) =>
                        {
                            try
                            {
                                gestionBinding.DataContext.updatingState = Notifier.UpdatingState.None;
                                //Convertion automatique selon le type
                                Object value = Convert.ChangeType(((UITextView)p_objBindingDescription.View).Text, Nullable.GetUnderlyingType(l_objProperty.PropertyType) ?? l_objProperty.PropertyType);
                                l_objProperty.SetValue(gestionBinding.DataContext, value, null);
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
                    else if (p_objBindingDescription.TypeView == typeof(UISearchBar))
                    {
                        ((UISearchBar)p_objBindingDescription.View).TextChanged += (sender, e) =>
                        {
                            try
                            {
                                gestionBinding.DataContext.updatingState = Notifier.UpdatingState.None;
                                l_objProperty.SetValue(gestionBinding.DataContext, ((UISearchBar)p_objBindingDescription.View).Text, null);
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
                    else if (p_objBindingDescription.TypeView == typeof(UISwitch) || p_objBindingDescription.TypeView == typeof(UISlider))
                    {
                        ((UIControl)p_objBindingDescription.View).ValueChanged += (sender, e) =>
                        {
                            try
                            {
                                gestionBinding.DataContext.updatingState = Notifier.UpdatingState.None;
                                if (p_objBindingDescription.TypeView == typeof(UISwitch))
                                    l_objProperty.SetValue(gestionBinding.DataContext, ((UISwitch)p_objBindingDescription.View).On, null);
                                else if (p_objBindingDescription.TypeView == typeof(UISlider))
                                    l_objProperty.SetValue(gestionBinding.DataContext, (int)((UISlider)p_objBindingDescription.View).Value, null);
                                else
                                    throw new Exception(string.Format("BindingMode.TwoWay Type de composant {0} non géré", p_objBindingDescription.TypeView.ToString()));
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
                        throw new Exception(string.Format("BindingMode.TwoWay Type de composant {0} non géré", p_objBindingDescription.TypeView.ToString()));
                }
            }
            catch (Exception)
            {
                throw;
            }            
        }

        public static void AddCommand(this IGestionBinding gestionBinding, NSObject view, string commandPath, object commandParameter = null)
        {
            if (view == null)
                throw new Exception("Bouton Id introuvable");
            if (gestionBinding.DataContext == null)
                throw new Exception("DataContext non renseigné");
            Type l_objType = view.GetType();

            var command = DataContextProperty<ICommand>(gestionBinding.DataContext, commandPath);

            if (l_objType == typeof(UIButton))
            {
                (view as UIButton).TouchUpInside += (sender, e) =>
                {
                    try
                    {
                        //if (commandParameter is BindingDescription) //Permet de gérer un commandParameter en binding
                        //    command.Execute(DataContextProperty<object>(SubDataContext, (commandParameter as BindingDescription).SourcePropertyPath));
                        //else
                        command.Execute(commandParameter);
                    }
                    catch (Exception l_objException)
                    {
                        Services.DialogsService.ShowErrorDialog(l_objException);
                    }
                };

                command.CanExecuteChanged += (sender, e) =>
                {
                    (view as UIButton).Enabled = ((ICommand)command).CanExecute(commandParameter);
                };
            }
            else if (l_objType == typeof(UISearchBar))
            {
                (view as UISearchBar).SearchButtonClicked += (sender, e) =>
                {
                    try
                    {
                        command.Execute(commandParameter);
                    }
                    catch (Exception l_objException)
                    {
                        Services.DialogsService.ShowErrorDialog(l_objException);
                    }
                };
            }
            else if (l_objType == typeof(UIBarButtonItem))
            {
                (view as UIBarButtonItem).Clicked += (sender, e) =>
                {
                    try
                    {
                        command.Execute(commandParameter);
                    }
                    catch (Exception l_objException)
                    {
                        Services.DialogsService.ShowErrorDialog(l_objException);
                    }
                };
                command.CanExecuteChanged += (sender, e) =>
                {
                    new MonoTouch.Foundation.NSObject().InvokeOnMainThread(() =>
                    {
                        (view as UIBarButtonItem).Enabled = ((ICommand)command).CanExecute(commandParameter);
                    });
                };
            }
            else
                throw new Exception(string.Format("AddCommand Type de composant {0} non géré", l_objType.ToString()));

        }

        public static void AddBehavior(this IGestionBinding gestionBinding, Behavior behavior)
        {
        }

        /// <summary>
        /// Fonction levée quand un binding est modifié
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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
                new MonoTouch.Foundation.NSObject().InvokeOnMainThread(() =>
                {
                    l_lstBinding.ForEach(item => LinkBindingToControl(_CurrentDataContext, item));
                });

            }
            catch (Exception l_objException)
            {
                Services.DialogsService.ShowErrorDialog(l_objException);
            }
        }

        static void ViewModelValidationError(object sender, ValidationErrorEventArgs e)
        {
            try
            {
                IViewModel _CurrentDataContext = (sender as IViewModel);
                BindingDescription l_objBinding = _CurrentDataContext.Bindings.FirstOrDefault(binding => binding.SourcePropertyPath == e.PropertyName);
                if (l_objBinding == null)
                    throw new NullReferenceException("ViewModelValidationError Binding [" + e.PropertyName + "] introuvable");

                if (l_objBinding.TypeView == typeof(UITextField)
                 || l_objBinding.TypeView == typeof(UITextView)
                 || l_objBinding.TypeView == typeof(ComboBox))
                {
                    UIView l_objUIView = l_objBinding.View as UIView;

                    l_objUIView.Layer.BorderWidth = 1;
                    l_objUIView.Layer.BorderColor = UIColor.Red.CGColor;
                    l_objUIView.Layer.MasksToBounds = true;
                    l_objUIView.Layer.CornerRadius = 0.8f;
                    if (l_objBinding.TypeView == typeof(UITextField))
                        (l_objUIView as UITextField).AttributedPlaceholder = new NSAttributedString(e.ErrorMessage, null, UIColor.Red);
                }
                else
                    throw new Exception(string.Format("Type de composant ValidationError {0} non géré", l_objBinding.TypeView.ToString()));
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
                    l_objVisibility = DataContextProperty<XmVisibility>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);

                if (l_objVisibility == XmVisibility.Visible)
                    ((UIView)p_objBindingDescription.View).Hidden = false;
                else
                    ((UIView)p_objBindingDescription.View).Hidden = true;
            }
            else if (p_objBindingDescription.Action == ActionProperty.Enabled)
            {
                ((UIControl)p_objBindingDescription.View).Enabled = DataContextProperty<bool>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
            }
            else if (p_objBindingDescription.Action == ActionProperty.BackgroundColor)
            {
                UIColor l_objColor = UIColor.Clear;
                if (p_objBindingDescription.Converter != null)
                {
                    object l_objConvertValue = DataContextProperty<object>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                    l_objColor = (UIColor)p_objBindingDescription.Converter.Convert(l_objConvertValue, p_objBindingDescription.TypeView, p_objBindingDescription.ConverterParamater, null);
                }
                else
                {
                    object l_objectColor = DataContextProperty<string>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                    if (l_objectColor.GetType() == typeof(string))
                    {
                        string l_strBackgroundColor = DataContextProperty<string>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                        if (!string.IsNullOrEmpty(l_strBackgroundColor))
                            l_objColor = l_strBackgroundColor.FromHexString();
                    }
                    else
                        l_objColor = DataContextProperty<UIColor>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                }

                ((UIView)p_objBindingDescription.View).BackgroundColor = l_objColor;
            }            
            else if (p_objBindingDescription.TypeView == typeof(UIButton))
            {
                switch (p_objBindingDescription.Action)
                {
                    case ActionProperty.Text:
                        ((UIButton)p_objBindingDescription.View).SetTitle(DataContextProperty<string>(p_objDataContext, p_objBindingDescription.SourcePropertyPath), UIControlState.Normal);
                        break;
                    case ActionProperty.Checked:
                        ((UIButton)p_objBindingDescription.View).Selected = DataContextProperty<bool>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                        break;
                    default:
                        throw new Exception(string.Format("Action UIButton {0} non gérée", p_objBindingDescription.Action.ToString()));
                }
            }
            else if (p_objBindingDescription.TypeView == typeof(ComboBox))
            {
                switch (p_objBindingDescription.Action)
                {
                    case ActionProperty.ItemsSource:
                        var l_lstListe = DataContextProperty<IList>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                        ((ComboBox)p_objBindingDescription.View).ItemsSource = l_lstListe;
                        break;
                    case ActionProperty.SelectedItem:
                        object l_objSelectedItem = DataContextProperty<object>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                        ((ComboBox)p_objBindingDescription.View).SelectedItem = l_objSelectedItem;
                        break;
                    default:
                        throw new Exception(string.Format("Action ComboBox {0} non gérée", p_objBindingDescription.Action.ToString()));
                }
            }
            else if (p_objBindingDescription.TypeView == typeof(UILabel))
            {
                if (p_objBindingDescription.View == null) return;

                if (p_objBindingDescription.Converter != null)
                {
                    if (string.IsNullOrEmpty(p_objBindingDescription.SourcePropertyPath))
                        ((UILabel)p_objBindingDescription.View).Text = p_objBindingDescription.Converter.Convert(p_objDataContext, typeof(UILabel), p_objBindingDescription.ConverterParamater, null).ToString();
                    else
                        ((UILabel)p_objBindingDescription.View).Text = p_objBindingDescription.Converter.Convert(DataContextProperty<object>(p_objDataContext, p_objBindingDescription.SourcePropertyPath), typeof(UILabel), p_objBindingDescription.ConverterParamater, null).ToString();
                }
                else
                {
                    switch (p_objBindingDescription.Action)
                    {
                        case ActionProperty.Text:
                            var l_objData = DataContextProperty<object>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                            if (l_objData != null)
                            {
                                if (!string.IsNullOrEmpty(p_objBindingDescription.StringFormat))
                                    ((UILabel)p_objBindingDescription.View).Text = string.Format(p_objBindingDescription.StringFormat, l_objData);
                                else
                                    ((UILabel)p_objBindingDescription.View).Text = l_objData.ToString();
                            }
                            else
                                ((UILabel)p_objBindingDescription.View).Text = "";
                            break;
                        case ActionProperty.TextColor :
                            string l_strTextColor = DataContextProperty<string>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                            if (!string.IsNullOrEmpty(l_strTextColor))
                                ((UILabel)p_objBindingDescription.View).TextColor = l_strTextColor.FromHexString();
                            break;
                        default:
                            throw new Exception(string.Format("Action UILabel {0} non gérée", p_objBindingDescription.Action.ToString()));
                    }


                }
            }
            else if (p_objBindingDescription.TypeView == typeof(UITextField))
            {
                ((UITextField)p_objBindingDescription.View).Text = DataContextProperty<string>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
            }
            else if (p_objBindingDescription.TypeView == typeof(UITextView))
            {
                ((UITextView)p_objBindingDescription.View).Text = DataContextProperty<string>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
            }            
            else if (p_objBindingDescription.TypeView == typeof(UITabBarItem))
            {
                switch (p_objBindingDescription.Action)
                {
                    case ActionProperty.Count:
                        ((UITabBarItem)p_objBindingDescription.View).BadgeValue = DataContextProperty<object>(p_objDataContext, p_objBindingDescription.SourcePropertyPath).ToString();
                        break;
                    default:
                        throw new Exception(string.Format("Action UITableView {0} non gérée", p_objBindingDescription.Action.ToString()));
                }
            }  
            else if (p_objBindingDescription.TypeView == typeof(UITableView))
            {
                var l_objTableView = (UITableView)p_objBindingDescription.View;
                switch (p_objBindingDescription.Action)
                {
                    case ActionProperty.ItemsSource:
                        var l_lstListe = DataContextProperty<IList>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                        if (l_objTableView.Source == null) //Le BindingTable est instancié si un des binding est TwoWay pour récupèrer l'événement SelectedItemChanged
                            l_objTableView.Source = new BindingTable(l_objTableView, l_lstListe);
                        else
                            (l_objTableView.Source as BindingTable).LoadListe(l_objTableView, l_lstListe);
                        l_objTableView.ReloadData();
                        break;
                    default:
                       throw new Exception(string.Format("Action UITableView {0} non gérée", p_objBindingDescription.Action.ToString()));
                }
            }         
            else if (p_objBindingDescription.TypeView == typeof(UISearchBar))
            {
                switch (p_objBindingDescription.Action)
                {
                    case ActionProperty.Hint:
                        ((UISearchBar)p_objBindingDescription.View).Placeholder = DataContextProperty<string>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                        break;
                    case ActionProperty.Text:
                        ((UISearchBar)p_objBindingDescription.View).Text = DataContextProperty<string>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                        break;
                    default:
                        throw new Exception(string.Format("Action UISearchBar {0} non gérée", p_objBindingDescription.Action.ToString()));
                }            
            }
            else if (p_objBindingDescription.TypeView == typeof(UISlider))
            {
                switch (p_objBindingDescription.Action)
                {
                    case ActionProperty.Enabled:
                        ((UISlider)p_objBindingDescription.View).Enabled = DataContextProperty<bool>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                        break;
                    case ActionProperty.Progress:
                        ((UISlider)p_objBindingDescription.View).Value = DataContextProperty<int>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                        break;
                    case ActionProperty.Max:
                        ((UISlider)p_objBindingDescription.View).MaxValue = DataContextProperty<int>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
                        break;
                    default:
                        throw new Exception(string.Format("Action UISlider {0} non gérée", p_objBindingDescription.Action.ToString()));
                }
            }
            else if (p_objBindingDescription.TypeView == typeof(UISwitch))
            {
                ((UISwitch)p_objBindingDescription.View).On = DataContextProperty<bool>(p_objDataContext, p_objBindingDescription.SourcePropertyPath);
            }
            else
                throw new Exception(string.Format("Type de composant {0} non géré", p_objBindingDescription.TypeView.ToString()));
        }

        public static T DataContextProperty<T>(object dataContext, string propertyName)
        {
            object l_objValue = DataContextProperty(dataContext, propertyName);
            if (l_objValue == null) //Valeur de la proprieté à null on renvoit le defaut du type
                return default(T);
            if (!(l_objValue is T))
                throw new Exception(string.Format("Type attendu {0} non correct {1}", l_objValue.GetType().Name, typeof(T).Name));
            return (T)l_objValue;
        }

        public static object DataContextProperty(object dataContext, string propertyName)
        {
            PropertyInfo l_objProperty = dataContext.GetType().GetProperty(propertyName);
            if (l_objProperty == null)
                throw new Exception(string.Format("Binding element {0} introuvable", propertyName));
            return l_objProperty.GetValue(dataContext, null);
        }
    }
}