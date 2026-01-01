/// <remarks>
/// In this example, the plugin class is named 'MyExamplePlugin' and is in the 'nobody' namespace.
/// So that the plugin's ID is 'nobody.MyExamplePlugin', which equals to it's full name.
/// Your plugin's Id SHOULD NOT starts with 'projectFrameCut', Case-insensitive.
/// </remarks>
using projectFrameCut.Plugin;
using projectFrameCut.Render.RenderAPIBase.ClipAndTrack;
using projectFrameCut.Render.RenderAPIBase.EffectAndMixture;
using projectFrameCut.Render.RenderAPIBase.Plugins;
using projectFrameCut.Render.RenderAPIBase.Sources;
using projectFrameCut.Shared;
using System.Text.Json;

namespace nobody
{
    /// <summary>
    /// This is an example plugin implementation.
    /// </summary>
    /// <remarks>
    /// A fast way to create your plugin is to copy this class and modify it.
    /// Please make sure the full name of your plugin class (including namespace) is unique among all plugins you want to use in projectFrameCut.
    /// Otherwise, there will be conflicts when loading plugins.
    /// </remarks>
    public partial class MyExamplePlugin : IPluginBase
    {
        /// <summary>
        /// Provides localization strings for the plugin, including name and description.
        /// </summary>
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
        public Dictionary<string, Func<IEffect>> EffectProvider => new Dictionary<string, Func<IEffect>>
        {
            {"ToBlankFrameEffect", () => new ToBlankFrameEffect() }
        };

        public Dictionary<string, Func<IEffect>> ContinuousEffectProvider => new Dictionary<string, Func<IEffect>>
        {

        };

        public Dictionary<string, Func<IEffect>> VariableArgumentEffectProvider => new Dictionary<string, Func<IEffect>>
        {

        };

        public Dictionary<string, Func<IMixture>> MixtureProvider => new Dictionary<string, Func<IMixture>>
        {

        };

        public Dictionary<string, Func<IComputer>> ComputerProvider => new Dictionary<string, Func<IComputer>>
        {

        };

        public Dictionary<string, Func<string, string, IClip>> ClipProvider => new Dictionary<string, Func<string, string, IClip>>
        {

        };

        public Dictionary<string, Func<string, IVideoSource>> VideoSourceProvider => new Dictionary<string, Func<string, IVideoSource>>
        {

        };
        /// <summary>
        /// Provides configuration options for the plugin.
        /// </summary>
        /// <remarks>
        /// The keys are option identifiers, and the values are their default value.
        /// </remarks>
        Dictionary<string, string> config = new Dictionary<string, string>
        {
            {"TestOption1","abc" },
            {"TestOption2","def" },
            {"TestOption3","ghi" },
            {"TestOption4","jkl" },
            {"NotLocalizedOption1","mno" },

        };

        public Dictionary<string, string> Configuration { get => config; set { config = value; } }
        /// <summary>
        /// Provides display strings for configuration options in different locales.
        /// </summary>
        /// <remarks>
        /// The outer dictionary's keys are locale identifiers, and the inner dictionary's keys are option identifiers with their display strings as values.
        /// If the localized string for the current locale is not found, the application will fall back to it's key.
        /// </remarks>
        public Dictionary<string, Dictionary<string, string>> ConfigurationDisplayString => new Dictionary<string, Dictionary<string, string>>
        {
            {
                "zh-CN",
                new Dictionary<string, string>
                {
                    {"TestOption1","测试选项1" },
                    {"TestOption2","测试选项2" },
                    {"TestOption3","测试选项3" },
                    {"TestOption4","测试选项4" },
                }
            },
            {
                "en-US",
                new Dictionary<string, string>
                {
                    {"TestOption1","Test option 1" },
                    {"TestOption2","Test option 2" },
                    {"TestOption3","Test option 3" },
                    {"TestOption4","Test option 4" },
                }
            },
            {
                "ja-JP",
                new Dictionary<string, string>
                {
                    {"TestOption1","テストオプション1" },
                    {"TestOption2","テストオプション2" },
                    {"TestOption3","テストオプション3" },
                    {"TestOption4","テストオプション4" },
                }
            },
            {
                "ko-KR",
                new Dictionary<string, string>
                {
                    {"TestOption1","테스트 옵션 1" },
                    {"TestOption2","테스트 옵션 2" },
                    {"TestOption3","테스트 옵션 3" },
                    {"TestOption4","테스트 옵션 4" },
                }
            },
            {
                "fr-FR", 
                new Dictionary<string, string>
                {
                    {"TestOption1","Option de test 1" },
                    {"TestOption2","Option de test 2" },
                    {"TestOption3","Option de test 3" },
                    {"TestOption4","Option de test 4" },
                }
            }
        };

        public Dictionary<string, Func<string, string, ISoundTrack>> SoundTrackProvider => new Dictionary<string, Func<string, string, ISoundTrack>> { };

        public Dictionary<string, Func<string, IAudioSource>> AudioSourceProvider => new Dictionary<string, Func<string, IAudioSource>> { };

        public Dictionary<string, Func<string, IVideoWriter>> VideoWriterProvider => new Dictionary<string, Func<string, IVideoWriter>> { };

        /// <summary>
        /// Creates an IClip instance from a JsonElement.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException">Indicates that this plugin doesn't provide any IClip.</exception>
        public IClip ClipCreator(JsonElement element)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates an ISoundTrack instance from a JsonElement.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException">Indicates that this plugin doesn't provide any ISoundTrack.</exception>
        public ISoundTrack SoundTrackCreator(JsonElement element)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called when the plugin is loaded. You can use this method to check whether all dependencies are met.
        /// </summary>
        /// <param name="FailedReason">The reason why the plugin failed to load, if any.</param>
        /// <returns>true for success, false otherwise.</returns>
        bool IPluginBase.OnLoaded(out string FailedReason)
        {
            if(!File.Exists(Path.Combine(PluginLoader.PluginRoot, "example_plugin_dependency.txt")))
            {
                FailedReason = "Missing dependency file: example_plugin_dependency.txt";
                return false;
            }
            FailedReason = string.Empty;
            return true;
        }
    }
}
