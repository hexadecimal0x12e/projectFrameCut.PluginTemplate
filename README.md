# projectFrameCut 插件模板
这个仓库提供了能够让你为[projectFrameCut](https://github.com/hexadecimal0x12e/projectFrameCut)及其衍生版本（如果他们的开发者愿意的话）开发一个插件，自定义projectFrameCut的功能的能力。
它还包含了一个插件的模板，你可以从模板开始，也可以手动一步步来，从头开始。

# 基础条件
请确保你的设备上有 .NET 10 的SDK，如果你在使用Visual Studio，请使用Visual Studio 2022或者更新。
虽然你的插件可以不面向 .NET 10，但是共享库是面向 .NET 10 的，使用 .NET 10 可以避免一些奇奇怪怪的问题。
你可以使用任何IDE、系统开发插件，你还可以用F#、VB .NET~~或者MSIL~~来写你的插件。

# 从模板开始
0. 克隆这个项目到你的电脑上
1. 获取并引用projectFrameCut共享库

projectFrameCut共享库包括了大部分projectFrameCut渲染和处理的基础API，并且定义了许多基础类的接口。
你可以直接从projectFrameCut的主程序目录里找到`projectFrameCut.Shared.dll`和`projectFrameCut.Render.RenderAPIBase.dll`，然后复制到项目根目录里的`PluginBaseAssembly`文件夹里。

2. 配置项目
修改项目的.csproj的第一个`PropertyGroup`：
```xml
<PackageId>nobody.MyExamplePlugin</PackageId>
<Version>42.42.42.42</Version>
<PackageProjectUrl>https://example.com/1</PackageProjectUrl>
<Title>Example plugin</Title>
<Authors>none</Authors>
<Description>desc</Description>
```
其中，
* `PackageId`： 插件的唯一标识符，**请确保他和你的插件类的全名一致，并且不得以`projectFrameCut`开头（不区分大小写）**
* `Version`： 插件的版本号
* `PackageProjectUrl`： 插件的项目主页URL，可以留空
* `Title`： 插件的名称
* `Authors`： 插件的作者
* `Description`： 插件的描述


3. 创建签名
有很多种方法创建签名，最方便的是直接使用`PluginKeyGenerator.cs`:
a. 在项目的根目录里，打开一个终端/命令提示符
b. 运行命令`dotnet run PluginKeyGenerator.cs`
c. 项目的根目录里会生成一个key.json文件。**把它移动到一个安全的地方**
> [!WARNING]
> **请保管好生成的 key.json 文件！**
> **如果丢失，你的的用户将不能在未来更新他们的插件，只能卸载重装。**
> **如果签名意外的泄露，你的插件可能会被滥用，因为projectFrameCut依赖签名来校验发布者！**

e. 修改项目.csproj文件的第一个`PropertyGroup`：
```xml
<PluginSignFilePath>Path\To\You\key.json</PluginSignFilePath>
```
你需要把`Path\To\You\key.json`替换成签名文件实际的路径。

4. 开发
修改`PluginBase.cs`：
* 把命名空间`nobody`，和类名`MyExamplePlugin`替换掉，**请注意，这些值会在最后成为你的插件ID的构成部分**
* 打开`PluginLoader.cs`，把`return new MyExamplePlugin();`的`MyExamplePlugin`替换成你的插件类。
* 然后，实现你想要的东西
你可以参阅共享库的API文档来了解每一个类、结构或者方法是干什么的。

5. 分发
a. 打开终端，转到你的项目的根目录（.csproj文件所在的目录）
b. 运行这个命令：
```bash
dotnet publish -p:BundlePlugin=true --restore
```
你也可以使用大部分的`dotnet publish`参数来控制编译过程，包括但不限于`-o`选项来选择输出目录
c. 稍等一会，在输出目录里你会发现一个`.pjfcPlugin`文件，这个就是你要分发的文件了。

# 如何开发
首先，新建一个类库项目。
1. 获取并引用projectFrameCut共享库

projectFrameCut共享库包括了大部分projectFrameCut渲染和处理的基础API，并且定义了许多基础类的接口。
你可以直接从projectFrameCut的主程序目录里找到`projectFrameCut.Shared.dll`和`projectFrameCut.Render.RenderAPIBase.dll`，然后复制他们到一个文件夹里。

然后，修改你的.csproj文件，添加下面的引用项：
```xml
<Reference Include="\Path\To\Shared\Libraries\projectFrameCut.Render.RenderAPIBase.dll" />
<Reference Include="\Path\To\Shared\Libraries\projectFrameCut.Shared.dll" />
<PackageReference Include="projectFrameCut.PluginPackager.MSBuild" Version="1.0.0" />
```
**请把`\Path\To\Shared\Libraries`替换成你把共享库复制到的目标文件夹**

2. 实现插件类
a. 在默认的Class1.cs里，修改默认提供的命名空间和类名到你想要的值。**请注意，这些值会在最后成为你的插件ID的构成部分**
b. 修改声明，让默认的Class1 **变成`partial`** 并且实现`IPluginBase`，类似这样子：
```csharp
namespace nobody
{
    public partial class MyExamplePlugin : IPluginBase
    {
        //...
    }
}
```
你会注意到一堆错误信息：“MyExamplePlugin”不实现接口成员“IPluginBase.***”，先暂时不管他
3. 实现接口
粘贴下列代码到你的插件类里面：
```csharp
public Dictionary<string, Dictionary<string, string>> LocalizationProvider => new Dictionary<string, Dictionary<string, string>> { };

public Dictionary<string, Func<IEffect>> EffectProvider => new Dictionary<string, Func<IEffect>> { };

public Dictionary<string, Func<IEffect>> ContinuousEffectProvider => new Dictionary<string, Func<IEffect>> { };

public Dictionary<string, Func<IEffect>> VariableArgumentEffectProvider => new Dictionary<string, Func<IEffect>> { };

public Dictionary<string, Func<IMixture>> MixtureProvider => new Dictionary<string, Func<IMixture>> { };

public Dictionary<string, Func<IComputer>> ComputerProvider => new Dictionary<string, Func<IComputer>> { };

public Dictionary<string, Func<string, string, IClip>> ClipProvider => new Dictionary<string, Func<string, string, IClip>> { };

public Dictionary<string, Func<string, IVideoSource>> VideoSourceProvider => new Dictionary<string, Func<string, IVideoSource>> { };

public Dictionary<string, string> Configuration { get => config; set { config = value; } }
Dictionary<string, string> config = new Dictionary<string, string> { };

public Dictionary<string, Dictionary<string, string>> ConfigurationDisplayString => new Dictionary<string, Dictionary<string, string>> { };

public Dictionary<string, Func<string, string, ISoundTrack>> SoundTrackProvider => new Dictionary<string, Func<string, string, ISoundTrack>> { };

public Dictionary<string, Func<string, IAudioSource>> AudioSourceProvider => new Dictionary<string, Func<string, IAudioSource>> { };

public Dictionary<string, Func<string, IVideoWriter>> VideoWriterProvider => new Dictionary<string, Func<string, IVideoWriter>> { };

public IClip ClipCreator(JsonElement element)
{
    throw new NotImplementedException();
}

public ISoundTrack SoundTrackCreator(JsonElement element)
{
    throw new NotImplementedException();
}

bool IPluginBase.OnLoaded(out string FailedReason)
{
    FailedReason = string.Empty;
    return true;
}

```
你还会注意到一些错误指出类似于“MyExamplePlugin”不实现接口成员“IPluginBase.PluginID”的错误，**先不要实现这里提出的接口，他们会自动生成**

3. 配置项目
然后，配置插件的属性。在项目的.csproj的第一个`PropertyGroup`添加：
```xml
<PackageId>nobody.MyExamplePlugin</PackageId>
<Version>42.42.42.42</Version>
<PackageProjectUrl>https://example.com/1</PackageProjectUrl>
<Title>Example plugin</Title>
<Authors>none</Authors>
<Description>desc</Description>
```
其中，
* `PackageId`： 插件的唯一标识符，**请确保他和你的插件类的全名一致，并且不得以`projectFrameCut`开头（不区分大小写）**
* `Version`： 插件的版本号
* `PackageProjectUrl`： 插件的项目主页URL，可以留空
* `Title`： 插件的名称
* `Authors`： 插件的作者
* `Description`： 插件的描述

你可以试着生成项目，所有的`“MyExamplePlugin”不实现接口成员“...”`错误都应该消失了。

4. 创建签名
有很多种方法创建签名，最方便的是直接使用`PluginKeyGenerator.cs`:
a. 下载文件[PluginKeyGenerator.cs](https://github.com/hexadecimal0x12e/projectFrameCut.PluginTemplate/blob/main/PluginKeyGenerator.cs)到本地
b. 打开终端/命令提示符，使用CD命令转到一个安全的目录（比如你的插件项目的根目录）
c. 运行命令`dotnet run <path>`，把`<path>`替换成你下载的`PluginKeyGenerator.cs`的完整路径（比如"c:\user\nobody\download\PluginKeyGenerator.cs"）
d. 你在b里转到的目录里会生成一个key.json文件。
> [!WARNING]
> **请保管好生成的 key.json 文件！**
> **如果丢失，你的的用户将不能在未来更新他们的插件，只能卸载重装。**
> **如果签名意外的泄露，你的插件可能会被滥用，因为projectFrameCut依赖签名来校验发布者！**

e. 修改你的项目.csproj文件，在第一个`PropertyGroup`里添加这个：
```xml
<PluginSignFilePath>Path\To\You\key.json</PluginSignFilePath>
```
你需要把`Path\To\You\key.json`替换成签名文件实际的路径。

5. 开发
a. 创建插件加载器
a1. 下载[PluginLoader.cs](https://github.com/hexadecimal0x12e/projectFrameCut.PluginTemplate/blob/main/projectFrameCut.PluginTemplate/PluginLoader.cs)
a2. 在你的项目里新建一个`PluginLoader.cs`，替换内容到你下载的文件，不要修改任何部分
a3. 打开`PluginLoader.cs`，把`return new MyExamplePlugin();`的`MyExamplePlugin`替换成你的插件类。

b. 开发你喜欢的东西
你可以参阅共享库的API文档来了解每一个类、结构或者方法是干什么的。

6. 分发
a. 打开终端，转到你的项目的根目录（.csproj文件所在的目录）
b. 运行这个命令：
```bash
dotnet publish -p:BundlePlugin=true --restore
```
你也可以使用大部分的`dotnet publish`参数来控制编译过程，包括但不限于`-o`选项来选择输出目录
c. 稍等一会，在输出目录里你会发现一个`.pjfcPlugin`文件，这个就是你要分发的文件了。