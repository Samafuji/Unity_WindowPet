using UnityEngine;
using System;
using System.IO;
// 为了使用属性：DllImport
using System.Runtime.InteropServices;
using System.Text;
using Unity.VisualScripting;

public class TablePet : MonoBehaviour
{
    public int currentX;
    public int currentY;
    public int initCurrentX;
    public int initCurrentY;

    #region Win函数常量

    private struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    public struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    // 注意一定要指定字符集为Unicode，否则气泡内容不能支持中文
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct NOTIFYICONDATA
    {
        internal int cbSize;
        internal IntPtr hwnd;
        internal int uID;
        internal int uFlags;
        internal int uCallbackMessage;
        internal IntPtr hIcon;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        internal string szTip;

        internal int dwState; // 这里往下几个是 5.0 的精华
        internal int dwStateMask;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        internal string szInfo;

        internal int uTimeoutAndVersion;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        internal string szInfoTitle;

        internal int dwInfoFlags;
    }


    [DllImport("user32.dll")]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    // 获取显示在最上面的窗口
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    static extern IntPtr SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

    [DllImport("Dwmapi.dll")]
    static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll")]
    public static extern bool ReleaseCapture();

    [DllImport("user32.dll")]
    public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

    [DllImport("user32.dll")]
    static extern bool GetWindowRect(IntPtr hWnd, ref Rect lpRect);


    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);


    [DllImport("shell32.dll", EntryPoint = "Shell_NotifyIcon", CharSet = CharSet.Unicode)]
    private static extern bool Shell_NotifyIcon(int dwMessage, ref NOTIFYICONDATA lpData);

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr ExtractAssociatedIcon(IntPtr hInst, StringBuilder lpIconPath,
        out ushort lpiIcon);

    [DllImport("user32.dll")]
    public static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr lpIconName);

    public enum SystemIcons
    {
        IDI_APPLICATION = 32512,
        IDI_HAND = 32513,
        IDI_QUESTION = 32514,
        IDI_EXCLAMATION = 32515,
        IDI_ASTERISK = 32516,
        IDI_WINLOGO = 32517,
        IDI_WARNING = IDI_EXCLAMATION,
        IDI_ERROR = IDI_HAND,
        IDI_INFORMATION = IDI_ASTERISK,
    }


    private const int GWL_EXSTYLE = -20;
    private const int GWL_STYLE = -16;
    private const int WS_EX_LAYERED = 0x00080000;
    private const int WS_BORDER = 0x00800000;
    private const int WS_EX_TOOLWINDOW = 0x0080;

    private const int WS_CAPTION = 0x00C00000;
    private const int SWP_SHOWWINDOW = 0x0040;
    private const int LWA_COLORKEY = 0x00000001;
    private const int LWA_ALPHA = 0x00000002;
    private const int WS_EX_TRANSPARENT = 0x20;

    #endregion

    private IntPtr hwnd;

    void Awake()
    {
        var productName = Application.productName;


        // hwnd = FindWindow(null, productName);


        /*
        hwnd = FindWindow(null, productName);
         int intExTemp = GetWindowLong(hwnd, GWL_EXSTYLE);
        SetWindowLong(hwnd, GWL_EXSTYLE, intExTemp | WS_EX_TRANSPARENT | WS_EX_LAYERED);
        SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_BORDER & ~WS_CAPTION);
        currentX = 1000;
        currentY = 500;
        SetWindowPos(hwnd, -1, currentX, currentY, Screen.currentResolution.width, Screen.currentResolution.height, SWP_SHOWWINDOW);
        var margins = new MARGINS() { cxLeftWidth = -1 };
        DwmExtendFrameIntoClientArea(hwnd, ref margins);*/

        /*// 获取窗口句柄
        hwnd = GetForegroundWindow();

        // 设置窗口的属性
        SetWindowLong(hwnd, -16, 0x80000000);

        var margins = new MARGINS() { cxLeftWidth = -1 };

        // 将窗口框架扩展到工作区
        DwmExtendFrameIntoClientArea(hwnd, ref margins);

        // 设置窗口位置
        SetWindowPos(hwnd, -1, 0, 0, 0, 0, 2 | 1 | 64);*/

        hwnd = FindWindow(null, productName);
        //        ShowWindow(hwnd, 0);

        int intExTemp = GetWindowLong(hwnd, GWL_EXSTYLE);
        //SetWindowLong(hwnd, GWL_EXSTYLE, intExTemp | WS_EX_TRANSPARENT | WS_EX_LAYERED);
        SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_BORDER & ~WS_CAPTION);
        SetWindowLong(hwnd, GWL_EXSTYLE, GetWindowLong(hwnd, GWL_EXSTYLE) | WS_EX_TOOLWINDOW);
        currentX = 1000;
        currentY = 500;
        int sizexiX = Screen.currentResolution.width / 1920;
        int sizexiY = Screen.currentResolution.height / 1080;
        int windowWidget = 140 * sizexiX;
        int windowHeight = 90 * sizexiY;


        initCurrentX = Screen.currentResolution.width - windowWidget - 0;
        initCurrentY = Screen.currentResolution.height - windowHeight - 300;

        currentX = initCurrentX;
        currentY = initCurrentY;
        Debug.Log($"最终窗体:{windowWidget} ---- {windowHeight}");
        SetWindowPos(hwnd, -1, currentX, currentY, windowWidget, windowHeight, SWP_SHOWWINDOW);

        var margins = new MARGINS() { cxLeftWidth = -1 };
        DwmExtendFrameIntoClientArea(hwnd, ref margins);
    }


    private void Start()
    {
        if (Screen.fullScreen)
        {
            Screen.fullScreen = false;
        }
        
    }

    public bool MovementWindows(bool isLeft)
    {
        var react = new Rect() { };
        GetWindowRect(hwnd, ref react);


        currentX = react.Left;
        currentY = react.Top;


        if (isLeft)
        {
            currentX -= 1;
        }
        else
        {
            currentX += 1;
        }

        //Debug.Log($"窗体:{react.Right - react.Left}  --- {react.Bottom - react.Top}");
        SetWindowPos(hwnd, -1, currentX, currentY, react.Right - react.Left, react.Bottom - react.Top, SWP_SHOWWINDOW);
        if (isLeft && currentX < 80) return false;

        if (!isLeft && currentX > (initCurrentX - 80)) return true;


        return isLeft;
    }


    private void Update()
    {
        //if (hwnd != GetForegroundWindow()) SetForegroundWindow(hwnd);
        // 左键拖动
        if (Input.GetMouseButtonDown(0))
        {
            GameObject[] contextMenus = GameObject.FindGameObjectsWithTag("ContextMenuTag");
            if (contextMenus is { Length: > 0 })
            {
                foreach (GameObject contextMenu in contextMenus)
                {
                    if (contextMenu.activeSelf) return;
                    Debug.Log(contextMenu);
                }
            }

            ReleaseCapture();
            SendMessage(hwnd, 0xA1, 0x02, 0);
            SendMessage(hwnd, 0x0202, 0, 0);
        }
    }
}