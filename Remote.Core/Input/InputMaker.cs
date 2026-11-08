using static Remote.Core.Input.KeyCode;
using System.Runtime.InteropServices;

namespace Remote.Core.Input;

internal partial class InputMaker
{
    private static readonly object _movementLock = new();
    private static double _remainingX;
    private static double _remainingY;

    [LibraryImport("user32.dll", SetLastError = true)]
    private static partial uint SendInput(uint numberOfInputs, INPUT[] inputs, int sizeOfInputStructure);

    /// <summary>
    /// Keyboard related inputs
    /// </summary>
    public static void SendKeyPress(KeyCode keyCode)
    {
        INPUT input = new INPUT{ Type = (uint)InputType.Keyboard };
        input.Data.Keyboard = new KEYBDINPUT()
        {
            VK = (ushort)keyCode,
            Scan = 0,
            Flags = 0,
            Time = 0,
            ExtraInfo = UIntPtr.Zero,
        };

        INPUT input2 = new INPUT { Type = (uint)InputType.Keyboard };
        input2.Data.Keyboard = new KEYBDINPUT()
        {
            VK = (ushort)keyCode,
            Scan = 0,
            Flags = 2,
            Time = 0,
            ExtraInfo = UIntPtr.Zero,
        };

        INPUT[] inputs = [input, input2];
        if(SendInput(2, inputs, Marshal.SizeOf<INPUT>()) == 0) {
            throw new Exception();
        }
    }
    public static void SendKeyDown(KeyCode keyCode)
    {
        INPUT input = new INPUT { Type = (uint)InputType.Keyboard };
        input.Data.Keyboard = new KEYBDINPUT()
        {
            VK = (ushort)keyCode,
            Scan = 0,
            Flags = 0,
            Time = 0,
            ExtraInfo = UIntPtr.Zero,
        };

        INPUT[] inputs = [input];
        if (SendInput(1, inputs, Marshal.SizeOf<INPUT>()) == 0)
        {
            throw new Exception();
        }
    }
    public static void SendKeyUp(KeyCode keyCode)
    {
        INPUT input = new INPUT { Type = (uint)InputType.Keyboard };
        input.Data.Keyboard = new KEYBDINPUT()
        {
            VK = (ushort)keyCode,
            Scan = 0,
            Flags = 2,
            Time = 0,
            ExtraInfo = UIntPtr.Zero,
        };

        INPUT[] inputs = [input];
        if (SendInput(1, inputs, Marshal.SizeOf<INPUT>()) == 0)
        {
            throw new Exception();
        }
    }

    /// <summary>
    /// Mouse related inputs
    /// </summary>
    public static void SendMouseInput(int x, int y)
    {
        uint MOUSEEVENTF_MOVE = 0x0001;

        INPUT input = new INPUT { Type = (uint)InputType.Mouse };
        input.Data.Mouse = new MOUSEINPUT()
        {
            X = x,
            Y = y,
            MouseData = 0,
            Flags = MOUSEEVENTF_MOVE,
            Time = 0,
            ExtraInfo = UIntPtr.Zero,
        };
        INPUT[] inputs = [input];
        if(SendInput(1, inputs, Marshal.SizeOf<INPUT>()) == 0)
        {
            throw new Exception();
        }
    }
    public static void MouseMoveRelative(double dx, double dy)
    {
        if(!double.IsFinite(dx) || !double.IsFinite(dy) || Math.Abs(dx) > 100_000 || Math.Abs(dy) > 100_000)
        {
            throw new ArgumentException("invalid movement");
        }

        lock(_movementLock)
        {
            double totalX = _remainingX + dx;
            double totalY = _remainingY + dy;

            int moveX = (int)Math.Truncate(totalX);
            int moveY = (int)Math.Truncate(totalY);

            if(moveX != 0 || moveY != 0)
            {
                SendMouseInput(moveX, moveY);
            }

            _remainingX = totalX - moveX;
            _remainingY = totalY - moveY;
        }
    }
    public static void SendMouseClick(MouseButton button)
    {
        uint downFlag = button switch
        {
            MouseButton.Left => 0x0002,
            MouseButton.Right => 0x0008,
            _ => throw new ArgumentOutOfRangeException(nameof(button))
        };

        uint upFlag = button switch
        {
            MouseButton.Left => 0x0004,
            MouseButton.Right => 0x0010,
            _ => throw new ArgumentOutOfRangeException(nameof(button))
        };

        INPUT mouseDown = new()
        {
            Type = (uint)InputType.Mouse
        };
        mouseDown.Data.Mouse = new MOUSEINPUT
        {
            X = 0,
            Y = 0,
            MouseData = 0,
            Flags = downFlag,
            Time = 0,
            ExtraInfo = UIntPtr.Zero
        };

        INPUT mouseUp = new()
        {
            Type = (uint)InputType.Mouse
        };
        mouseUp.Data.Mouse = new MOUSEINPUT
        {
            X = 0,
            Y = 0,
            MouseData = 0,
            Flags = upFlag,
            Time = 0,
            ExtraInfo = UIntPtr.Zero
        };

        INPUT[] inputs = [mouseDown, mouseUp];
        if(SendInput((uint)inputs.Length, inputs, Marshal.SizeOf<INPUT>()) == 0)
        {
            throw new InvalidOperationException("Mouse input not sent");
        }
    }
    public static void SendMouseDown(MouseButton button)
    {
        uint downFlag = button switch
        {
            MouseButton.Left => 0x0002,
            MouseButton.Right => 0x0008,
            _ => throw new ArgumentOutOfRangeException(nameof(button))
        };

        INPUT mouseDown = new()
        {
            Type = (uint)InputType.Mouse
        };
        mouseDown.Data.Mouse = new MOUSEINPUT
        {
            X = 0,
            Y = 0,
            MouseData = 0,
            Flags = downFlag,
            Time = 0,
            ExtraInfo = UIntPtr.Zero
        };

        INPUT[] inputs = [mouseDown];
        if (SendInput((uint)inputs.Length, inputs, Marshal.SizeOf<INPUT>()) != 0)
        {
            throw new InvalidOperationException("Mouse input not sent");
        }
    }
    public static void SendMouseUp(MouseButton button)
    {
        uint upFlag = button switch
        {
            MouseButton.Left => 0x0004,
            MouseButton.Right => 0x0010,
            _ => throw new ArgumentOutOfRangeException(nameof(button))
        };

        INPUT mouseUp = new()
        {
            Type = (uint)InputType.Mouse
        };
        mouseUp.Data.Mouse = new MOUSEINPUT
        {
            X = 0,
            Y = 0,
            MouseData = 0,
            Flags = upFlag,
            Time = 0,
            ExtraInfo = UIntPtr.Zero
        };

        INPUT[] inputs = [mouseUp];
        if (SendInput((uint)inputs.Length, inputs, Marshal.SizeOf<INPUT>()) != 0)
        {
            throw new InvalidOperationException("Mouse input not sent");
        }
    }
}
