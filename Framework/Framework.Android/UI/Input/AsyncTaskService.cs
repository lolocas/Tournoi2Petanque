using System;

namespace Framework.UI.Input
{
    public class AsyncTaskService
    {
        /// <summary>
        /// Demarre une tache d'attente, attention cette partie doit se trouver en amont du thread sinon
        /// il peut y avoir des problèmes de fermeture de la tache d'attente.
        /// </summary>
        /// <param name="p_strTitre"></param>
        /// <param name="p_strDescription"></param>
        /// <returns></returns>
        public IAsyncTask BeginGlobalTask(string p_strTitre, string p_strDescription)
        {
            try
            {
                return new AsyncTask(p_strTitre, p_strDescription);
            }
            catch (Exception)
            {                
                throw;
            } 
        }

        public void EndGlobalTask(IAsyncTask p_objTask)
        {
            if (p_objTask != null)
            {
                p_objTask.EndTask();
                p_objTask = null;
            }
        }

        public IProgressTask BeginProgressTask(string p_strTitre, string p_strDescription, int p_intMax)
        {
            try
            {
                return new ProgressTask(p_strTitre, p_strDescription, p_intMax);
            }
            catch (Exception)
            {
                throw;
            } 
        }

        public void EndProgressTask(IProgressTask p_objTask)
        {
            p_objTask.EndTask();
        }
    }
}
