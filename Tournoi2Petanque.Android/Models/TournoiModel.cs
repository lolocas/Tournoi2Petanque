using Framework.Model;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tournoi2Petanque.Models
{
    public class TournoiModel : IModel
    {
        [PrimaryKey, AutoIncrement]
        public int CleTournoi { set; get; }
        public string Nom { set; get; }
        public List<int> ListeParticipants { set; get; }
        public List<PartieModel> ListeParties { set; get; }
    }
}
