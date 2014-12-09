using System;
using System.Collections.Generic;
using System.Text;
using Android.App;
using Android.Content;

namespace Framework.UI.Input
{
    class AsyncTask : IAsyncTask, IDisposable
    {
        ProgressDialog _progressDialog;
        Activity _currentActivity;

        public AsyncTask(string p_strTitre, string p_strDescription)
        {
            if (Services.CurrentActivity != null)
            {
                _currentActivity = Services.CurrentActivity;
                _currentActivity.RunOnUiThread(() =>
                    _progressDialog = ProgressDialog.Show(Services.CurrentActivity, p_strTitre, p_strDescription, true)
                    );            
            }
            else
                throw new Exception("AsyncTask Services.CurrentActivity not a Context type");
        }

        public void ModifyTask(string p_strTitre, string p_strDescription)
        {
            if (Services.CurrentActivity != null)
            {
                _currentActivity = Services.CurrentActivity;
                _currentActivity.RunOnUiThread(() =>
                {
                    if (_progressDialog != null)
                    {
                        _progressDialog.SetTitle(p_strTitre);
                        _progressDialog.SetMessage(p_strDescription);
                    }
                });
            }
            else
                throw new Exception("AsyncTask Services.CurrentActivity not a Context type");        
        }

        public void EndTask()
        {
            if (_currentActivity != null)
            {
                _currentActivity.RunOnUiThread(() =>
                {
                    if (_progressDialog != null)
                    {
                        _progressDialog.Dismiss();
                        _progressDialog.Dispose();
                        _progressDialog = null;                        
                    }
                });
            }
        }

        public void Dispose()
        {
            EndTask();
        }
    }
}
