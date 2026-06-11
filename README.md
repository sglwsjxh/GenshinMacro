# Genshin Macro

原神鼠标宏工具

## 功能

- **X（鼠标侧键下键）** — 按住旋转视角
- **X2（鼠标侧键上键）** — 按住执行双玛头

## 安装

从 [Releases](https://github.com/sglwsjxh/GenshinMacro/releases/latest) 下载最新版本的 `GenshinMacro.exe`

## 从源码构建

1. 确保已有 Python 3.10+
2. 创建并激活虚拟环境：

```bash
python -m venv venv
.\venv\Scripts\activate
pip install -r requirements.txt
```

## 运行

```bash
pythonw main.py
```

## 注意事项

- **管理员权限**：首次启动时会自动请求管理员权限（因为 `pydirectinput` 需要硬件级输入权限）

## 免责声明

本工具仅供学习和娱乐使用，请勿用于任何违反游戏规则或法律法规的行为。使用者需自行承担使用风险

## 许可证

[MIT License](LICENSE)
