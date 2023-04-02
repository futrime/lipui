# LipUI - Tooth Pack Manager with UI
[![GitHub release (latest by date)](https://img.shields.io/github/v/release/LL-Package-Hub/LipUI)](
Work In Progress
### How to build
- Visual Studio 2022
- .Net Framework 4.6.2 SDK
- .net 7.0 SDK

### FAQ
#### how to use custom registry for tooth pack source？(the default registry source is LL-Package-Hub)
- launch LipUI.exe with extra arguments flag `--registry`or`-r`，for example `LipUI.exe -r http://xxx.xxx.xxx/registry.json`，(registry in setting havn't finished)。
#### Does it cross-platform?
- currently only support Windows,no cross-platform plan yet.any pull request is welcome.
#### What is the minimum requirement for the operating system?
- Windows 7 SP1 and above (and corresponding server versions) and .net framework 4.6.2 - 4.8 runtime are required
- or include the .net 7.0 desktop runtime
   - build the sln will generate both .net 7.0 and .net framework 4.6.2 versions.
