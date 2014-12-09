using System;
using System.Windows.Input;
using System.Windows.MVVM;
using Tournoi2Petanque.DataService;
using Tournoi2Petanque.Views;

namespace Tournoi2Petanque.ViewModels
{
    class MainViewModel : ViewModel
    {
        #region === COMMANDS ===========================================================

        #region === Property : TournoisCommand ===
        /// <summary>
        /// Property : TournoisCommand
        /// </summary>
        private ICommand _TournoisCommand;
        public ICommand TournoisCommand
        {
            get
            {
                return _TournoisCommand = _TournoisCommand ?? new RelayCommand(OnTournois, () => CanTournoisCommand);
            }
        }

        private void OnTournois(object parameter)
        {
            Services.NavigationService.OpenXmView<TournoiListView>(null);
        }

        private bool _CanTournoisCommand = true;
        public bool CanTournoisCommand
        {
            private get
            {
                return _CanTournoisCommand;
            }
            set
            {
                _CanTournoisCommand = value;
                ((RelayCommand)TournoisCommand).RaiseCanExecuteChanged();
            }
        }
        #endregion

        #region === Property : ParticipantsCommand ===
        /// <summary>
        /// Property : ParticipantsCommand
        /// </summary>
        private ICommand _ParticipantsCommand;
        public ICommand ParticipantsCommand
        {
            get
            {
                return _ParticipantsCommand = _ParticipantsCommand ?? new RelayCommand(OnParticipants, () => CanParticipantsCommand);
            }
        }

        private void OnParticipants(object parameter)
        {
            Services.NavigationService.OpenXmView<ParticipantListView>(null);
        }

        private bool _CanParticipantsCommand = true;
        public bool CanParticipantsCommand
        {
            private get
            {
                return _CanParticipantsCommand;
            }
            set
            {
                _CanParticipantsCommand = value;
                ((RelayCommand)ParticipantsCommand).RaiseCanExecuteChanged();
            }
        }
        #endregion

        #endregion

        #region === METHODS (private / protected) ======================================

        protected override void OnAttachView(IView View)
        {
            base.OnAttachView(View);
            DatabaseService.Init();
        }

        #endregion
    }
}
