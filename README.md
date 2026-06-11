# Genshin Macro

原神鼠标宏工具

## 功能

- **侧键1（X1）** — 按住龙王自动旋转
- **侧键2（X2）** — 按住执行双玛头宏

## 系统要求

- Windows 10/11 （64位）
- [.NET 10.0 运行时](https://dotnet.microsoft.com/download/dotnet/10.0)

## 下载

从 [Releases](https://github.com/sglwsjxh/GenshinMacro/releases/latest) 下载最新版本的 `GenshinMacro.exe`

## 从源码构建

```bash
# 还原依赖
dotnet restore

# 构建
dotnet build -c Release

# 运行
dotnet run --project GenshinMacro.csproj

# 运行测试
dotnet test

# 发布
dotnet publish GenshinMacro.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## 项目结构

```
GenshinMacro.sln
├── GenshinMacro.csproj          # WPF 应用程序
├── App.xaml                     # 应用入口（自动提权逻辑）
├── MainWindow.xaml              # 主界面
├── app.manifest                 # 应用程序清单
├── ViewModels/                  # MVVM ViewModel
├── Interop/                     # Win32 P/Invoke（SendInput, GetAsyncKeyState）
├── Input/                       # 输入抽象接口 + Win32 实现
├── MacroEngine/                 # 宏引擎（RotationWorker, DoubleClickWorker）
├── Styles/                      # UI 主题资源
├── tests/                       # xUnit 测试项目
└── logo.ico                     # 应用图标
```

## 注意事项

- **管理员权限**：启动时会自动检测并提权（UAC），使用 Win32 `SendInput` API 模拟输入
- **侧键检测**：使用 `GetAsyncKeyState` 轮询检测 X1/X2 侧键状态

## 免责声明

本工具仅供学习和娱乐使用，请勿用于任何违反游戏规则或法律法规的行为。使用者需自行承担使用风险

## 许可证

[MIT License](LICENSE)
