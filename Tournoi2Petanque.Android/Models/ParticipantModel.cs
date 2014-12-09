using Framework.Model;
using SQLite.Net.Attributes;
using System;
using System.Windows.Input;
using System.Windows.MVVM;
using Tournoi2Petanque.DataService;

namespace Tournoi2Petanque.Models
{
    public class ParticipantModel : Notifier, IModel
    {
        [PrimaryKey, AutoIncrement]
        public int CleParticipant { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Surnom { get; set; }

        #region === COMMANDS ===========================================================

        #region === Property : DeleteCommand ===
        /// <summary>
        /// Property : DeleteCommand
        /// </summary>
        public ICommand DeleteCommand
        {
            get
            {
                return new RelayCommand(
                    (param) =>
                    {
#if __ANDROID__ || WPF
                        Services.DialogsService.CreateYesNoDialog("Confirmation", "Confirmez-vous la suppression de ce participant ?").Show((str) =>
                        {
                            if (str == "YES")
                                DataBaseModelService<ParticipantModel>.DeleteModel(param as ParticipantModel);
                        });
#else //Pas de confirmation en iOS grace au swipe             
                        DatabaseService.DeleteModel<ParticipantModel>(param as ParticipantModel);
#endif
                    });


            }
        }
        #endregion

        #endregion

    }
}
