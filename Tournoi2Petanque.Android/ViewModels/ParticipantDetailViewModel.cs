using System;
using System.Windows.MVVM;
using Tournoi2Petanque.DataService;
using Tournoi2Petanque.Models;

namespace Tournoi2Petanque.ViewModels
{
    public class ParticipantDetailViewModel : ViewModel
    {
        private int CleParticipant = 0;

        #region === PROPERTIES =========================================================

        #region === Property : Nom ===

        private string _Nom = default(string);
        /// <summary>
        /// Property : Nom
        /// </summary>
        public string Nom
        {
            get { return GetValue<ParticipantDetailViewModel, string>(NomDefinition); }
            set { SetValue<ParticipantDetailViewModel, string>(NomDefinition, value); }
        }
        public readonly static ViewModelPropertyDefinition<ParticipantDetailViewModel, string> NomDefinition
            = new ViewModelPropertyDefinition<ParticipantDetailViewModel, string>((vm) => vm.Nom, (vm) => vm._Nom);

        #endregion

        #region === Property : Prenom ===

        private string _Prenom = default(string);
        /// <summary>
        /// Property : Prenom
        /// </summary>
        public string Prenom
        {
            get { return GetValue<ParticipantDetailViewModel, string>(PrenomDefinition); }
            set { SetValue<ParticipantDetailViewModel, string>(PrenomDefinition, value); }
        }
        public readonly static ViewModelPropertyDefinition<ParticipantDetailViewModel, string> PrenomDefinition
            = new ViewModelPropertyDefinition<ParticipantDetailViewModel, string>((vm) => vm.Prenom, (vm) => vm._Prenom);

        #endregion

        #region === Property : Surnom ===

        private string _Surnom = default(string);
        /// <summary>
        /// Property : Surnom
        /// </summary>
        public string Surnom
        {
            get { return GetValue<ParticipantDetailViewModel, string>(SurnomDefinition); }
            set { SetValue<ParticipantDetailViewModel, string>(SurnomDefinition, value); }
        }
        public readonly static ViewModelPropertyDefinition<ParticipantDetailViewModel, string> SurnomDefinition
            = new ViewModelPropertyDefinition<ParticipantDetailViewModel, string>((vm) => vm.Surnom, (vm) => vm._Surnom);

        #endregion

        #endregion === PROPERTIES =========================================================

        #region === METHODS (private / protected) ======================================

        protected override void OnAttachView(IView View)
        {
            base.OnAttachView(View);

            ParticipantModel l_objParticipantModel = null;
            if (View.CurrentArguments != null && View.CurrentArguments.ContainsKey("Participant"))
            {
                l_objParticipantModel = View.CurrentArguments["Participant"] as ParticipantModel;
                CleParticipant = l_objParticipantModel.CleParticipant;
                Nom = l_objParticipantModel.Nom;
                Prenom = l_objParticipantModel.Prenom;
                Surnom = l_objParticipantModel.Surnom;
            }
        }

        protected override void OnAfterEndEdit()
        {
            base.OnAfterEndEdit();

            ParticipantModel l_objParticipantModel = new ParticipantModel
                {
                    CleParticipant = CleParticipant,
                    Nom = this.Nom,
                    Prenom = this.Prenom,
                    Surnom = this.Surnom
                };

            if (CleParticipant > 0)
                DataBaseModelService<ParticipantModel>.UpdateModel(l_objParticipantModel);
            else
                DataBaseModelService<ParticipantModel>.AddModel(l_objParticipantModel);
            Services.NavigationService.CloseXmView();
        }

        protected override bool OnBeforeValidate(object parameter)
        {
            bool l_blnReturn = true;
            if (string.IsNullOrEmpty(Nom))
            {
                DoValidateError(() => Nom, "Saisie obligatoire");
                l_blnReturn = false;
            }
            if (string.IsNullOrEmpty(Prenom))
            {
                DoValidateError(() => Prenom, "Saisie obligatoire");
                l_blnReturn = false;
            }
            return l_blnReturn;
        }


        #endregion === METHODS (private / protected) ======================================
    }
}
