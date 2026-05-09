using System;
using System.IO;
using System.Text.Json;

namespace KitapCell.Core
{
    /// <summary>
    /// Holds application-wide configuration settings.
    /// The single instance is accessed via <see cref="SettingsManager.Config"/>
    /// and is serialized to / deserialized from <c>settings.json</c>.
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// Default loan duration in days for new borrow transactions.
        /// Default: 15 days.
        /// </summary>
        public int DefaultLoanDays { get; set; } = 15;

        /// <summary>
        /// Port number the Kestrel web server listens on.
        /// Default: 5000. Can be changed from the Settings screen.
        /// </summary>
        public int WebServerPort { get; set; } = 5000;

        /// <summary>
        /// Controls how PDF files are opened.
        /// InApp: in-app viewer (WebView2) | System: default OS PDF application.
        /// </summary>
        public PdfOpenMode PdfOpenMode { get; set; } = PdfOpenMode.InApp;

        /// <summary>
        /// When true (default), all web content requires a user session.
        /// When false (Guest Mode), anonymous visitors can browse books and
        /// read digital content (PDF/EPUB) without logging in.
        /// User-specific actions (ratings, favorites, profile, history)
        /// always require authentication regardless of this setting.
        /// </summary>
        public bool RequireLoginForWebServer { get; set; } = true;
    }

    public enum PdfOpenMode
    {
        InApp,
        System
    }

    /// <summary>
    /// Loads and saves JSON-based application settings.
    /// Settings are stored in <c>settings.json</c> in the application's base directory.
    /// If the file does not exist it is created automatically with default values.
    /// </summary>
    public static class SettingsManager
    {
        private static readonly string SettingsFile =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

        /// <summary>Access point for the current application configuration.</summary>
        public static AppConfig Config { get; private set; } = new AppConfig();

        /// <summary>
        /// Loads settings from <c>settings.json</c>.
        /// Creates the file with default values on first run.
        /// Falls back to default <see cref="AppConfig"/> values if the file is corrupt or unreadable.
        /// </summary>
        public static void Load()
        {
            try
            {
                if (File.Exists(SettingsFile))
                {
                    string json = File.ReadAllText(SettingsFile);
                    var cfg = JsonSerializer.Deserialize<AppConfig>(json);
                    if (cfg != null) Config = cfg;
                }
                else
                {
                    Save(); // Write defaults to disk on first launch
                }
            }
            catch
            {
                // Fall back to defaults if the file is corrupt or cannot be read
                Config = new AppConfig();
            }
        }

        /// <summary>
        /// Writes the current <see cref="Config"/> object to <c>settings.json</c>.
        /// Write errors are silently ignored; the application continues to run.
        /// </summary>
        public static void Save()
        {
            try
            {
                string json = JsonSerializer.Serialize(
                    Config,
                    new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsFile, json);
            }
            catch { }
        }
    }
}
