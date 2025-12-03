using Microsoft.Extensions.Logging;
using PokerStars.Accessibility.Core;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace PokerStars.Accessibility.Services;

/// <summary>
/// Screen capture service implementation for Windows
/// </summary>
[SupportedOSPlatform("windows")]
public class WindowsScreenCaptureService : IScreenCaptureService
{
    private readonly ILogger<WindowsScreenCaptureService> _logger;

    public WindowsScreenCaptureService(ILogger<WindowsScreenCaptureService> logger)
    {
        _logger = logger;
    }

    public async Task<ScreenCapture> CaptureScreenAsync()
    {
        return await Task.Run(() =>
        {
            try
            {
                _logger.LogInformation("Capturing screen...");
                
                // Get screen dimensions
                int screenWidth = GetSystemMetrics(SM_CXSCREEN);
                int screenHeight = GetSystemMetrics(SM_CYSCREEN);

                if (screenWidth <= 0 || screenHeight <= 0)
                {
                    return ScreenCapture.CreateFailure("Could not get screen dimensions");
                }

                // Capture the screen using GDI
                IntPtr hdcScreen = GetDC(IntPtr.Zero);
                IntPtr hdcMemory = CreateCompatibleDC(hdcScreen);
                IntPtr hBitmap = CreateCompatibleBitmap(hdcScreen, screenWidth, screenHeight);
                IntPtr hOld = SelectObject(hdcMemory, hBitmap);

                BitBlt(hdcMemory, 0, 0, screenWidth, screenHeight, hdcScreen, 0, 0, SRCCOPY);

                SelectObject(hdcMemory, hOld);

                // Convert to bytes (simplified - in real implementation would convert to PNG/BMP)
                var capture = ScreenCapture.CreateSuccess(
                    Array.Empty<byte>(), // Placeholder - real implementation would extract bitmap bytes
                    screenWidth,
                    screenHeight,
                    "Desktop"
                );

                // Cleanup
                DeleteObject(hBitmap);
                DeleteDC(hdcMemory);
                ReleaseDC(IntPtr.Zero, hdcScreen);

                _logger.LogInformation("Screen captured: {Width}x{Height}", screenWidth, screenHeight);
                return capture;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error capturing screen");
                return ScreenCapture.CreateFailure($"Error capturing screen: {ex.Message}");
            }
        });
    }

    public async Task<ScreenCapture> CaptureWindowAsync(string windowTitle)
    {
        return await Task.Run(() =>
        {
            try
            {
                _logger.LogInformation("Looking for window: {WindowTitle}", windowTitle);
                
                IntPtr hwnd = FindWindow(null, windowTitle);
                if (hwnd == IntPtr.Zero)
                {
                    // Try to find window containing the title
                    hwnd = FindWindowContaining(windowTitle);
                }

                if (hwnd == IntPtr.Zero)
                {
                    return ScreenCapture.CreateFailure($"Window not found: {windowTitle}");
                }

                // Get window rect
                if (!GetWindowRect(hwnd, out RECT rect))
                {
                    return ScreenCapture.CreateFailure("Could not get window dimensions");
                }

                int width = rect.Right - rect.Left;
                int height = rect.Bottom - rect.Top;

                if (width <= 0 || height <= 0)
                {
                    return ScreenCapture.CreateFailure("Invalid window dimensions");
                }

                // Capture the window
                IntPtr hdcWindow = GetDC(hwnd);
                IntPtr hdcMemory = CreateCompatibleDC(hdcWindow);
                IntPtr hBitmap = CreateCompatibleBitmap(hdcWindow, width, height);
                IntPtr hOld = SelectObject(hdcMemory, hBitmap);

                PrintWindow(hwnd, hdcMemory, PW_CLIENTONLY);

                SelectObject(hdcMemory, hOld);

                var capture = ScreenCapture.CreateSuccess(
                    Array.Empty<byte>(), // Placeholder
                    width,
                    height,
                    windowTitle
                );

                // Cleanup
                DeleteObject(hBitmap);
                DeleteDC(hdcMemory);
                ReleaseDC(hwnd, hdcWindow);

                _logger.LogInformation("Window captured: {WindowTitle} ({Width}x{Height})", windowTitle, width, height);
                return capture;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error capturing window: {WindowTitle}", windowTitle);
                return ScreenCapture.CreateFailure($"Error capturing window: {ex.Message}");
            }
        });
    }

    public async Task<IEnumerable<string>> FindPokerStarsWindowsAsync()
    {
        return await Task.Run(() =>
        {
            var windows = new List<string>();
            
            EnumWindows((hwnd, lParam) =>
            {
                int length = GetWindowTextLength(hwnd);
                if (length > 0)
                {
                    var builder = new System.Text.StringBuilder(length + 1);
                    GetWindowText(hwnd, builder, builder.Capacity);
                    string title = builder.ToString();
                    
                    if (title.Contains("PokerStars", StringComparison.OrdinalIgnoreCase) ||
                        title.Contains("Hold'em", StringComparison.OrdinalIgnoreCase) ||
                        title.Contains("Omaha", StringComparison.OrdinalIgnoreCase))
                    {
                        windows.Add(title);
                    }
                }
                return true;
            }, IntPtr.Zero);

            _logger.LogInformation("Found {Count} PokerStars windows", windows.Count);
            return windows.AsEnumerable();
        });
    }

    public async Task<ScreenCapture> CapturePokerStarsTableAsync()
    {
        var windows = await FindPokerStarsWindowsAsync();
        var tableWindow = windows.FirstOrDefault(w => 
            w.Contains("Hold'em", StringComparison.OrdinalIgnoreCase) ||
            w.Contains("Omaha", StringComparison.OrdinalIgnoreCase) ||
            w.Contains("Table", StringComparison.OrdinalIgnoreCase));

        if (string.IsNullOrEmpty(tableWindow))
        {
            return ScreenCapture.CreateFailure("No PokerStars table window found");
        }

        return await CaptureWindowAsync(tableWindow);
    }

    private IntPtr FindWindowContaining(string partialTitle)
    {
        IntPtr found = IntPtr.Zero;
        
        EnumWindows((hwnd, lParam) =>
        {
            int length = GetWindowTextLength(hwnd);
            if (length > 0)
            {
                var builder = new System.Text.StringBuilder(length + 1);
                GetWindowText(hwnd, builder, builder.Capacity);
                string title = builder.ToString();
                
                if (title.Contains(partialTitle, StringComparison.OrdinalIgnoreCase))
                {
                    found = hwnd;
                    return false; // Stop enumeration
                }
            }
            return true;
        }, IntPtr.Zero);

        return found;
    }

    #region Win32 API

    private const int SM_CXSCREEN = 0;
    private const int SM_CYSCREEN = 1;
    private const int SRCCOPY = 0x00CC0020;
    private const uint PW_CLIENTONLY = 0x1;

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    private delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int nIndex);

    [DllImport("user32.dll")]
    private static extern IntPtr GetDC(IntPtr hwnd);

    [DllImport("user32.dll")]
    private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int width, int height);

    [DllImport("gdi32.dll")]
    private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

    [DllImport("gdi32.dll")]
    private static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int width, int height, IntPtr hdcSrc, int xSrc, int ySrc, int rop);

    [DllImport("gdi32.dll")]
    private static extern bool DeleteObject(IntPtr hObject);

    [DllImport("gdi32.dll")]
    private static extern bool DeleteDC(IntPtr hdc);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern IntPtr FindWindow(string? lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

    [DllImport("user32.dll")]
    private static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, uint nFlags);

    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int GetWindowTextLength(IntPtr hwnd);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int GetWindowText(IntPtr hwnd, System.Text.StringBuilder lpString, int nMaxCount);

    #endregion
}
