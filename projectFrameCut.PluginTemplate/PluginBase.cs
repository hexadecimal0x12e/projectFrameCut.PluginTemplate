//All these stuff will appear in both normal and App-level plugins.
using projectFrameCut.Render.RenderAPIBase.ClipAndTrack;
using projectFrameCut.Render.RenderAPIBase.EffectAndMixture;
using projectFrameCut.Render.RenderAPIBase.Plugins;
using projectFrameCut.Render.RenderAPIBase.Sources;
using System.Text.Json;

namespace somebody
{
    public partial class AExamplePlugin : IPluginBase
    {
        public Dictionary<string, Dictionary<string, string>> LocalizationProvider => new Dictionary<string, Dictionary<string, string>>
        {
            {
                "zh-CN",
                new Dictionary<string, string>
                {
                    {"_PluginBase_Name_", "一个示例插件" },
                    {"_PluginBase_Description_","一些描述" },
                }
            },
            {
                "en-US",
                new Dictionary<string, string>
                {
                    {"_PluginBase_Name_", "a example plugin" },
                    {"_PluginBase_Description_","some description" },
                }
            },
            {
                "ja-JP",
                new Dictionary<string, string>
                {
                    {"_PluginBase_Name_", "サンプルプラグイン" },
                    {"_PluginBase_Description_","いくつかの説明" },
                }
            },
            {
                "ko-KR",
                new Dictionary<string, string>
                {
                    {"_PluginBase_Name_", "예제 플러그인" },
                    {"_PluginBase_Description_","일부 설명" },
                }
            },
            {
                "fr-FR", new Dictionary<string, string>
                {
                    {"_PluginBase_Name_", "Un plugin exemple" },
                    {"_PluginBase_Description_","Une description" },
                }
            }
        };
        Dictionary<string, Func<string, string, IClip>> IPluginBase.ClipProvider => new();

        Dictionary<string, Func<string, string, ISoundTrack>> IPluginBase.SoundTrackProvider => new();

        Dictionary<string, Func<Guid, Guid, ITransform>> IPluginBase.TransformProvider => new();

        Dictionary<string, Func<IEffect>> IPluginBase.EffectProvider => new();

        Dictionary<string, IEffectFactory> IPluginBase.EffectFactoryProvider => new();

        Dictionary<string, Func<IEffect>> IPluginBase.ContinuousEffectProvider => new();

        Dictionary<string, IEffectFactory> IPluginBase.ContinuousEffectFactoryProvider => new();

        Dictionary<string, Func<IEffect>> IPluginBase.BindableArgumentEffectProvider => new();

        Dictionary<string, IEffectFactory> IPluginBase.BindableArgumentEffectFactoryProvider => new();

        Dictionary<string, Func<IComputer>> IPluginBase.ComputerProvider => new();

        Dictionary<string, Func<string, IVideoSource>> IPluginBase.VideoSourceProvider => new();

        Dictionary<string, Func<string, IAudioSource>> IPluginBase.AudioSourceProvider => new();

        Dictionary<string, Func<string, IVideoWriter>> IPluginBase.VideoWriterProvider => new();

        Dictionary<string, string> IPluginBase.Configuration { get; set; } = new();

        Dictionary<string, Dictionary<string, string>> IPluginBase.ConfigurationDisplayString => new();

        IClip IPluginBase.ClipCreator(JsonElement element)
        {
            throw new NotImplementedException();
        }

        ISoundTrack IPluginBase.SoundTrackCreator(JsonElement element)
        {
            throw new NotImplementedException();
        }
    }
}
