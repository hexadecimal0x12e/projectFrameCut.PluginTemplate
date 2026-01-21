# 基础条件
请确保你的设备上有 .NET 10 的SDK，如果你在使用Visual Studio，请使用Visual Studio 2022或者更新。如果你要开发应用程序插件，你还需要安装 .NET MAUI的工作负载。
虽然你的插件可以不面向 .NET 10，但是共享库是面向 .NET 10 的，使用 .NET 10 可以避免一些奇奇怪怪的问题。
你可以使用任何IDE、系统开发插件，你还可以用F#、VB .NET~~或者MSIL~~来写你的插件。

# 从模板开始
0. 克隆这个项目到你的电脑上
1. 获取并引用projectFrameCut共享库

projectFrameCut共享库包括了大部分projectFrameCut渲染和处理的基础API，并且定义了许多基础类的接口。
你可以直接从projectFrameCut的主程序目录里找到`projectFrameCut.Shared.dll`、`projectFrameCut.Render.RenderAPIBase.dll`和`projectFrameCut.ApplicationAPIBase.dll`，然后复制到项目根目录里的`PluginBaseAssembly`文件夹里。

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
你需要把`PluginSignFilePath`里的路径替换成签名文件实际的路径。

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

# 从头开始
首先，新建一个类库或者 .NET MAUI 类库项目。
1. 获取并引用projectFrameCut共享库

projectFrameCut共享库包括了大部分projectFrameCut渲染和处理的基础API，并且定义了许多基础类的接口。
你可以直接从projectFrameCut的主程序目录里找到`projectFrameCut.Shared.dll`、`projectFrameCut.Render.RenderAPIBase.dll`和`projectFrameCut.ApplicationAPIBase.dll`，然后复制他们到一个文件夹里。

然后，修改你的.csproj文件，添加下面的引用项：
```xml
<Reference Include="\Path\To\Shared\Libraries\projectFrameCut.Render.RenderAPIBase.dll" />
<Reference Include="\Path\To\Shared\Libraries\projectFrameCut.Shared.dll" />
<PackageReference Include="projectFrameCut.PluginPackager.MSBuild" Version="1.0.0" />
```
**请把`\Path\To\Shared\Libraries`替换成你把共享库复制到的目标文件夹**

如果要开发应用程序插件，请添加这个引用项：
```xml
<Reference Include="\Path\To\Shared\Libraries\projectFrameCut.ApplicationAPIBase.dll" />
```

2. 实现插件类
a. 在默认的Class1.cs里，修改默认提供的命名空间和类名到你想要的值。**请注意，这些值会在最后成为你的插件ID的构成部分**
b. 修改声明，让默认的Class1 **变成`partial`** 并且实现`IPluginBase`来实现标准插件，或者`IApplicationPluginBase`来实现应用程序插件，类似这样子：
```csharp
namespace nobody
{
    public partial class MyExamplePlugin : IPluginBase
    {
        //...
    }
}
```
c. 使用IDE的工具自动补全所有的接口
**替换掉所有Dictionary成员的`NotImplementedException`实现到新的实例`new()`**
**删除自动生成的PluginID、PluginAPIVersion、Name、Author、Description、Version、AuthorUrl和PublishingUrl实现，他们会稍后自动生成**

这一步你可以参考模板

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
<PluginSignFilePath>Path\To\Your\key.json</PluginSignFilePath>
```
你需要把`PluginSignFilePath`里的路径替换成签名文件实际的路径。

5. 开发
a. 创建插件加载器
a1. 下载[PluginLoader.cs](https://github.com/hexadecimal0x12e/projectFrameCut.PluginTemplate/blob/main/projectFrameCut.PluginTemplate/PluginLoader.cs)
a2. 在你的项目里新建一个`PluginLoader.cs`，替换内容到你下载的文件。
a3. 打开`PluginLoader.cs`，把`return new MyExamplePlugin();`的`MyExamplePlugin`替换成你的插件类。
如果你开发了应用程序插件，**你还需要把`PluginLoader.cs`里的`PluginLoader`替换成`AppLevelPluginLoader`**.


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