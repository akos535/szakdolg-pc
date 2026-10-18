using static Remote.Core.Input.KeyCode;

using System.Runtime.InteropServices;

namespace Remote.Core.Input;

internal partial class InputMaker
{
    [LibraryImport("user32.dll", SetLastError = true)]
    private static partial uint SendInput(uint numberOfInputs, INPUT[] inputs, int sizeOfInputStructure);

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
}
