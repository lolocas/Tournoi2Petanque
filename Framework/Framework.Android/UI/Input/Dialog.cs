using System;
using System.Collections.Generic;
using System.Text;
using Android.App;
using System.Threading.Tasks;

namespace Framework.UI.Input
{
    public class Dialog : IDialog
    {
        AlertDialog.Builder adb;
        Action<string> _CallBack;
        Activity _Element;        

        public Dialog(Activity p_Element, string p_strTitre, string p_strMessage, int p_intIconID)
        {
            _Element = p_Element;
            adb = new AlertDialog.Builder(p_Element);
            adb.SetTitle(p_strTitre);
            adb.SetMessage(p_strMessage);
            adb.SetIcon(p_intIconID);
        }
        public void Show(Action<string> CallBack)
        {
            _Element.RunOnUiThread(() =>
            {
                if (_withYes)
                {
                    adb.SetPositiveButton(Android.Resource.String.Yes, (s, e) =>
                    {
                        this.OnYesClick(s, e);
                    });
                }
                if (_withNo)
                {
                    adb.SetNegativeButton(Android.Resource.String.No, (s, e) =>
                    {
                        this.OnNoClick(s, e);
                    });
                }
                if (_withDetails)
                {
                    adb.SetNeutralButton("Détails", (s, e) =>
                    {
                        this.OnDetailsClick(s, e);
                    });
                }

                _CallBack = CallBack;
                adb.Show();
            });
        }

        public System.Threading.Tasks.Task<string> Show()
        {
            var tcs = new TaskCompletionSource<string>();
            if (_withYes)
            {
                adb.SetPositiveButton(Android.Resource.String.Yes, (s, e) =>
                {
                    tcs.TrySetResult("YES");
                });
            }
            if (_withNo)
            {
                adb.SetNegativeButton(Android.Resource.String.No, (s, e) =>
                {
                    tcs.TrySetResult("NO");
                });
            }

            adb.Show();
            return tcs.Task;
        }

        private bool _withYes;
        public bool WithYes
        {
            get
            {
                return _withYes;
            }
            set
            {
                _withYes = value;
                //if (value)
                //    adb.SetPositiveButton(Android.Resource.String.Yes, (s, e) =>
                //    {
                //        this.OnYesClick(s, e);
                //    });
            }
        }

        void OnYesClick(object sender, EventArgs e)
        {
            _CallBack("YES");
        }

        private bool _withNo;
        public bool WithNo
        {
            get
            {
                return _withNo;
            }
            set
            {
                _withNo = value;
                //if (value)
                //    adb.SetNegativeButton(Android.Resource.String.No, (s, e) =>
                //    {
                //        this.OnNoClick(s, e);
                //    });
            }
        }

        void OnNoClick(object sender, EventArgs e)
        {
            _CallBack("NO");
        }

        private bool _withOk;
        public bool WithOk
        {
            get
            {
                return _withOk;
            }
            set
            {
                _withOk = value;
                if (value)
                    adb.SetPositiveButton(Android.Resource.String.Ok, (s, e) =>
                    {
                        this.OnOkClick(s, e);
                    });
            }
        }
        void OnOkClick(object sender, EventArgs e)
        {
            _CallBack("OK");
        }

        private bool _withDetails;
        public bool WithDetails
        {
            get
            {
                return _withDetails;
            }
            set
            {
                _withDetails = value;
                //if (value)
                //    adb.SetNeutralButton("Détails", (s, e) =>
                //    {
                //        this.OnDetailsClick(s, e);
                //    });
            }
        }
        void OnDetailsClick(object sender, EventArgs e)
        {
            _CallBack("DETAILS");
        }
    }
}
