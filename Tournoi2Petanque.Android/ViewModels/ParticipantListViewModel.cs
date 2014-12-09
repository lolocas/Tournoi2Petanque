using Framework.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.MVVM;
using Tournoi2Petanque.DataService;
using Tournoi2Petanque.Models;
using Tournoi2Petanque.Views;
using System.Linq;

namespace Tournoi2Petanque.ViewModels
{
    public class ParticipantListViewModel : ViewModel
    {
        #region === PROPERTIES =========================================================

        #region === Property : SelectedParticipant ===

        private ParticipantModel _SelectedParticipant = default(ParticipantModel);
        /// <summary>
        /// Property : SelectedParticipant
        /// </summary>
        public ParticipantModel SelectedParticipant
        {
            get { return GetValue<ParticipantListViewModel, ParticipantModel>(SelectedParticipantDefinition); }
            set { SetValue<ParticipantListViewModel, ParticipantModel>(SelectedParticipantDefinition, value); }
        }
        public readonly static ViewModelPropertyDefinition<ParticipantListViewModel, ParticipantModel> SelectedParticipantDefinition
            = new ViewModelPropertyDefinition<ParticipantListViewModel, ParticipantModel>((vm) => vm.SelectedParticipant, (vm) => vm._SelectedParticipant)
        {
            ValueChanged = (vm, oldValue, NewValue) =>
            {
                Dictionary<string, object> l_objDictionary = new Dictionary<string, object>();

                l_objDictionary.Add("Participant", NewValue);

                Services.NavigationService.OpenXmView<ParticipantDetailView>(l_objDictionary);
            }
        };

        #endregion

        #region === Property : ListeParticipant ===

        private ObservableCollection<ParticipantModel> _ListeParticipant = default(ObservableCollection<ParticipantModel>);
        /// <summary>
        /// Property : ListeParticipant
        /// </summary>
        public ObservableCollection<ParticipantModel> ListeParticipant
        {
            get { return GetValue<ParticipantListViewModel, ObservableCollection<ParticipantModel>>(ListeParticipantDefinition); }
            set { SetValue<ParticipantListViewModel, ObservableCollection<ParticipantModel>>(ListeParticipantDefinition, value); }
        }
        public readonly static ViewModelPropertyDefinition<ParticipantListViewModel, ObservableCollection<ParticipantModel>> ListeParticipantDefinition
            = new ViewModelPropertyDefinition<ParticipantListViewModel, ObservableCollection<ParticipantModel>>((vm) => vm.ListeParticipant, (vm) => vm._ListeParticipant);

        #endregion

        #endregion === PROPERTIES =========================================================

        #region === COMMANDS ===========================================================

        #region === Property : ParticipantAddCommand ===
        /// <summary>
        /// Property : ParticipantAddCommand
        /// </summary>
        private ICommand _ParticipantAddCommand;
        public ICommand ParticipantAddCommand
        {
            get
            {
                return _ParticipantAddCommand = _ParticipantAddCommand ?? new RelayCommand(OnParticipantAdd);
            }
        }

        private void OnParticipantAdd(object parameter)
        {
            Services.NavigationService.OpenXmView<ParticipantDetailView>(null);
        }
        #endregion

        #endregion === COMMANDS ===========================================================

        #region === METHODS (private / protected) ======================================

        protected override void OnAttachView(IView View)
        {
            base.OnAttachView(View);
            this.ListeParticipant = new ObservableCollection<ParticipantModel>(DataBaseModelService<ParticipantModel>.GetParticipants());
            DataBaseModelService<ParticipantModel>.ModelAdded += ParticipantListViewModel_ModelAdded;
            DataBaseModelService<ParticipantModel>.ModelModified +=ParticipantListViewModel_ModelModified;
            DataBaseModelService<ParticipantModel>.ModelDeleted +=ParticipantListViewModel_ModelDeleted;
        }

        void ParticipantListViewModel_ModelAdded(object sender, EventModelArgs<ParticipantModel> e)
        {
            this.ListeParticipant.Add(e.ModelValue);
        }

        void ParticipantListViewModel_ModelModified(object sender, EventModelArgs<ParticipantModel> e)
        {
            var l_objNewValue = this.ListeParticipant.FirstOrDefault(item => item.CleParticipant == e.ModelValue.CleParticipant);
            var l_intIndex = this.ListeParticipant.IndexOf(l_objNewValue);

            this.ListeParticipant[l_intIndex] = e.ModelValue;
        }

        void ParticipantListViewModel_ModelDeleted(object sender, EventModelArgs<ParticipantModel> e)
        {
            this.ListeParticipant.Remove(e.ModelValue);
        }

        protected override void OnDettachView(IView View)
        {
            base.OnDettachView(View);
            DataBaseModelService<ParticipantModel>.ModelAdded -= ParticipantListViewModel_ModelAdded;
            DataBaseModelService<ParticipantModel>.ModelModified -= ParticipantListViewModel_ModelModified;
            DataBaseModelService<ParticipantModel>.ModelDeleted -= ParticipantListViewModel_ModelDeleted;
        }

        #endregion
    }
}
