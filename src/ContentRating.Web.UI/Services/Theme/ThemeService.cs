using Microsoft.JSInterop;
using MudBlazor;

namespace ContentRating.Web.UI.Services.Theme;

public class ThemeService
{
    private readonly IJSRuntime _jsRuntime;
    private bool _isDarkMode = false;
    private bool _isInitialized = false;
    private const string THEME_KEY = "theme-preference";

    public event Action? OnThemeChanged;

    public bool IsDarkMode => _isDarkMode;

    public MudTheme CurrentTheme => _isDarkMode ? DarkTheme : LightTheme;

    public ThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        try
        {
            var savedTheme = await _jsRuntime.InvokeAsync<string?>(
                "localStorage.getItem",
                THEME_KEY
            );
            if (!string.IsNullOrEmpty(savedTheme))
            {
                _isDarkMode = savedTheme == "dark";
            }
            else
            {
                // Проверяем системные настройки темы
                var prefersDark = await _jsRuntime.InvokeAsync<bool>(
                    "themeHelpers.getSystemPreference"
                );
                _isDarkMode = prefersDark;
                await SaveThemeAsync();
            }
        }
        catch
        {
            // Если localStorage недоступен, используем светлую тему по умолчанию
            _isDarkMode = false;
        }

        _isInitialized = true;
        OnThemeChanged?.Invoke();
    }

    public async Task ToggleThemeAsync()
    {
        _isDarkMode = !_isDarkMode;
        await SaveThemeAsync();
        OnThemeChanged?.Invoke();
    }

    public void ToggleTheme()
    {
        _ = ToggleThemeAsync();
    }

    private async Task SaveThemeAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync(
                "localStorage.setItem",
                THEME_KEY,
                _isDarkMode ? "dark" : "light"
            );
        }
        catch
        {
            // Игнорируем ошибки сохранения
        }
    }

    public MudTheme LightTheme { get; } =
        new()
        {
            PaletteLight = new PaletteLight()
            {
                Primary = "#1976d2", // Синий
                Secondary = "#dc004e", // Красный
                Tertiary = "#2e7d32", // Зеленый
                Info = "#0288d1",
                Success = "#2e7d32",
                Warning = "#f57c00",
                Error = "#d32f2f",
                Dark = "#1a1a1a",

                AppbarBackground = "#1976d2",
                AppbarText = Colors.Shades.White,
                DrawerBackground = "#ffffff",
                DrawerText = "rgba(0,0,0,0.87)",
                Background = "#f5f5f5",
                BackgroundGray = "#f8f9fa",
                Surface = "#ffffff",

                TextPrimary = "rgba(0,0,0,0.87)",
                TextSecondary = "rgba(0,0,0,0.6)",

                ActionDefault = "rgba(0,0,0,0.54)",
                ActionDisabled = "rgba(0,0,0,0.26)",
                ActionDisabledBackground = "rgba(0,0,0,0.12)",

                Divider = "rgba(0,0,0,0.12)",
                DividerLight = "rgba(0,0,0,0.06)",

                TableLines = "rgba(224, 224, 224, 1)",
                TableStriped = "rgba(0,0,0,0.02)",
                TableHover = "rgba(0,0,0,0.04)",

                LinesDefault = "rgba(0,0,0,0.12)",
                LinesInputs = "rgba(0,0,0,0.42)",

                GrayDefault = "rgba(0,0,0,0.26)",
                GrayLight = "rgba(0,0,0,0.12)",
                GrayLighter = "rgba(0,0,0,0.06)",
                GrayDark = "rgba(0,0,0,0.54)",
                GrayDarker = "rgba(0,0,0,0.87)",

                OverlayDark = "rgba(33,33,33,0.4)",
                OverlayLight = "rgba(255,255,255,0.4)",
            },
        };

    // Темная тема с кастомными цветами
    public MudTheme DarkTheme { get; } =
        new()
        {
            PaletteDark = new PaletteDark()
            {
                Primary = "#90caf9", // Светло-синий для темной темы
                Secondary = "#f48fb1", // Светло-розовый
                Tertiary = "#a5d6a7", // Светло-зеленый
                Info = "#29b6f6",
                Success = "#66bb6a",
                Warning = "#ffa726",
                Error = "#f44336",
                Dark = "#27272f",

                AppbarBackground = "#1e1e1e",
                AppbarText = Colors.Shades.White,
                DrawerBackground = "#1e1e1e",
                DrawerText = "rgba(255,255,255,0.87)",
                Background = "#121212",
                BackgroundGray = "#1e1e1e",
                Surface = "#1e1e1e",

                TextPrimary = "rgba(255,255,255,0.87)",
                TextSecondary = "rgba(255,255,255,0.6)",

                ActionDefault = "rgba(255,255,255,0.54)",
                ActionDisabled = "rgba(255,255,255,0.26)",
                ActionDisabledBackground = "rgba(255,255,255,0.12)",

                Divider = "rgba(255,255,255,0.12)",
                DividerLight = "rgba(255,255,255,0.06)",

                TableLines = "rgba(81, 81, 81, 1)",
                TableStriped = "rgba(255,255,255,0.02)",
                TableHover = "rgba(255,255,255,0.04)",

                LinesDefault = "rgba(255,255,255,0.12)",
                LinesInputs = "rgba(255,255,255,0.42)",

                GrayDefault = "rgba(255,255,255,0.26)",
                GrayLight = "rgba(255,255,255,0.12)",
                GrayLighter = "rgba(255,255,255,0.06)",
                GrayDark = "rgba(255,255,255,0.54)",
                GrayDarker = "rgba(255,255,255,0.87)",

                OverlayDark = "rgba(33,33,33,0.4)",
                OverlayLight = "rgba(255,255,255,0.4)",
            },
        };
}
