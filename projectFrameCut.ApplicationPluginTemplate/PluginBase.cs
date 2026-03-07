using projectFrameCut.ApplicationAPIBase.Effect;
using projectFrameCut.ApplicationAPIBase.Plugins;
using projectFrameCut.ApplicationAPIBase.Views.PropertyPanelBuilders;
namespace somebody
{
    // All the code in this file is included in all platforms in the plugin.
    public partial class AExamplePlugin_AppPart : AExamplePlugin, IApplicationPluginBase
    {
        public int AppLevelPluginAPIVersion => 3;

        public Dictionary<string, Func<IEffectBundle>> EffectBundleProvider => new();

        public View? SettingPageProvider(ref IApplicationPluginBase instance)
        {
            var ppb = new PropertyPanelBuilder();
            ppb.AddButton("Test button", async (s,e) =>
            {
                await (Application.Current?.Windows[0]?.Page?.DisplayAlertAsync("Hello", PlatformSpecificFunction(), "ok") ?? Task.CompletedTask);
            });
            return ppb.Build();
        }
    }
}
