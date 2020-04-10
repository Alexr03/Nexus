﻿﻿using System.Collections.Generic;

namespace Nexus.SDK.Plugins
{
    public class PluginManager<T>
    {
        public IList<T> Plugins { get; }
        
        public PluginManager(List<object> modules)
        {
            this.Plugins = ParseModules(modules);
        }

        private IList<T> ParseModules(List<object> modules)
        {
            IList<T> realModules = new List<T>();

            foreach (object module in modules)
            {
                if (module is T o)
                {
                    realModules.Add(o);
                }
            }

            return realModules;
        }
    }
}