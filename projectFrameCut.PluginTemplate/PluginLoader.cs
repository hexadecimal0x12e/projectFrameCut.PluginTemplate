// IMPORTANT: DO NOT add edit or add any methods, members or namespaces in this file except the line indicated in the comments.

using projectFrameCut.Render.RenderAPIBase.Plugins;
using System;
using System.Collections.Generic;
using System.Text;

//DO NOT modify the namespace name, 
//be careful when you're using some tool to sync namespaces from file structure.
namespace projectFrameCut.Plugin
{
    /// <summary>
    /// Provides methods and properties to load the plugin and communicate with the host application.
    /// </summary>
    /// <remarks>
    /// Please only edit the CreateInstance method to return an instance of your plugin implementation.
    /// </remarks>
    public static partial class PluginLoader
    {
        /// <summary>
        /// Specifies the default locale identifier used for some culture-specific operations.
        /// </summary>
        public static string LocaleId = "en-US";
        /// <summary>
        /// Specifies the instance path of the projectFrameCut application.
        /// </summary>
        public static string PluginRoot = string.Empty;
        /// <summary>
        /// Provide a system to let plugin communicate with host application.
        /// </summary>
        public static Func<string, object, object>? MessageCaller = null;
        /// <summary>
        /// Invokes when a message is got from the host application.
        /// </summary>
        public static event EventHandler<Tuple<string,object>>? MessageHandler;

        /// <summary>
        /// Creates and returns an instance of the plugin.
        /// </summary>
        public static IPluginBase CreateInstance(string currentLocale, string pluginRoot)
        {
            LocaleId = currentLocale;
            PluginRoot = pluginRoot;
            //Please edit the line below ONLY.
            //note you may add some extra methods before returning the instance,
            //like boot the helper process, prepare dependencies files, etc.

            //please use the full qualified name for your plugin class.
            var instance = new nobody.MyExamplePlugin();

            //DO NOT modify the lines below.
            projectFrameCut.Shared.Logger.LogDiagnostic($"PluginLoader for plugin '{instance.Name}' invoked.");
            return instance;
        }

    }
}
