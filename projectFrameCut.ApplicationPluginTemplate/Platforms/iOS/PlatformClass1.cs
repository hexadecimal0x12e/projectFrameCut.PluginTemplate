using projectFrameCut.ApplicationAPIBase.Plugins;

namespace somebody
{
    // All the code in this file is only included on iOS.
    public partial class AExamplePlugin_AppPart : AExamplePlugin, IApplicationPluginBase
    {
        public string PlatformSpecificFunction()
        {
            return "Hello from iOS!";
        }
    }
}
