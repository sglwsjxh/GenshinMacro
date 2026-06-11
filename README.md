# Genshin Macro

原神鼠标宏工具 — C# WPF 重写版

## 功能

- **侧键1（X1）** — 按住持续旋转视角
- **侧键2（X2）** — 按住执行双键连击

## 系统要求

- Windows 10/11 （64位）
- [.NET 10.0 运行时](https://dotnet.microsoft.com/download/dotnet/10.0)（如果使用框架依赖发布版本）
- 管理员权限（因为需要使用 Win32 `SendInput` 模拟硬件输入）

## 下载与使用

从 [Releases](https://github.com/sglwsjxh/GenshinMacro/releases/latest) 下载最新版本的 `GenshinMacro.exe`。

直接运行即可，首次启动会请求管理员权限（UAC）。

## 从源码构建

### 前置要求

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### 构建与运行

```bash
# 还原依赖
dotnet restore

# 构建
dotnet build -c Release

# 运行（需要管理员权限）
dotnet run --project src/GenshinMacro/GenshinMacro.csproj

# 运行测试
dotnet test

# 发布为单文件 exe
dotnet publish src/GenshinMacro/GenshinMacro.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

发布产物位于 `dist/GenshinMacro.exe`。

## 项目结构

```
GenshinMacro.sln
├── src/GenshinMacro/          # WPF 应用程序
│   ├── App.xaml               # 应用入口
│   ├── MainWindow.xaml        # 主界面（双态 UI）
│   ├── ViewModels/            # MVVM ViewModel
│   ├── Interop/               # Win32 P/Invoke（SendInput, GetAsyncKeyState）
│   ├── Input/                 # 输入抽象接口 + Win32 实现
│   ├── MacroEngine/           # 宏引擎（RotationWorker, DoubleClickWorker）
│   └── Styles/                # UI 主题资源
├── tests/GenshinMacro.Tests/  # xUnit 测试项目
└── logo.ico                   # 应用图标
```

## 注意事项

- **管理员权限**：因使用 Win32 `SendInput` API 模拟输入，需要以管理员身份运行
- **反作弊兼容**：`SendInput` 属于硬件级输入模拟，兼容性较好
- **侧键检测**：使用 `GetAsyncKeyState` 轮询检测 X1/X2 侧键状态

## 免责声明

本工具仅供学习和娱乐使用，请勿用于任何违反游戏规则或法律法规的行为。使用者需自行承担使用风险。

## 许可证

[MIT License](LICENSE)
