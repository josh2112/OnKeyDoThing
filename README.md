# OnKeyDoThing
Does a thing on a key.

Watches for a global hotkey, then carries out a series of actions which can include the following:
 - Activate a window
 - Minimize a window
 - Simulate a mouse click over a window

You can easily write your own actions by implementing the `IHotKeyAction` interface and filling in the `Invoke` method.

Multiple hotkeys can be defined, and multiple actions can be defined per hotkey.

Hotkeys can be captured using the Win32 `RegisterHotKey` API, or by using the `SetWindowsHookEx` API to watch the keyboard.

Warranty? None. This is just some code I wrote for fun, and to satisfy a very specific need I had. It might be what you want or it might not.
