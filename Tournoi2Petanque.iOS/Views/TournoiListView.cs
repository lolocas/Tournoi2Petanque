using System;
using System.Drawing;

using MonoTouch.CoreFoundation;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Framework.ViewModel;

namespace Tournoi2Petanque.Views
{
    [Register("TournoiListView")]
    public class TournoiListView : XmUIViewController
    {
        public TournoiListView(IntPtr handle)
            : base(handle)
		{
		}

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {

            base.ViewDidLoad();
        }

        public override void AddBinding()
        {
            throw new NotImplementedException();
        }
    }
}