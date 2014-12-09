using Android.App;
using Android.OS;
using Android.Widget;
using Framework.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.MVVM;
using Tournoi2Petanque.Models;
using Tournoi2Petanque.ViewModels;

namespace Tournoi2Petanque.Views
{
    [Activity(Label = "ParticipantDetailView")]
    public class ParticipantDetailView : XmView
    {
        protected override void OnCreate(Dictionary<string, object> p_dicArguments)
        {
            base.OnCreate(p_dicArguments);
            SetContentView(Resource.Layout.ParticipantDetail);
            this.DataContext = new ParticipantDetailViewModel();
        }

        public override void AddBindings()
        {
            this.AddBinding<EditText>(Resource.Id.txtParticipantDetailNom, ActionProperty.Text, ParticipantDetailViewModel.NomDefinition, BindingMode.TwoWay);
            this.AddBinding<EditText>(Resource.Id.txtParticipantDetailPrenom, ActionProperty.Text, ParticipantDetailViewModel.PrenomDefinition, BindingMode.TwoWay);
            this.AddBinding<EditText>(Resource.Id.txtParticipantDetailSurnom, ActionProperty.Text, ParticipantDetailViewModel.SurnomDefinition, BindingMode.TwoWay);

            this.AddCommand<Button>(Resource.Id.btnParticipantDetailValider, "ValidateCommand");
        }
    }
}
