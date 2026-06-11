# Genshin Macro - 原神鼠标宏工具

原神鼠标宏工具 — 统一的 Tkinter GUI 管理两个鼠标宏

## 功能

- **X（鼠标侧键下键）** — 按住持续旋转视角
- **X2（鼠标侧键上键）** — 按住重复执行双玛头

## 安装

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

启动后会出现 GUI 窗口，点击「启动」按钮激活两个宏。点击「停止」按钮或关闭窗口即可退出。

## 注意事项

- **管理员权限**：首次启动时会自动请求管理员权限（因为 `pydirectinput` 需要硬件级输入权限）
