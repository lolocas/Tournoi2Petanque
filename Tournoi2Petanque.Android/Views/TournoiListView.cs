using Android.App;
using Android.OS;
using Framework.ViewModel;

namespace Tournoi2Petanque.Views
{
    [Activity(Label = "TournoiListView")]
    public class TournoiListView : XmView
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.TournoiList);
        }

        public override void AddBindings()
        {
            throw new System.NotImplementedException();
        }
    }
}