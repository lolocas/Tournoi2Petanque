using Framework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Events
{
    public class EventModelArgs<TModel> : EventArgs where TModel : IModel
    {
        public TModel ModelValue { get; set; }

        public EventModelArgs(TModel p_clsModel)
        {
            ModelValue = p_clsModel;
        }
    }
}
