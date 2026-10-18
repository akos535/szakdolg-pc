using System.Runtime.InteropServices;

namespace Remote.Core.Input;

/// <summary>
/// https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-input
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct INPUT
{
    internal uint Type;
    internal MKH_INPUT Data;
}

[StructLayout(LayoutKind.Explicit)]
internal struct MKH_INPUT
{
    [FieldOffset(0)] internal MOUSEINPUT Mouse;
    [FieldOffset(0)] internal KEYBDINPUT Keyboard;
    [FieldOffset(0)] internal HARDWAREINPUT Hardware;
}

internal enum InputType : uint
{
    Mouse = 0,
    Keyboard = 1,
    Hardware = 2
}

/// <summary>
/// https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-mouseinput
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct MOUSEINPUT
{
    internal int X;
    internal int Y;
    internal uint MouseData;
    internal uint Flags;
    internal uint Time;
    internal UIntPtr ExtraInfo;
}

/// <summary>
/// https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-keybdinput
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct KEYBDINPUT
{
    internal ushort VK;
    internal ushort Scan;
    internal uint Flags;
    internal uint Time;
    internal UIntPtr ExtraInfo;
}

/// <summary>
/// https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-hardwareinput
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct HARDWAREINPUT
{
    internal uint Msg;
    internal uint ParamL;
    internal uint ParamH;
}