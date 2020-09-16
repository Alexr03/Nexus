using System.Collections.Generic;

namespace Nexus.SDK.Plugins
{
    public class PluginManager<T>
    {
        public IList<T> Plugins { get; }

        public PluginManager(IEnumerable<object> modules)
        {
            Plugins = ParseModules(modules);
        }

        private static IList<T> ParseModules(IEnumerable<object> modules)
        {
            IList<T> realModules = new List<T>();

            foreach (var module in modules)
                if (module is T o)
                    realModules.Add(o);

            return realModules;
        }
    }
}