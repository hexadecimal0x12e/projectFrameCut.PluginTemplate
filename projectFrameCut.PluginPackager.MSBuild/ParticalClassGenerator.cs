using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace projectFrameCut.PluginPackager.MSBuild
{
    public static class PartialClassGenerator
    {
        public static string GeneratePartialClassFile(string PluginID, string PluginVersion, string PluginPublishUrl, string NeutralLanguageDisplayName, string Authors, string NeutralLanguageDescription, string PluginProjectUrl, string ProjectRootPath, string generatedOutputDirectory = null)
        {
            if (string.IsNullOrWhiteSpace(PluginID)) throw new InvalidOperationException("PluginID is required to generate plugin source.");
            var idx = PluginID.LastIndexOf('.');
            string ns = idx > 0 ? PluginID.Substring(0, idx) : "nobody";
            string className = idx > 0 ? PluginID.Substring(idx + 1) : PluginID;

            Version v = null;
            if (!Version.TryParse(PluginVersion, out v))
            {
                v = new Version(1, 0, 0, 0);
            }
            int major = v.Major;
            int minor = v.Minor;
            int build = v.Build < 0 ? 0 : v.Build;
            int rev = v.Revision < 0 ? 0 : v.Revision;

            string Escape(string s) => s == null ? string.Empty : s.Replace("\\", "\\\\").Replace("\"", "\\\"");

            var publishExpr = string.IsNullOrWhiteSpace(PluginPublishUrl) ? "null" : $"\"{Escape(PluginPublishUrl)}\"";
            var tw = new StringWriter();
            System.CodeDom.Compiler.IndentedTextWriter writer = new System.CodeDom.Compiler.IndentedTextWriter(tw);
            writer.WriteLine("#nullable enable");
            writer.WriteLine("using System;");
            writer.WriteLine($"namespace {ns}");
            writer.WriteLine("{");
            writer.Indent++;
            writer.WriteLine($"public partial class {className}");
            writer.WriteLine("{");
            writer.Indent++;
            writer.WriteLine($"public string PluginID => \"{Escape(PluginID)}\";");
            writer.WriteLine($"public int PluginAPIVersion => {PluginBuilder.CurrentPluginAPIVersion};");
            writer.WriteLine($"public string Name => \"{Escape(NeutralLanguageDisplayName)}\";");
            writer.WriteLine($"public string Author => \"{Escape(Authors)}\";");
            writer.WriteLine($"public string Description => \"{Escape(NeutralLanguageDescription)}\";");
            writer.WriteLine($"public Version Version => new Version({major}, {minor}, {build}, {rev});");
            writer.WriteLine($"public string AuthorUrl => \"{Escape(PluginProjectUrl)}\";");
            writer.WriteLine($"public string? PublishingUrl => {publishExpr};");
            writer.Indent--;
            writer.WriteLine("}");
            writer.Indent--;
            writer.WriteLine("}");
            string baseDir;

            if (!string.IsNullOrWhiteSpace(generatedOutputDirectory))
            {
                baseDir = generatedOutputDirectory;
                if (!Path.IsPathRooted(baseDir))
                {
                    baseDir = Path.Combine(ProjectRootPath ?? Directory.GetCurrentDirectory(), baseDir);
                }
            }
            else if (!string.IsNullOrWhiteSpace(ProjectRootPath))
            {
                baseDir = Path.Combine(ProjectRootPath, "obj", "GeneratedSource");
            }
            else
            {
                baseDir = Path.Combine(Directory.GetCurrentDirectory(), "obj", "GeneratedSource");
            }

            Directory.CreateDirectory(baseDir);
            var outPath = Path.Combine(baseDir, "PluginInfo.sg.cs");
            File.WriteAllText(outPath, tw.ToString(), Encoding.UTF8);
            return outPath;
        }
    }
}
