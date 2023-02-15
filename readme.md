# LipUI - 图形化齿包管理器
WIP
快速开发阶段


- [x] 查看本地齿包及其信息
- [x] 安装齿包
  - [x] 基本功能和进度UI
  - [x] 下载进度条反馈
- [x] 卸载齿包
- [x] registry包市场
  - [ ] 使用自定义源(启动参数附加`--registry`或`-r`)
  - [x] 搜索
  - [x] 获取和预览齿包
  - [x] 跳转到安装页面 
- [x] 设置/关于
  - [x] 深色/浅色主题切换
    - [x] 保存设置
  - [x] 自定义lip位置
- [x] 环境准备
  - [x] 未获取到lip.exe则提示并自动下载
  - [x] 强制用户选择工作目录方可继续操作

### 编译方法
- Visual Studio 2022
- .Net Framework 4.6.2 SDK
- .net 7.0 SDK

### FAQ
#### 如何使用自定义源？(默认源为LL-Package-Hub的registry)
- 启动LipUI.exe附带参数`--registry`或`-r`，例如`LipUI.exe -r http://xxx.xxx.xxx/registry.json`，(设置修改暂不开放)。
#### 是否跨平台？
- 目前只支持Windows，暂无跨平台计划。如有比较好的设计方案欢迎pr或者重写。
#### 运行环境？
- Windows 7 SP1及以上（以及对应的服务器版本）且包含 .net framework 4.6.2 - 4.8运行时即可
#### 多语言方案？
- 目前只有中文，文本较多，多语言较为麻烦。如有比较好的方案欢迎pr。
#### 为什么用WPF？
- 方便且轻量
#### 为什么用C#？
- 方便
#### 为什么用.net framework 4.6.2？
- 兼容性
#### 支持.net 7.0？
- 是的，打开sln直接编译会同时生成.net 7.0和.net framework 4.6.2的版本，但通常只发布.net framework 4.6.2的版本，因为大多数用户并未安装.net 7.0运行时。
