import sys
import ctypes
import threading
import pyautogui

pyautogui.FAILSAFE = False

input_lock = threading.Lock()

def is_admin():
    try:
        return bool(ctypes.windll.shell32.IsUserAnAdmin())
    except Exception:
        return False

def elevate():
    ctypes.windll.shell32.ShellExecuteW(
        None, "runas", sys.executable, " ".join(sys.argv), None, 1
    )
    sys.exit()

class MacroWorker:
    def __init__(self):
        self.stop_event = threading.Event()
        self.thread = None
        self.running = False

    def start(self, button_reader, action_callable):
        if self.running:
            return
        self.stop_event.clear()
        self.thread = threading.Thread(
            target=self._run, args=(button_reader, action_callable), daemon=False
        )
        self.thread.start()
        self.running = True

    def stop(self):
        if not self.running:
            return
        self.stop_event.set()
        if self.thread:
            self.thread.join()
        self.running = False

    def _run(self, button_reader, action_callable):
        while not self.stop_event.is_set():
            if button_reader():
                action_callable()
            self.stop_event.wait(0.2)
