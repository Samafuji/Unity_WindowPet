using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class WindowPet : MonoBehaviour
{
    private int currentX;
    private int currentY;

    #region Win函数常量
    private struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    [DllImport("user32.dll")]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

    [DllImport("Dwmapi.dll")] // clear background
    static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);
    // https://youtu.be/RqgsGaMPZTw?si=hAORpz9i24jpxja0

    [DllImport("user32.dll")]
    static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

    private const int GWL_EXSTYLE = -20;
    private const int GWL_STYLE = -16;
    private const int WS_EX_LAYERED = 0x00080000;
    private const int WS_BORDER = 0x00800000;
    private const int WS_CAPTION = 0x00C00000;
    private const int SWP_SHOWWINDOW = 0x0040;
    private const int LWA_COLORKEY = 0x00000001;
    private const int LWA_ALPHA = 0x00000002;
    private const int WS_EX_TRANSPARENT = 0x20;
    #endregion

    private IntPtr hwnd;
    private bool isTransparent;

    void Awake()
    {
        var productName = Application.productName;
#if UNITY_EDITOR
#else
        hwnd = FindWindow(null, productName);
        SetInitialWindowSettings();
#endif
    }

    private void SetInitialWindowSettings()
    {
        int intExTemp = GetWindowLong(hwnd, GWL_EXSTYLE);
        SetWindowLong(hwnd, GWL_EXSTYLE, intExTemp | WS_EX_LAYERED);
        // SetWindowLong(hwnd, GWL_EXSTYLE, intExTemp | WS_EX_TRANSPARENT | WS_EX_LAYERED);
        SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_BORDER & ~WS_CAPTION);
        currentX = 0;
        currentY = 0;

        SetWindowPos(hwnd, -1, currentX, currentY, Screen.currentResolution.width, Screen.currentResolution.height, SWP_SHOWWINDOW);

        var margins = new MARGINS() { cxLeftWidth = -1 };
        DwmExtendFrameIntoClientArea(hwnd, ref margins);

        SetLayeredWindowAttributes(hwnd, 0, 0, LWA_COLORKEY);
    }
    void Update()
    {
        CheckObjectUnderCursor();
    }
    private void CheckObjectUnderCursor()
    {
        // Perform a raycast to determine if the mouse is over a specific object.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("Unclickable") && !isTransparent)
                {
                    ToggleTransparency(true);
                }
                else if (!hit.collider.gameObject.CompareTag("Unclickable") && isTransparent)
                {
                    ToggleTransparency(false);
                }
            }
            else if (isTransparent)
            {
                ToggleTransparency(false);
            }
        }
        else if (isTransparent)
        {
            ToggleTransparency(false);
        }
    }

    private void ToggleTransparency(bool makeTransparent)
    {
        int intExTemp = GetWindowLong(hwnd, GWL_EXSTYLE);

        if (makeTransparent)
        {
            SetWindowLong(hwnd, GWL_EXSTYLE, intExTemp | WS_EX_TRANSPARENT);
            isTransparent = true;
            print(makeTransparent);
        }
        else
        {
            SetWindowLong(hwnd, GWL_EXSTYLE, intExTemp & ~WS_EX_TRANSPARENT);
            isTransparent = false;
            print(makeTransparent);
        }
    }

}
