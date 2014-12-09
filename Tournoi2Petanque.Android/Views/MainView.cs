using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Framework.ViewModel;
using Tournoi2Petanque.ViewModels;

namespace Tournoi2Petanque.Views
{
    [Activity(Label = "Tournoi2Petanque.Android", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainView : XmView
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            this.DataContext = new MainViewModel();
        }

        public override void AddBindings()
        {
            this.AddCommand<Button>(Resource.Id.btnMainTournois, "TournoisCommand");
            this.AddCommand<Button>(Resource.Id.btnMainParticipants, "ParticipantsCommand");
        }
    }
}

