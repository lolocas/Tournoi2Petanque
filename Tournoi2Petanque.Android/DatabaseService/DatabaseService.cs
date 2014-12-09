using Framework.Events;
using Framework.Model;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tournoi2Petanque.Models;

namespace Tournoi2Petanque.DataService
{
    public static class DatabaseService
    {
        public static SQLiteConnection m_objDB;

        public static void Init()
        {
            m_objDB = GetConnection();
            m_objDB.CreateTable<ParticipantModel>();
        }

        private static SQLiteConnection GetConnection()
        {
            #if __ANDROID__
            string l_strDirectory = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "Tournoi2Petanque");
            #elif __IOS__
            string l_strDirectory = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Tournoi2Petanque");
            #endif
            if (!Directory.Exists(l_strDirectory))
                Directory.CreateDirectory(l_strDirectory);
            string l_strPath = Path.Combine(l_strDirectory, "Tournoi2Petanque.db");
#if __ANDROID__
            return new SQLiteConnection(new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid(), l_strPath);
#elif __IOS__
            return new SQLiteConnection(new SQLite.Net.Platform.XamarinIOS.SQLitePlatformIOS(), l_strPath);
#endif
        }
    }

    public static class DataBaseModelService<TModel> where TModel : IModel
    {
        public static event EventHandler<EventModelArgs<TModel>> ModelDeleted;
        public static event EventHandler<EventModelArgs<TModel>> ModelAdded;
        public static event EventHandler<EventModelArgs<TModel>> ModelModified;


        public static void AddModel(TModel p_objModel)
        {
            DatabaseService.m_objDB.Insert(p_objModel, typeof(TModel));
            if (ModelAdded != null)
                ModelAdded(null, new EventModelArgs<TModel>(p_objModel));
        }

        public static void UpdateModel(TModel p_objModel)
        {
            DatabaseService.m_objDB.Update(p_objModel, typeof(TModel));
            if (ModelModified != null)
                ModelModified(null, new EventModelArgs<TModel>(p_objModel));
        }

        public static void DeleteModel(TModel p_objModel)
        {
            DatabaseService.m_objDB.Delete(p_objModel);
            if (ModelDeleted != null)
                ModelDeleted(null, new EventModelArgs<TModel>(p_objModel));

        }

        public static List<ParticipantModel> GetParticipants()
        {
            return DatabaseService.m_objDB.Table<ParticipantModel>().ToList();
        }
    }
}
