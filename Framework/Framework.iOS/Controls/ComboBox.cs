using Framework.Events;
using Framework.ViewModel;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace Framework.Controls
{
    [Register("ComboBox")]
    public class ComboBox : UIView
    {
        public event EventHandler<EventDictionaryArgs> SelectionChanged;
        public UIButton m_objButton;

        public ComboBox(IntPtr handle)
            : base(handle)
        {
            m_objButton = UIButton.FromType(UIButtonType.RoundedRect);
            m_objButton.Frame = new RectangleF(0, 0, this.Bounds.Width, this.Bounds.Height);
            m_objButton.Layer.BorderWidth = 1;
            m_objButton.Layer.BorderColor = UIColor.Black.CGColor;

            CAGradientLayer l_objCAGradientLayer = new CAGradientLayer();
            l_objCAGradientLayer.Colors = new CGColor[]
            {
                UIColor.FromRGB(215, 215, 215).CGColor,
                UIColor.FromRGB(100, 100, 100).CGColor
            };
            l_objCAGradientLayer.Locations = new NSNumber[]
            {
                .5f,
                1f
            };
            l_objCAGradientLayer.Frame = m_objButton.Layer.Bounds;
            m_objButton.Layer.AddSublayer(l_objCAGradientLayer);
            m_objButton.Layer.MasksToBounds = true;

            m_objButton.TouchUpInside += (s, e) =>
            {
                Services.DialogsService.ShowListDialog(this.Title, ItemsSource, DisplayMemberPath, SelectedItem, (selecteditem) =>
                {
                    this.SelectedItem = selecteditem;
                });
            };

            this.AddSubview(m_objButton);
        }

        private IList _ItemsSource;
        public IList ItemsSource
        {
            get { return _ItemsSource; }
            set { _ItemsSource = value; }
        }

        [Export("DisplayMemberPath"), Browsable(true)]
        public string DisplayMemberPath { get; set; }

        [Export("Title"), Browsable(true)]
        public string Title { get; set; }

        private object _SelectedItem;
        public object SelectedItem
        {
            get { return _SelectedItem; }
            set 
            { 
                _SelectedItem = value; 
                if (SelectionChanged != null)
                {
                    Dictionary<string, object> l_objDictionary = new Dictionary<string, object>();
                    l_objDictionary.Add("SelectedItem", value);
                    SelectionChanged(this, new EventDictionaryArgs(l_objDictionary));
                }
                if (_SelectedItem != null && ItemsSource != null)
                {
                    string l_strTitle = "";
                    if (!string.IsNullOrEmpty(DisplayMemberPath))
                        l_strTitle = GestionBindingExtensions.DataContextProperty<string>(_SelectedItem, DisplayMemberPath);
                    else
                        l_strTitle = _SelectedItem.ToString();

                    m_objButton.SetTitle(l_strTitle, UIControlState.Normal);
                }
                else
                    m_objButton.SetTitle("", UIControlState.Normal);
            }
        }       
        
    }
}