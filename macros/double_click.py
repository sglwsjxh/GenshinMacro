import time
import mouse
import pyautogui

from macros.base import MacroWorker, input_lock

class DoubleClickWorker(MacroWorker):
    def _run(self, button_reader, action_callable):
        while not self.stop_event.is_set():
            if button_reader():
                with input_lock:
                    action_callable()
            self.stop_event.wait(0.05)

def _default_button_reader():
    return mouse.is_pressed('x2')

def _default_action():
        pyautogui.mouseDown()
        time.sleep(0.1)
        pyautogui.click(button='right')
        time.sleep(0.03)
        pyautogui.mouseUp()
        time.sleep(0.03)

        pyautogui.mouseDown()
        time.sleep(0.1)
        pyautogui.click(button='right')
        time.sleep(0.03)
        pyautogui.mouseUp()
        time.sleep(0.08)

def main():
    worker = DoubleClickWorker()
    worker.start(_default_button_reader, _default_action)

    print("DoubleClickWorker running. Press Ctrl+C to stop.")
    try:
        while True:
            time.sleep(1)
    except KeyboardInterrupt:
        print("\nStopping...")
        worker.stop()

if __name__ == "__main__":
    main()
