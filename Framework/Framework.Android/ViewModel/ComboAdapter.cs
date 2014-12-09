using Android.App;
using Android.Content;
using Android.Database;
using Android.Views;
using Android.Widget;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Framework.ViewModel
{
    public class ComboAdapter : Java.Lang.Object, ISpinnerAdapter
    {
        public IList m_lstItems { get; private set; }
        private string m_strDisplayMemberPath;

        public ComboAdapter(IList p_lstItems, string p_strDisplayMemberPath)
        {
            m_lstItems = p_lstItems;
            m_strDisplayMemberPath = p_strDisplayMemberPath;
            if (this.m_lstItems != null && this.m_lstItems.Count > 0) //On ne peut ne pas avoir un item selectionné sur un spinner
                m_lstItems.Insert(0, Activator.CreateInstance(m_lstItems[0].GetType(), new object[] {})); //On rajoute un item null pour avoir aucun selecteditem
        }

        public View GetDropDownView(int position,View convertView, ViewGroup parent)
        {
            TextView txtSpinner = (TextView)((LayoutInflater)Services.CurrentActivity.GetSystemService(Context.LayoutInflaterService)).Inflate(Android.Resource.Layout.SimpleSpinnerDropDownItem, parent, false);
            string l_strTitle = "";
            if (!string.IsNullOrEmpty(m_strDisplayMemberPath))
                l_strTitle = GestionBindingExtensions.DataContextProperty<string>(m_lstItems[position], m_strDisplayMemberPath);
            else
                l_strTitle = m_lstItems[position].ToString();
            
            txtSpinner.Text = l_strTitle;
            return txtSpinner;
        }

        public int Count
        {
            get { return m_lstItems.Count; }
        }

        public Java.Lang.Object GetItem(int position)
        {
            if (position == 0) //Blank item
                return null;
            return (Java.Lang.Object)this.m_lstItems[position];
        }

        public int GetItemIndex(object p_objItem)
        {
            if (p_objItem == null)
                return 0;
            for(int l_intPosition = 0; l_intPosition < m_lstItems.Count; l_intPosition++)
            {
                if (m_lstItems[l_intPosition].GetHashCode() == p_objItem.GetHashCode())
                    return l_intPosition;
            }
            return 0;
        }

        public long GetItemId(int position)
        {
            return position;
        }

        public int GetItemViewType(int position)
        {
            throw new NotImplementedException();
        }

        public View GetView(int position, View convertView, ViewGroup parent)
        {
            TextView l_txtSpinner = (TextView)((LayoutInflater)Services.CurrentActivity.GetSystemService(Context.LayoutInflaterService)).Inflate(Android.Resource.Layout.SimpleSpinnerItem, parent, false);
            string l_strTitle = "";
            if (!string.IsNullOrEmpty(m_strDisplayMemberPath))
                l_strTitle = GestionBindingExtensions.DataContextProperty<string>(m_lstItems[position], m_strDisplayMemberPath);
            else
                l_strTitle = m_lstItems[position].ToString();

            l_txtSpinner.Text = l_strTitle;
            return l_txtSpinner;
        }

        public bool HasStableIds
        {
            get { return false; }
        }

        public bool IsEmpty
        {
            get { return false; }
        }

        public void RegisterDataSetObserver(DataSetObserver observer)
        {
            
        }

        public void UnregisterDataSetObserver(DataSetObserver observer)
        {
            
        }

        public int ViewTypeCount
        {
            get { throw new NotImplementedException(); }
        }
    }
}
