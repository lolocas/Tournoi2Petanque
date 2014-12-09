using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Widget;
using Android.App;
using System.Collections;
using Android.Views;
using Android.Content;
using System.Reflection;
using System.Windows.Input;
using System.Collections.Specialized;
using System.ComponentModel;
using Framework.ViewModel;

namespace System.Windows.MVVM
{
    //Classe permettant la gestion du BaseAdapter avec le binding
    class BindingAdapter : BaseAdapter
    {
        private IList m_lstItems { get; set; }
        BindingDescription m_objCurrentBindingDescription;

        public BindingAdapter(IList p_lstItems, BindingDescription p_objCurrentBindingDescription)
        {
            m_lstItems = p_lstItems;
            m_objCurrentBindingDescription = p_objCurrentBindingDescription;
            if (m_lstItems != null)
            {
                if (this.m_lstItems is INotifyCollectionChanged)
                {
                    (this.m_lstItems as INotifyCollectionChanged).CollectionChanged += BindingAdapter_CollectionChanged;
                }
                foreach (object item in m_lstItems)
                {
                    if (item is INotifyPropertyChanged)
                        (item as INotifyPropertyChanged).PropertyChanged += BindingAdapter_PropertyChanged;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (this.m_lstItems != null)
            {
                if (this.m_lstItems is INotifyCollectionChanged)
                {
                    (this.m_lstItems as INotifyCollectionChanged).CollectionChanged -= BindingAdapter_CollectionChanged;
                }
                foreach (object item in m_lstItems)
                {
                    if (item is INotifyPropertyChanged)
                        (item as INotifyPropertyChanged).PropertyChanged -= BindingAdapter_PropertyChanged;
                }
            }
        }

        void BindingAdapter_PropertyChanged(object sender, ComponentModel.PropertyChangedEventArgs e)
        {
            Services.CurrentActivity.
                RunOnUiThread(()=>
                    NotifyDataSetChanged());
        }

        void BindingAdapter_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Services.CurrentActivity.
                RunOnUiThread(() =>
                    NotifyDataSetChanged());
        }

        public override int Count
        {
            get { return m_lstItems != null ? m_lstItems.Count : 0; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return (Java.Lang.Object)this.m_lstItems[position];
        }

        public object GetItemAtPosition(int position)
        {
            return this.m_lstItems[position];
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public int GetItemIndex(object p_objItem)
        {
            if (p_objItem == null)
                return 0;
            for (int l_intPosition = 0; l_intPosition < m_lstItems.Count; l_intPosition++)
            {
                if (m_lstItems[l_intPosition].GetHashCode() == p_objItem.GetHashCode())
                    return l_intPosition;
            }
            return 0;
        }

        public override global::Android.Views.View GetView(int position, global::Android.Views.View convertView, global::Android.Views.ViewGroup parent)
        {
            var item = m_lstItems[position];

            View view = convertView;             
            if (view == null) //On ne passe qu'une fois à la création cela permet de ne faire qu'un click par bouton
            {
                view = LayoutInflater.From(Services.CurrentActivity).Inflate(m_objCurrentBindingDescription.SubDataContext.SubBindingDescription.ViewId, parent, false) as LinearLayout;
                if (m_objCurrentBindingDescription.SubDataContext.Commands != null)
                {
                    foreach (CommandDescription l_objCommandDescription in m_objCurrentBindingDescription.SubDataContext.Commands)
                    {
                        View l_objViewCommand = view.FindViewById(l_objCommandDescription.CommandViewId);
                        l_objViewCommand.Click += (object sender, EventArgs e) =>
                        {
                            //Comme on ne passe qu'une fois, l'item n'est pas le bon
                            var l_objCommand = GestionBindingExtensions.DataContextProperty<ICommand>(item, l_objCommandDescription.CommandPath);
                            if (((Button)sender).Tag != null) //Tag != null => SelectedItem
                            {
                                //On se sert de celui enregistré dans le tag qui lui est mis à jour à chaque appel de getview
                                int l_intPosition = (int)((Button)sender).Tag;
                                l_objCommand.Execute(GetItemAtPosition(l_intPosition));
                            }
                            else
                                l_objCommand.Execute(l_objCommandDescription.CommandParameter);                            
                        };

                    }
                }
            }
            //Lien entre les controles de la liste et le binding courant
            foreach (BindingDescription l_objBindingDescription in m_objCurrentBindingDescription.SubDataContext.Bindings)
            {
                if (l_objBindingDescription.ViewId == 0)
                    l_objBindingDescription.View = view;
                else
                    l_objBindingDescription.View = view.FindViewById<View>(l_objBindingDescription.ViewId);
                GestionBindingExtensions.LinkBindingToControl(item, l_objBindingDescription);
            }
            if (m_objCurrentBindingDescription.SubDataContext.Commands != null)
            {
                foreach (CommandDescription l_objCommandDescription in m_objCurrentBindingDescription.SubDataContext.Commands)
                {
                    View l_objViewCommand = view.FindViewById(l_objCommandDescription.CommandViewId);
                    if (l_objCommandDescription.CommandParameter != null)
                    {
                        if (l_objCommandDescription.CommandParameter.ToString() == "SelectedItem")
                            l_objViewCommand.Tag = position;
                    }
                }
            }

            return view;
        }
    }
}
