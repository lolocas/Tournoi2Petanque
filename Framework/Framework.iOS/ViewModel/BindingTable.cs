using Framework.Events;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.MVVM;

namespace Framework.ViewModel
{
    /// <summary>
    /// Permet de mettre en correspondance la liste et la cellule
    /// Dans le storyboard, il faut paramétrer pour le TableViewCell
    ///  - "class" le fichier de description de la cellule (dérivé de XmUITableViewCell)
    ///  - "identifier" il faut mettre "BindingCell"
    /// </summary>
    public class BindingTable : UITableViewSource
    {
        public const string BINDINGCELL = "BindingCell";

        public object SelectedItem { get; set; }
        public event EventHandler<EventDictionaryArgs> SelectedItemChanged;
        private IList m_lstItems;
        MonoTouch.Foundation.NSObject _threadUI;

        private UITableView m_objTableView;

        public BindingTable()
        {
            _threadUI = new MonoTouch.Foundation.NSObject();
        }

        public BindingTable(UITableView p_objTableView, IList p_lstItems)
        {
            _threadUI = new MonoTouch.Foundation.NSObject();
            LoadListe(p_objTableView, p_lstItems);
        }

        public void LoadListe(UITableView p_objTableView, IList p_lstItems)
        {
            m_objTableView = p_objTableView;
            m_lstItems = p_lstItems;
            if (this.m_lstItems is INotifyCollectionChanged)
            {
                (this.m_lstItems as INotifyCollectionChanged).CollectionChanged += BindingTable_CollectionChanged;
            }
            foreach (object item in m_lstItems)
            {
                if (item is INotifyPropertyChanged)
                    (item as INotifyPropertyChanged).PropertyChanged += BindingTable_PropertyChanged;
            } 
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (this.m_lstItems != null)
            {
                if (this.m_lstItems is INotifyCollectionChanged)
                {
                    (this.m_lstItems as INotifyCollectionChanged).CollectionChanged -= BindingTable_CollectionChanged;
                }
                foreach (object item in m_lstItems)
                {
                    if (item is INotifyPropertyChanged)
                        (item as INotifyPropertyChanged).PropertyChanged -= BindingTable_PropertyChanged;
                }
            }
        }

        void BindingTable_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _threadUI.InvokeOnMainThread(() =>
            {
                m_objTableView.ReloadData();
            });
        }

        void BindingTable_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _threadUI.InvokeOnMainThread(() =>
            {
                m_objTableView.ReloadData();
            });
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            XmUITableViewCell cell = tableView.DequeueReusableCell(BINDINGCELL) as XmUITableViewCell;
            //On ajoute les bindings de la CustomCell associés à la table
            cell.AddBinding();
            //Pour chaque binding de la custom cell on met en correspondance les controles et le binding de la ligne
            foreach (BindingDescription l_objBindingDescription in cell.ListeBindings)
            {
                cell.Link(m_lstItems[indexPath.Row], l_objBindingDescription);
            }
            return cell;
        }

        public override int RowsInSection(UITableView tableview, int section)
        {
            if (m_lstItems != null) //On passe par le constructeur dans param et on n'a pas encore de liste
                return m_lstItems.Count;
            return 0;
        }

        public T GetItem<T>(int id)
        {
            return (T)m_lstItems[id];
        }

        public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
        {
            if (editingStyle == UITableViewCellEditingStyle.Delete)
            {
                //Pour supprimer une ligne le ViewModel lié à l'item doit avoir une Command "DeleteCommand"
                var l_objItem = GetItem<object>(indexPath.Row);
                var l_objDeleteCommand = GestionBindingExtensions.DataContextProperty<ICommand>(l_objItem, "DeleteCommand");
                l_objDeleteCommand.Execute(l_objItem);

                //m_lstItems.RemoveAt(indexPath.Row);
                //tableView.DeleteRows(new[] { indexPath }, UITableViewRowAnimation.Fade);
            }
        }

        public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return UITableViewCellEditingStyle.Delete;
        }

        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            return true;
        }

        public override string TitleForDeleteConfirmation(UITableView tableView, NSIndexPath indexPath)
        {
            return "Supprimer";
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            SelectedItem = GetItem<object>(indexPath.Row);

            if (SelectedItemChanged != null)
            {
                Dictionary<string, object> l_objDictionary = new Dictionary<string, object>();

                l_objDictionary.Add("SelectedItem", SelectedItem);
                SelectedItemChanged(this, new EventDictionaryArgs(l_objDictionary));
            }                
        }
    }
}