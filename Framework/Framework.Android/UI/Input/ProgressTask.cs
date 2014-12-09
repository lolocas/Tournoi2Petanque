using System;
using System.Collections.Generic;
using System.Text;
using Android.App;
using Android.Content;

namespace Framework.UI.Input
{
    class ProgressTask : IProgressTask, IDisposable
    {
        ProgressDialog _progressDialog;
        Activity _currentActivity;

        public ProgressTask(string p_strTitre, string p_strDescription, int p_intMax)
        {
            if (Services.CurrentActivity != null)
            {
                _currentActivity = Services.CurrentActivity;
                _currentActivity.RunOnUiThread(() =>
                {
                    _progressDialog = new ProgressDialog(Services.CurrentActivity);
                    _progressDialog.SetTitle(p_strTitre);
                    _progressDialog.SetMessage(p_strDescription);
                    _progressDialog.SetProgressStyle(ProgressDialogStyle.Horizontal);
                    _progressDialog.Max = p_intMax;
                    _progressDialog.SetCancelable(false);
                    _progressDialog.Show();
                });
            }
            else
                throw new Exception("ProgressTask p_Element not a Context type");
        }

        public void EndTask()
        {
            if (_progressDialog != null && _currentActivity != null)
            {
                _currentActivity.RunOnUiThread(() =>
                {
                    _progressDialog.Dismiss();
                    _progressDialog.Dispose();
                    _progressDialog = null;
                });
            }
        }

        public void Dispose()
        {
            EndTask();
        }

        public int Progress
        {
            get
            {
                return _progressDialog.Progress;
            }
            set
            {
                 _currentActivity.RunOnUiThread(() => _progressDialog.Progress = value);
            }
        }
    }
}
