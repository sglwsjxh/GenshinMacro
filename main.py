import sys
import tkinter as tk

from macros.base import is_admin, elevate
from macros.rotation import RotationWorker
from macros.rotation import _default_button_reader as rotation_btn_reader
from macros.rotation import _default_action as rotation_action
from macros.double_click import DoubleClickWorker
from macros.double_click import _default_button_reader as double_click_btn_reader
from macros.double_click import _default_action as double_click_action


class App(tk.Tk):
    def __init__(self):
        super().__init__()
        self.title("通用宏工具")
        self.geometry("350x350+400+200")
        self.resizable(False, False)
        try:
            self.iconbitmap("logo.ico")
        except Exception:
            pass
        self.protocol("WM_DELETE_WINDOW", self.on_close)
        self.bind("<Escape>", lambda e: self.on_close())

        self.rotation_worker = RotationWorker()
        self.double_click_worker = DoubleClickWorker()

        self.show_start_interface()

    def clear_widgets(self):
        for widget in self.winfo_children():
            widget.destroy()

    def show_start_interface(self):
        self.clear_widgets()
        tk.Label(self, text="通用宏工具", font=("黑体", 16)).place(
            relx=0.5, rely=0.25, anchor=tk.CENTER
        )
        tk.Button(
            self, text="启动", width=12, height=2, bg="lightgreen",
            font=("黑体", 14), command=self.start
        ).place(relx=0.5, rely=0.55, anchor=tk.CENTER)

    def show_stop_interface(self):
        self.clear_widgets()
        tk.Label(self, text="运行中", font=("黑体", 16), fg="green").place(
            relx=0.5, rely=0.15, anchor=tk.CENTER
        )
        tk.Label(self, text="X  →  旋转视角", font=("黑体", 12)).place(
            relx=0.5, rely=0.35, anchor=tk.CENTER
        )
        tk.Label(self, text="X2 →  双击右键", font=("黑体", 12)).place(
            relx=0.5, rely=0.48, anchor=tk.CENTER
        )
        tk.Button(
            self, text="停止", width=12, height=2, bg="lightcoral",
            font=("黑体", 14), command=self.stop
        ).place(relx=0.5, rely=0.72, anchor=tk.CENTER)

    def start(self):
        self.rotation_worker.start(rotation_btn_reader, rotation_action)
        self.double_click_worker.start(double_click_btn_reader, double_click_action)
        self.show_stop_interface()

    def stop(self):
        self.rotation_worker.stop()
        self.double_click_worker.stop()
        self.show_start_interface()

    def on_close(self):
        self.rotation_worker.stop()
        self.double_click_worker.stop()
        self.destroy()


if __name__ == "__main__":
    if not is_admin():
        print("需要管理员权限，正在请求提权...")
        elevate()

    app = App()
    app.mainloop()
