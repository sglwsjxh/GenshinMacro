import pyautogui
import pydirectinput
import keyboard.mouse

from macros.base import MacroWorker, input_lock


SCREEN_WIDTH = pyautogui.size()[0]
ROTATION_SPEED = 0.02


def _default_action():
    pydirectinput.move(SCREEN_WIDTH, 0, duration=ROTATION_SPEED, relative=True)


def _default_button_reader():
    import keyboard.mouse
    return keyboard.mouse.is_pressed(keyboard.mouse.X)


class RotationWorker(MacroWorker):
    def __init__(self):
        super().__init__()

    def _run(self, button_reader, action_callable):
        while not self.stop_event.is_set():
            if button_reader():
                with input_lock:
                    action_callable()
            self.stop_event.wait(ROTATION_SPEED)
