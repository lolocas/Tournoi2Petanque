using System;
using System.Collections.Generic;
using System.Text;
using Android.App;
using Android.Content;
using System.Collections;

namespace Framework.UI.Input
{
    public class DialogsService
    {
        public void ShowErrorDialog(Exception p_objException, string p_strMessage = "" )
        {
            if (string.IsNullOrEmpty(p_strMessage))
                p_strMessage = "Erreur inattendue, si le problème persiste veuillez contacter le support";
            if (Services.CurrentActivity != null)
            {
                Services.CurrentActivity.RunOnUiThread(() =>
                {
                    CreateExceptionDialog(p_strMessage).Show(str =>
                        {
                            if (str == "DETAILS")
                                CreateDialog("Erreur fatale", p_objException.Message + "\n\r" + p_objException.StackTrace, Android.Resource.Drawable.IcDialogAlert).Show();
                        });
                });
            }
            else
                throw new Exception("ShowErrorDialog Services.CurrentActivity not set");
        }
        public void ShowErrorDialog(string p_strTitre, string p_strMessage)
        {
            if (Services.CurrentActivity != null)
            {
                Services.CurrentActivity.RunOnUiThread(() =>
                {
                    CreateDialog(p_strTitre, p_strMessage, Android.Resource.Drawable.IcDialogAlert).Show();
                });
            }
            else
                throw new Exception("ShowErrorDialog Services.CurrentActivity not a Context type");
        }

        public IDialog CreateErrorDialog(string p_strTitre, string p_strMessage)
        {
            if (Services.CurrentActivity != null)
            {
                var dialog = new Dialog(Services.CurrentActivity, p_strTitre, p_strMessage, Android.Resource.Drawable.IcDialogAlert);
                dialog.WithOk = true;
                return dialog;
            }
            else
                throw new Exception("ShowErrorDialog Services.CurrentActivity not a Context type");
        }

        public void ShowInfoDialog(string p_strTitre, string p_strMessage)
        {
            if (Services.CurrentActivity != null)
            {
                Services.CurrentActivity.RunOnUiThread(() =>
                {
                    CreateDialog(p_strTitre, p_strMessage, Android.Resource.Drawable.IcDialogInfo).Show();
                });
            }
            else
                throw new Exception("ShowErrorDialog Services.CurrentActivity not a Context type");
        }

        public IDialog CreateYesNoDialog(string p_strTitre, string p_strMessage)
        {
            if (Services.CurrentActivity != null)
            {
                var dialog = new Dialog(Services.CurrentActivity, p_strTitre, p_strMessage, Android.Resource.Drawable.IcDialogInfo);
                dialog.WithYes = true;
                dialog.WithNo = true;
                return dialog;
            }
            else
                throw new Exception("CreateYesNoDialog Services.CurrentActivity not a Context type");
        }

        public void ShowToast(string p_strTitre)
        {
            if (Services.CurrentActivity != null)
            {
                Services.CurrentActivity.RunOnUiThread(() =>
                {
                    Android.Widget.Toast.MakeText(Services.CurrentActivity, p_strTitre, Android.Widget.ToastLength.Short).Show();
                });
            }
            else
                throw new Exception("ShowToast Services.CurrentActivity not a Context type");
            
        }

        public void ShowListDialog(string p_strTitre, string[] p_lstItems, string p_strDisplayMemberPath = "", object p_objSelectedItem = null, Action<object> p_objCallBack = null)
        {
            if (p_lstItems == null)
                return;

            AlertDialog dialog;
            AlertDialog.Builder builder = new AlertDialog.Builder(Services.CurrentActivity);
            builder.SetTitle(p_strTitre);
            builder.SetItems(p_lstItems, (ss, es) =>
            {
                p_objCallBack(p_lstItems[es.Which]);                
            });
            dialog = builder.Create();
            dialog.Show();  
        }

        private AlertDialog.Builder CreateDialog(string p_strTitre, string p_strMessage, int p_intIconID)
        {
            AlertDialog.Builder adb = new AlertDialog.Builder(Services.CurrentActivity);
            adb.SetTitle(p_strTitre);
            adb.SetMessage(p_strMessage);
            adb.SetIcon(p_intIconID);
            adb.SetPositiveButton(Android.Resource.String.Ok, delegate { });
            return adb;
        }

        private IDialog CreateExceptionDialog(string p_strMessage)
        {
            if (Services.CurrentActivity != null)
            {
                var dialog = new Dialog(Services.CurrentActivity, "Erreur inattendue", p_strMessage, Android.Resource.Drawable.IcDialogAlert);
                dialog.WithOk = true;
                dialog.WithDetails = true;                
                return dialog;
            }
            else
                throw new Exception("CreateYesNoDialog Services.CurrentActivity not a Context type");
        }

    }
}
