# OnKeyDoThing

Does a thing on a key.

![image](https://user-images.githubusercontent.com/739972/161840270-7016740e-1587-4377-b458-df71c239e757.png)

Sits in the system tray watching for global hotkeys, then carries out a series of actions which can include the following:
 - Activate a window
 - Minimize a window
 - Simulate a mouse click over a window

You can easily write your own actions by implementing the `IHotKeyAction` interface and filling in the `Invoke` method.

Multiple hotkeys can be defined, and multiple actions can be defined per hotkey.

Hotkeys can be captured using the Win32 `RegisterHotKey` API, or by using the `SetWindowsHookEx` API to watch the keyboard.

Warranty? None. This is just some code I wrote for fun, and to satisfy a very specific need I had. It might be what you want or it might not.
