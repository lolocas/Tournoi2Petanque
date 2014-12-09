using Android.App;
using Android.OS;
using Android.Widget;
using Framework.ViewModel;
using System.Collections.Generic;
using System.Windows.MVVM;
using Tournoi2Petanque.ViewModels;

namespace Tournoi2Petanque.Views
{
    [Activity(Label = "ParticipantListView")]
    public class ParticipantListView : XmView
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ParticipantList);

            this.DataContext = new ParticipantListViewModel();
        }

        public override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();
        }

        public override void AddBindings()
        {
            this.AddBinding<ListView>(Resource.Id.lstParticipantList, ActionProperty.SelectedItem, ParticipantListViewModel.SelectedParticipantDefinition, BindingMode.TwoWay);

            this.AddBinding<ListView>(new BindingDescription
                {
                    ViewId = Resource.Id.lstParticipantList,
                    Action = ActionProperty.ItemsSource,
                    SourcePropertyPath = ParticipantListViewModel.ListeParticipantDefinition.PropertyName,
                    SubDataContext = ParticipantContextDescription()
                });

            this.AddCommand<Button>(Resource.Id.btnParticipantListAdd, "ParticipantAddCommand");
        }

        private DataContextDescription ParticipantContextDescription()
        {
            DataContextDescription ListParticipantsSubDataContext = new DataContextDescription();

            List<BindingDescription> ListParticipantsBindingList = new List<BindingDescription>();
            ListParticipantsBindingList.Add(new BindingDescription(typeof(TextView), null, Resource.Id.txtParticipantListItemNom, ActionProperty.Text, "Nom"));
            ListParticipantsBindingList.Add(new BindingDescription(typeof(TextView), null, Resource.Id.txtParticipantListItemPrenom, ActionProperty.Text, "Prenom"));
            ListParticipantsSubDataContext.Bindings = ListParticipantsBindingList;

            List<CommandDescription> lstParticipantsCommandList = new List<CommandDescription>();
            lstParticipantsCommandList.Add(new CommandDescription(typeof(Button), Resource.Id.btnParticipantListItemDelete, "DeleteCommand", "SelectedItem"));
            ListParticipantsSubDataContext.Commands = lstParticipantsCommandList;

            ListParticipantsSubDataContext.SubBindingDescription = new BindingDescription()
            {
                ViewId = Resource.Layout.ParticipantListItem
            };
            return ListParticipantsSubDataContext;
        }
    }
}
