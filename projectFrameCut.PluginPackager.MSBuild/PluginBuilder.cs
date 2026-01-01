using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace projectFrameCut.PluginPackager.MSBuild
{
    public class PluginBuilder : Microsoft.Build.Utilities.Task
    {
        public const int CurrentPluginAPIVersion = 1;

        public override bool Execute()
        {
#if NET5_0_OR_GREATER
            try
            {
                // 输出所有公开参数（使用反射）
                var sb = new StringBuilder();
                sb.AppendLine("PluginBuilder arguments:");
                var props = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var p in props)
                {
                    if (p.GetMethod == null)
                        continue;
                    object value;
                    try
                    {
                        value = p.GetValue(this);
                    }
                    catch (Exception ex)
                    {
                        value = $"<Failed to get value: {ex.Message}>";
                    }

                    string display;
                    if (value == null)
                    {
                        display = "<null>";
                    }
                    else if (value is string str)
                    {
                        display = str;
                    }
                    else if (value is IEnumerable enumerable && !(value is string))
                    {
                        var items = new List<string>();
                        foreach (var item in enumerable)
                        {
                            items.Add(item?.ToString() ?? "<null>");
                        }
                        display = string.Join(", ", items);
                    }
                    else
                    {
                        display = value.ToString();
                    }

                    sb.AppendLine($"{p.Name} = {display}");
                }

                Log.LogMessage(MessageImportance.High, sb.ToString());
                Log.LogMessage($"TFM:{TargetFrameworkID}");

                // If requested, generate a partial source file that provides
                // the basic IPluginBase properties (so the project can compile them).
                if (GenerateSource)
                {
                    try
                    {
                        var path = PartialClassGenerator.GeneratePartialClassFile(PluginID, PluginVersion, PluginPublishUrl, NeutralLanguageDisplayName, Authors, NeutralLanguageDescription, PluginProjectUrl, ProjectRootPath, GenerateSourcePath);
                        Log.LogMessage(MessageImportance.High, $"Generated plugin source: {path}");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Log.LogErrorFromException(ex, true);
                        return false;
                    }
                }

                if (string.IsNullOrWhiteSpace(SignFilePath) || !File.Exists(SignFilePath))
                {
                    Log.LogError($"SignFilePath invalid or not found: {SignFilePath}");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(PluginOutputPath))
                {
                    Log.LogError("PluginOutputPath is required.");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(OutputDirectory) || !Directory.Exists(OutputDirectory))
                {
                    Log.LogError($"OutputDirectory invalid or not found: {OutputDirectory}");
                    return false;
                }

                // Try to find the plugin assembly (prefer the project's assembly name)
                var assemblyName = Path.GetFileNameWithoutExtension(PluginOutputPath);
                var targetDll = Path.Combine(OutputDirectory, assemblyName + ".dll");

                if (!Version.TryParse(PluginVersion, out var ver))
                {
                    Log.LogError($"PackageVersion '{PluginVersion}' is not a valid version string.");
                    return false;
                }

                PluginMetadata mtd = new PluginMetadata
                {
                    Author = Authors,
                    AuthorUrl = PluginProjectUrl,
                    Description = NeutralLanguageDescription,
                    PluginID = PluginID,
                    Version = ver,
                    Name = NeutralLanguageDisplayName,
                    PluginAPIVersion = 1,
                    PublishingUrl = PluginPublishUrl

                };


                Build(PluginID, ProjectRootPath, targetDll, AssetPath ?? string.Empty, OutputDirectory, SignFilePath, mtd);

                var expectedName = mtd.PluginID + "_" + mtd.Version + ".pjfcPlugin";
                var pkgPath = Path.Combine(OutputDirectory, expectedName);
                if (File.Exists(pkgPath))
                {
                    PluginPackagePath = pkgPath;
                    Log.LogMessage(MessageImportance.High, $"Plugin packaged to {pkgPath}");
                    return true;
                }
                else
                {
                    Log.LogError($"Packaging completed but expected package not found: {pkgPath}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex, true);
                return false;
            }
#else
            if (GenerateSource)
            {
                try
                {
                    var path = PartialClassGenerator.GeneratePartialClassFile(PluginID, PluginVersion, PluginPublishUrl, NeutralLanguageDisplayName, Authors, NeutralLanguageDescription, PluginProjectUrl, ProjectRootPath, GenerateSourcePath);
                    Log.LogMessage(MessageImportance.High, $"Generated plugin source: {path}");
                    return true;
                }
                catch (Exception ex)
                {
                    Log.LogErrorFromException(ex, true);
                    return false;
                }
            }
            else
            {
                Log.LogError("To bundle a plugin, please use 'dotnet publish' command, along with appending this to your dotnet command:'-p:BundlePlugin=true', instead of using Visual Studio's Publish tool.");
                return false;
            }
            
#endif
        }
#if NET5_0_OR_GREATER
        public void Build(string pluginID, string projectRoot, string DllPath, string assetPath, string tempPath, string kpPath, PluginMetadata mtd)
        {

            var mainDllPath = Path.Combine(projectRoot, DllPath);
            Console.WriteLine($"pluginID:{pluginID}, mainDLLPath: {mainDllPath}, assetPath:{assetPath}, tempPath:{tempPath}, kpPath:{kpPath}");
            if (mainDllPath is null || !File.Exists(mainDllPath)) throw new FileNotFoundException($"Plugin source not found.");
            Console.WriteLine($"PluginID: {pluginID}, workingDir: {tempPath}");
            string pubKey = "", priKey = "";
            GetKeypairFromFile(kpPath, out pubKey, out priKey);

            Console.WriteLine("Encrypting assembly...");
            Directory.CreateDirectory(Path.Combine(tempPath, "plugin"));
            var sig = projectFrameCut.Shared.FileSignerService.SignFile(priKey, mainDllPath);
            var sigPath = Path.Combine(tempPath, "plugin", pluginID + ".dll.sig");
            File.WriteAllText(sigPath, sig);
            var encFilePath = Path.Combine(tempPath, "plugin", pluginID + ".dll.enc");
            var sigKey = ComputeStringHash(pubKey, SHA512.Create());
            projectFrameCut.Shared.FileCryptoService.EncryptToFileWithPassword(sigKey, mainDllPath, encFilePath);
            Console.WriteLine("Making metadata...");
            mtd.PluginHash = ComputeFileHashAsync(mainDllPath);
            mtd.PluginKey = sigKey;
            var mtdJson = JsonConvert.SerializeObject(mtd, Formatting.Indented);
            Console.WriteLine($"Metadata:\r\n{mtd}");

            Console.WriteLine("Packaging plugin...");
            File.WriteAllText(Path.Combine(tempPath, "plugin", "metadata.json"), mtdJson);
            File.WriteAllText(Path.Combine(tempPath, "plugin", "publickey.pem"), pubKey);
            if (!string.IsNullOrWhiteSpace(assetPath) && Directory.Exists(assetPath))
            {
                //Copy assets
                var destAssetPath = Path.Combine(tempPath, "plugin");
                Directory.CreateDirectory(destAssetPath);
                foreach (var file in Directory.GetFiles(assetPath, "*", SearchOption.AllDirectories))
                {
                    var relativePath = Path.GetRelativePath(assetPath, file);
                    var destFilePath = Path.Combine(destAssetPath, relativePath);
                    Directory.CreateDirectory(Path.GetDirectoryName(destFilePath));
                    Console.WriteLine($"Copying asset {file} to {destFilePath}...");
                    File.Copy(file, destFilePath, true);
                }
            }
            else
            {
                Log.LogMessage("No asset provided, skip.");
            }
            Dictionary<string, string> hashTable = new Dictionary<string, string>();
            foreach (var file in Directory.GetFiles(Path.Combine(tempPath, "plugin"), "*", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(Path.Combine(tempPath, "plugin"), file);
                var fileHash = ComputeFileHashAsync(file);
                hashTable[relativePath.Replace('\\', '/')] = fileHash;
            }
            var hashJson = JsonConvert.SerializeObject(hashTable, Formatting.Indented);
            Log.LogMessage($"hashtable:{hashJson}");
            File.WriteAllText(Path.Combine(tempPath, "plugin", "hashtable.json"), hashJson);

            var zipPath = Path.Combine(tempPath, $"{pluginID}_{mtd.Version}.pjfcPlugin");
            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }

            ZipFile.CreateFromDirectory(Path.Combine(tempPath, "plugin"), zipPath, CompressionLevel.Optimal, false);

            Console.WriteLine($"Plugin packaged to {zipPath}");

        }
#endif

        public static void GetKeypairFromFile(string path, out string pubKey, out string priKey)
        {
            try
            {
                KeyValuePair<string, string> kp = JsonConvert.DeserializeObject<KeyValuePair<string, string>>(File.ReadAllText(path));
                pubKey = kp.Key;
                priKey = kp.Value;
                Console.WriteLine("Success get keypair.");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to get keypair:{ex.Message}. Try again.");
                //pubKey = null;
                //priKey = null;
                throw;


            }
        }

        [DebuggerNonUserCode()]
        public static string ComputeFileHashAsync(string fileName, HashAlgorithm algorithm = null)
        {
            algorithm = SHA256.Create();
            if (System.IO.File.Exists(fileName))
            {
                System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                byte[] buffer = algorithm.ComputeHash(fs);
                algorithm.Clear();
                return buffer.Select(c => c.ToString("x2")).Aggregate((a, b) => a + b);
            }
            throw new FileNotFoundException("File not found", fileName);
        }

        [DebuggerNonUserCode()]
        public static string ComputeStringHash(string input, HashAlgorithm algorithm = null)
            => (algorithm ?? SHA512.Create())
                .ComputeHash(Encoding.UTF8.GetBytes(input))
                .Select(c => c.ToString("x2"))
                .Aggregate((a, b) => a + b);


        private static string AppendDirectorySeparatorChar(string path)
        {
            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
                return path + Path.DirectorySeparatorChar;
            return path;
        }

        [Required]
        public string PluginOutputPath { get; set; }
        [Required]
        public string ProjectRootPath { get; set; }

        public string TargetFrameworkID { get; set; }


        [Required]
        public string OutputDirectory { get; set; }
        [Required]
        public string SignFilePath { get; set; }
        [Required]
        public string PluginID { get; set; }
        [Required]
        public string NeutralLanguageDisplayName { get; set; }
        [Required]
        public string PluginVersion { get; set; }


        public string AssetPath { get; set; }
        public string Authors { get; set; }
        public string PluginProjectUrl { get; set; }
        public string PluginPublishUrl { get; set; }
        public string NeutralLanguageDescription { get; set; }


        public bool GenerateSource { get; set; }
        public string GenerateSourcePath { get; set; }


        [Output]
        public string PluginPackagePath { get; set; }

       
    }
}
