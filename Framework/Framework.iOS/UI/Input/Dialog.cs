using MonoTouch.UIKit;
using System;
using System.Threading.Tasks;

namespace Framework.UI.Input
{
    public class Dialog : IDialog
    {
        Action<string> _CallBack;
        static UIAlertView m_objAlert;

        public Dialog(string p_strTitre, string p_strMessage)
        {
            m_objAlert = new UIAlertView 
            {
                Title = p_strTitre,
                Message = p_strMessage
            };
        }

        public void Show(Action<string> CallBack)
        {
            m_objAlert.Clicked += (s, e) =>
            {
                if (m_objAlert.ButtonTitle(e.ButtonIndex) == "Oui")
                    _CallBack("YES");
                else if (m_objAlert.ButtonTitle(e.ButtonIndex) == "Non")
                    _CallBack("NO");
                else if (m_objAlert.ButtonTitle(e.ButtonIndex) == "Détails")
                    _CallBack("DETAILS");
            };
            _CallBack = CallBack;
            m_objAlert.Show();            
        }
        public Task<string> Show()
        {
            var tcs = new TaskCompletionSource<string>();
            m_objAlert.Clicked += (s, e) =>
                {
                    if (m_objAlert.ButtonTitle(e.ButtonIndex) == "Oui")
                        tcs.TrySetResult("YES");
                    else
                        tcs.TrySetResult("NO");
                };
            m_objAlert.Show();
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
                m_objAlert.AddButton("Oui");
            }
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
                m_objAlert.AddButton("Non");
            }
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
                m_objAlert.AddButton("OK");
            }
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
                m_objAlert.AddButton("Détails");
            }
        }
    }
}
