namespace Remote.Core.Input;

internal enum KeyCode : uint
{
    VK_LBUTTON = 0x01,
    VK_RBUTTON = 0x02,
    VK_CANCEL = 0x03,
    VK_MBUTTON = 0x04,
    VK_XBUTTON1 = 0x05,
    VK_XBUTTON2 = 0x06,
    VK_BACK = 0x08,
    VK_TAB = 0x09,
    VK_CLEAR = 0x0C,
    VK_RETURN = 0x0D,
    VK_SHIFT = 0x10,
    VK_CONTROL = 0x11,
    VK_MENU = 0x12,
    VK_PAUSE = 0x13,
    VK_CAPITAL = 0x14,
    VK_ESCAPE = 0x1B,
    VK_SPACE = 0x20,
    VK_PRIOR = 0x21,
    VK_NEXT = 0X22,
    VK_END = 0x23,
    VK_HOME = 0x24,
    VK_LEFT = 0x25,
    VK_UP = 0x26,
    VK_RIGHT = 0x27,
    VK_DOWN = 0x28,
    VK_SELECT = 0x29,
    VK_PRINT = 0x2A,
    VK_EXECUTE = 0x2B,
    VK_SNAPSHOT = 0x2C,
    VK_INSERT = 0x2D,
    VK_DELETE = 0x2E,

    // NUMBERS
    VK_ZERO = 0x30,
    VK_ONE = 0x31,
    VK_TWO = 0x32,
    VK_THREE = 0x33,
    VK_FOUR = 0x34,
    VK_FIVE = 0x35,
    VK_SIX = 0x36,
    VK_SEVEN = 0x37,
    VK_EIGHT = 0x38,
    VK_NINE = 0x39,

    // LETTERS
    VK_A = 0x41,
    VK_B = 0x42,
    VK_C = 0x43,
    VK_D = 0x44,
    VK_E = 0x45,
    VK_F = 0x46,
    VK_G = 0x47,
    VK_H = 0x48,
    VK_I = 0x49,
    VK_J = 0x4A,
    VK_K = 0x4B,
    VK_L = 0x4C,
    VK_M = 0x4D,
    VK_N = 0x4E,
    VK_O = 0x4F,
    VK_P = 0x50,
    VK_Q = 0x51,
    VK_R = 0x52,
    VK_S = 0x53,
    VK_T = 0x54,
    VK_U = 0x55,
    VK_V = 0x56,
    VK_W = 0x57,
    VK_X = 0x58,
    VK_Y = 0x59,
    VK_Z = 0x5A,

    // HUN LETTERS
    /// <summary>
    /// éÉ $
    /// </summary>
    VK_OEM_1 = 0xBA,
    /// <summary>
    /// óÓ
    /// </summary>
    VK_OEM_PLUS = 0xBB,
    /// <summary>
    /// , ? ;
    /// </summary>
    VK_OEM_COMMA = 0xBC,
    /// <summary>
    /// -_*
    /// </summary>
    VK_OEM_MINUS = 0xBD,
    /// <summary>
    /// . : >
    /// </summary>
    VK_OEM_PERIOD = 0xBE,
    /// <summary>
    /// üÜ
    /// </summary>
    VK_OEM_2 = 0xBF,
    /// <summary>
    /// öÖ
    /// </summary>
    VK_OEM_3 = 0xC0,
    /// <summary>
    /// őŐ ÷
    /// </summary>
    VK_OEM_4 = 0xDB,
    /// <summary>
    /// űŰ ¤
    /// </summary>
    VK_OEM_5 = 0xDC,
    /// <summary>
    /// úÚ ×
    /// </summary>
    VK_OEM_6 = 0xDD,
    /// <summary>
    /// áÁ ß
    /// </summary>
    VK_OEM_7 = 0xDE,
    /// <summary>
    /// íÍ
    /// </summary>
    VK_OEM_102 = 0xE2,

    // VOLUME
    VK_VOLUME_MUTE = 0xAD,
    VK_VOLUME_DOWN = 0xAE,
    VK_VOLUME_UP = 0xAF
}
