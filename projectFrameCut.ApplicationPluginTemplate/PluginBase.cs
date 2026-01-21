using projectFrameCut.ApplicationAPIBase.Effect;
using projectFrameCut.ApplicationAPIBase.Plugins;
using projectFrameCut.ApplicationAPIBase.PropertyPanelBuilders;
using projectFrameCut.Plugin;
using projectFrameCut.Render.RenderAPIBase.ClipAndTrack;
using projectFrameCut.Render.RenderAPIBase.EffectAndMixture;
using projectFrameCut.Render.RenderAPIBase.Plugins;
using projectFrameCut.Render.RenderAPIBase.Sources;
using projectFrameCut.Shared;
using System.Text.Json;
#pragma warning disable CA1416 // We can ensure it runs on supported platforms.
#pragma warning disable IDE0130
namespace nobody
#pragma warning restore IDE0130
{
    /// <summary>
    /// This is an example application-level plugin implementation.
    /// </summary>
    public class MyExamplePluginApplicationLevelPart : MyExamplePlugin, IApplicationPluginBase
    {
        public Dictionary<string, Func<IEffectBundle>> EffectBundleProvider => new Dictionary<string, Func<IEffectBundle>>
        {

        };

        public View? SettingPageProvider(ref IApplicationPluginBase instance)
        {
            PropertyPanelBuilder ppb = new();
            ppb.AddText("Hello world!");
            ppb.AddButton("Click me!", async (s,e) =>
            {
                if(Application.Current?.Windows?.First()?.Page is Page page)
                {
                    await page.DisplayAlertAsync("Button clicked", "You clicked the button!", "OK");

                }
            });

            return ppb.BuildWithScrollView();
        }
    }
}

