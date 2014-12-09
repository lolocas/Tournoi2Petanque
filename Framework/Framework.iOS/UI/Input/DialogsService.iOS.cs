using Framework.ViewModel;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.Collections;
using System.Drawing;

namespace Framework.UI.Input
{
	public class DialogsService
	{
		MonoTouch.Foundation.NSObject _threadUI;

		public DialogsService()
		{
			_threadUI = new MonoTouch.Foundation.NSObject();
		}

		public void ShowErrorDialog(Exception p_objException, string p_strMessage = "")
		{
			_threadUI.BeginInvokeOnMainThread(() =>
			{
				if (string.IsNullOrEmpty(p_strMessage))
					p_strMessage = "Erreur inattendue, si le problème persiste veuillez contacter le support";

				CreateExceptionDialog(p_strMessage).Show(str =>
				{
					if (str == "DETAILS")
						CreateDialog("Erreur fatale", p_objException.Message + "\n\r" + p_objException.StackTrace).Show();
				});
			});
		}
		public void ShowErrorDialog(string p_strTitre, string p_strMessage)
		{
			_threadUI.BeginInvokeOnMainThread(() =>
			{
				using (UIAlertView alert = new UIAlertView(p_strTitre, p_strMessage, null, "OK", null))
				{
					alert.Show();
				}
			});
		}

		public IDialog CreateErrorDialog(string p_strTitre, string p_strMessage)
		{
			IDialog dialog = new Dialog(p_strTitre, p_strMessage);
			dialog.WithOk = true;
			return dialog;
		}

		public void ShowInfoDialog(string p_strTitre, string p_strMessage)
		{
			_threadUI.BeginInvokeOnMainThread(() =>
			{
				using (UIAlertView alert = new UIAlertView(p_strTitre, p_strMessage, null, "OK", null))
				{
					alert.Show();
				}
			});
		}

		public IDialog CreateYesNoDialog(string p_strTitre, string p_strMessage)
		{
			var dialog = new Dialog(p_strTitre, p_strMessage);
			dialog.WithYes = true;
			dialog.WithNo = true;
			return dialog;
		}

		public void ShowToast(string p_strTitre)
		{
			_threadUI.BeginInvokeOnMainThread(() =>
			{
				ToastView l_objToast = new ToastView(p_strTitre, 2000);
				l_objToast.Show();
			});
		}

		/// <summary>
		/// Affiche une fenêtre modale contenant un liste sous forme de grille
		/// </summary>
		/// <param name="p_strTitre">Le titre de la fenêtre</param>
		/// <param name="p_lstItems">La liste d'item</param>
		/// <param name="p_strDisplayMemberPath">Le nom de variable à afficher</param>
		/// <param name="p_objSelectedItem">La ligne sélectionnée pour afficher un checkmark</param>
		/// <param name="p_objCallBack">La ligne sélectionnée en Callback</param>
		public void ShowListDialog(string p_strTitre, IList p_lstItems, string p_strDisplayMemberPath = "", object p_objSelectedItem = null, Action<object> p_objCallBack = null)
		{
			if (p_lstItems == null || p_lstItems.Count == 0)
				return;

			UIView l_objView = new UIView(new RectangleF(10, 20, Services.CurrentActivity.View.Bounds.Width * 0.95f, Services.CurrentActivity.View.Bounds.Height * 0.8f));
			l_objView.Layer.BorderWidth = 1;
			l_objView.Layer.BorderColor = UIColor.Black.CGColor;

			int l_intLabelHeight = 0;
			if (!string.IsNullOrEmpty(p_strTitre))
			{
                UILabel l_objLabel = new UILabel(new RectangleF(0, 0, l_objView.Bounds.Width, 40));
				l_objLabel.Text = p_strTitre;
				l_objLabel.TextColor = UIColor.White;
				l_objLabel.BackgroundColor = UIColor.Black;
				l_objLabel.TextAlignment = UITextAlignment.Center;
				l_intLabelHeight = 40;
				l_objView.AddSubview(l_objLabel);

                UIButton l_objButtonClose = new UIButton(new RectangleF(l_objView.Bounds.Width - 20, 0, 20, 20));
				l_objButtonClose.SetTitle("X", UIControlState.Normal);
				l_objButtonClose.TouchUpInside += (s, e) =>
				{
					l_objView.RemoveFromSuperview();
				};
				l_objView.AddSubview(l_objButtonClose);
			}
            UITableView l_objTableView = new UITableView(new RectangleF(0, l_intLabelHeight, l_objView.Bounds.Width, l_objView.Bounds.Height - l_intLabelHeight));
			l_objView.AddSubview(l_objTableView);


			l_objTableView.Source = new CustomComboCell(p_lstItems, p_strDisplayMemberPath, p_objSelectedItem, (selecteditem) =>
			{
				p_objCallBack(selecteditem);
				l_objView.RemoveFromSuperview();
			});

			Services.CurrentActivity.View.AddSubview(l_objView);
		}

		private class CustomComboCell : UITableViewSource
		{
			IList m_lstItems;
			string m_strDisplayMemberPath;
			Action<object> m_objCallBack;
			object m_objSelectedItem;

			public CustomComboCell(IList p_lstItems, string p_strDisplayMemberPath = "", object p_objSelectedItem = null, Action<object> p_objCallBack = null)
			{
				m_lstItems = p_lstItems;
				m_strDisplayMemberPath = p_strDisplayMemberPath;
				m_objCallBack = p_objCallBack;
				m_objSelectedItem = p_objSelectedItem;
			}

			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{
				UITableViewCell cell = new UITableViewCell(UITableViewCellStyle.Default, "CustomCell");
				if (!string.IsNullOrEmpty(m_strDisplayMemberPath))
					cell.TextLabel.Text = GestionBindingExtensions.DataContextProperty<string>(m_lstItems[indexPath.Row], m_strDisplayMemberPath);
				else
					cell.TextLabel.Text = m_lstItems[indexPath.Row].ToString();
				if (m_objSelectedItem != null && m_lstItems[indexPath.Row] == m_objSelectedItem)
					cell.Accessory = UITableViewCellAccessory.Checkmark;

				return cell;
			}

			public override int RowsInSection(UITableView tableview, int section)
			{
				if (m_lstItems != null)
					return m_lstItems.Count;
				return 0;
			}

			public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
			{
				m_objCallBack(m_lstItems[indexPath.Row]);
			}
		}


		private UIAlertView CreateDialog(string p_strTitre, string p_strMessage)
		{
			UIAlertView l_objAlertView = new UIAlertView(p_strTitre, p_strMessage, null, "OK", null);
			return l_objAlertView;
		}

		private IDialog CreateExceptionDialog(string p_strMessage)
		{
			var dialog = new Dialog("Erreur inattendue", p_strMessage);
			dialog.WithOk = true;
			dialog.WithDetails = true;
			return dialog;
		}

		public void CreateModale(string p_strTitre, string p_strMessage)
		{
			_threadUI.InvokeOnMainThread(() =>
			{
				if (Services.CurrentModale == null)
				{
					Services.CurrentModale = new AlertView();
				}
				Services.CurrentModale.Titre = p_strTitre;
				Services.CurrentModale.Message = p_strMessage;
			});
		}

		public class AlertView : UIView
		{
			UILabel m_lblTitre;
			UILabel m_lblMessage;
			UIView m_objCurrentView;

			private string _titre;
			public string Titre
			{
				get
				{
					return _titre;
				}
				set
				{
					_titre = value;
					m_lblTitre.Text = _titre;
				}
			}

			private string _message;
			public string Message
			{
				get
				{
					return _message;
				}
				set
				{
					_message = value;
					m_lblMessage.Text = _message;
				}
			}

			public AlertView()
			{
				 this.Frame = UIScreen.MainScreen.Bounds;

				float centerX = this.Frame.Width / 2;
				float centerY = this.Frame.Height / 2;
				float labelHeight = 22;
				float labelWidth = this.Frame.Width - 20;

				this.BackgroundColor = UIColor.Black;
				this.Alpha = 0.75f;
				this.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;

                m_lblTitre = new UILabel(new RectangleF(centerX - (labelWidth / 2), (this.Frame.Height / 3), labelWidth, labelHeight));
				m_lblTitre.BackgroundColor = UIColor.Clear;
				m_lblTitre.TextColor = UIColor.White;
				m_lblTitre.TextAlignment = UITextAlignment.Center;
				m_lblTitre.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
				AddSubview(m_lblTitre);

                m_lblMessage = new UILabel(new RectangleF(centerX - (labelWidth / 2), centerY, labelWidth, labelHeight));
				m_lblMessage.BackgroundColor = UIColor.Clear;
				m_lblMessage.TextColor = UIColor.White;
				m_lblMessage.TextAlignment = UITextAlignment.Center;
				m_lblMessage.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
				AddSubview(m_lblMessage);

				Services.CurrentActivity.Add(this);
			}

			public void AddCustomView(UIView p_objCurrentView)
			{
				if (m_objCurrentView != null)
					m_objCurrentView.RemoveFromSuperview();
				m_objCurrentView = p_objCurrentView;
				this.AddSubview(m_objCurrentView);
			}

			public void EndView()
			{
				UIView.Animate(0.5, () => { Alpha = 0; }, () => { RemoveFromSuperview();});
				Services.CurrentModale = null;
			}
		}
	}
}

