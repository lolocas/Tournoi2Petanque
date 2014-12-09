using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MonoTouch.UIKit;

namespace Framework.UI.Input
{
    class ProgressTask : IProgressTask, IDisposable
    {
        UIView m_viewModale;
        UIView m_viewProgress;
        UILabel m_lblProgress;
        UIProgressView m_progView;
        int m_intMax;
        MonoTouch.Foundation.NSObject _threadUI;

        public ProgressTask(string p_strTitre, string p_strDescription, int p_intMax)
        {
            m_intMax = p_intMax;
            _threadUI = new MonoTouch.Foundation.NSObject();
            _threadUI.InvokeOnMainThread(() =>
            {
                Services.DialogsService.CreateModale(p_strTitre, p_strDescription);

                m_viewModale = new UIView();
                m_viewModale.Frame = Services.CurrentActivity.View.Frame;

                m_viewProgress = new UIView(new RectangleF(10, (Services.CurrentActivity.View.Bounds.Height - 70) / 2, Services.CurrentActivity.View.Bounds.Width * 0.95f, 70));
                m_viewProgress.BackgroundColor = UIColor.Clear;

                m_progView = new UIProgressView(new RectangleF(20, 40, m_viewProgress.Bounds.Width - 40, 0));
                m_viewProgress.AddSubview(m_progView);

                m_lblProgress = new UILabel(new RectangleF(5, 50, m_viewProgress.Bounds.Width - 5, 20));
                m_lblProgress.Font = UIFont.SystemFontOfSize(12);
                m_lblProgress.TextColor = UIColor.White;
                m_viewProgress.AddSubview(m_lblProgress);

                m_viewModale.AddSubview(m_viewProgress);
                Services.CurrentModale.AddSubview(m_viewModale);
            });
        }

        public void Dispose()
        {
            EndTask();
        }

        public int Progress
        {
            get
            {
                return (int)(m_progView.Progress * m_intMax);
            }
            set
            {
                _threadUI.InvokeOnMainThread(() =>
                {
                if (m_intMax > 0)
                    {
                        m_progView.Progress = (float)(value) / m_intMax; //+1 -1 pour forcer le float
                        m_lblProgress.Text = value + "/" + m_intMax;
                    }
                });

            }
        }

        public void EndTask()
        {
            if (m_viewProgress != null && m_progView != null)
            {
                _threadUI.InvokeOnMainThread(() =>
                {
                    m_viewModale.RemoveFromSuperview();
                    m_viewModale.Dispose();
                    m_viewModale = null;
                    Services.CurrentModale.EndView();
                });
            }
        }
    }
}
