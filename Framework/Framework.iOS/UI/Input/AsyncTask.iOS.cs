using MonoTouch.UIKit;
using System;
using System.Drawing;
//iOS
namespace Framework.UI.Input
{
    public class AsyncTask : IAsyncTask
	{
        MonoTouch.Foundation.NSObject _threadUI;

        public AsyncTask(string p_strTitre, string p_strMessage)
		{
            _threadUI = new MonoTouch.Foundation.NSObject();

            _threadUI.InvokeOnMainThread(() =>
            {
                Services.DialogsService.CreateModale(p_strTitre, p_strMessage);

                // derive the center x and y
                float centerX = Services.CurrentModale.Frame.Width / 2;
                float centerY = Services.CurrentModale.Frame.Height / 2;

                // create the activity spinner, center it horizontall and put it 5 points above center x
                UIActivityIndicatorView l_objActivityView = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
                l_objActivityView.Frame = new RectangleF(centerX - (l_objActivityView.Frame.Width / 2), centerY - l_objActivityView.Frame.Height - 20, l_objActivityView.Frame.Width, l_objActivityView.Frame.Height);
                l_objActivityView.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
                Services.CurrentModale.AddCustomView(l_objActivityView);
                l_objActivityView.StartAnimating();
            });
		}

        public void ModifyTask(string p_strTitre, string p_strMessage)
        {
            Services.CurrentModale.Titre = p_strTitre;
            Services.CurrentModale.Message = p_strMessage;            
        }

        public void EndTask()
        {
            _threadUI.InvokeOnMainThread(() =>
            {
                if (Services.CurrentModale != null)
                    Services.CurrentModale.EndView();
            });
        }
    }
}

