using Framework.ViewModel;
using MonoTouch.Foundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tournoi2Petanque.Views
{
    [Register("ParticipantListView")]
    public class ParticipantListView : XmUIViewController
    {
        public ParticipantListView(IntPtr handle)
            : base(handle)
		{
		}

        public override void AddBinding()
        {
            throw new NotImplementedException();
        }
    }
}
