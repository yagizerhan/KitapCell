using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace KitapCell.Web
{
    /// <summary>
    /// KitapCell embedded web server — runs on top of Kestrel.
    /// Operates in the background without blocking the WinForms UI thread.
    /// </summary>
    public static class WebServer
    {
        /// <summary>The running Kestrel WebApplication instance. Null when the server is stopped.</summary>
        private static WebApplication? _app;

        /// <summary>Cancellation token source used to gracefully shut down the Kestrel host.</summary>
        private static CancellationTokenSource? _cts;

        /// <summary>True when the web server is currently running and accepting HTTP connections.</summary>
        public static bool IsRunning { get; private set; }

        /// <summary>The port number the server is currently listening on. Defaults to 5000.</summary>
        public static int CurrentPort { get; private set; } = 5000;

        /// <summary>Thread-safe queue of the last 500 server log entries (newest at the back).</summary>
        public static readonly ConcurrentQueue<string> Logs = new();

        /// <summary>Raised on the thread-pool whenever a new log entry is added. Subscribed by the Settings UI.</summary>
        public static event Action<string>? LogAdded;

        /// <summary>
        /// Appends a timestamped entry to the internal log queue.
        /// Automatically trims the queue to the last 500 entries to prevent unbounded memory growth.
        /// Fires the <see cref="LogAdded"/> event so subscribers (e.g. the Settings log panel) can update.
        /// </summary>
        public static void AddLog(string msg)
        {
            var entry = $"[{DateTime.Now:HH:mm:ss}] {msg}";
            Logs.Enqueue(entry);
            while (Logs.Count > 500) Logs.TryDequeue(out _); // Keep the last 500 entries
            LogAdded?.Invoke(entry);
        }

        /// <summary>
        /// Resolves the machine's primary local (LAN) IPv4 address.
        /// Falls back to "127.0.0.1" if no suitable address is found.
        /// </summary>
        public static string GetLocalIPAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        return ip.ToString();
            }
            catch { }
            return "127.0.0.1";
        }

        /// <summary>Returns the full LAN URL of the web server (e.g. "http://192.168.1.10:5000").</summary>
        public static string GetNetworkUrl() => $"http://{GetLocalIPAddress()}:{CurrentPort}";

        /// <summary>
        /// Builds and starts the Kestrel web server on the given port.
        /// Configures static file serving for <c>wwwroot</c> and <c>Assets</c>,
        /// registers the API routes, and starts listening for HTTP connections.
        /// Does nothing and returns <c>true</c> if the server is already running.
        /// </summary>
        /// <param name="port">TCP port to listen on. Default: 5000.</param>
        /// <returns>True if the server started successfully; false on error.</returns>
        public static async Task<bool> StartAsync(int port = 5000)
        {
            if (IsRunning) return true;

            try
            {
                CurrentPort = port;
                AddLog($"Sunucu başlatılıyor... Port: {port}");

                var builder = WebApplication.CreateSlimBuilder(Array.Empty<string>());
                builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
                builder.Logging.ClearProviders();
                builder.Logging.AddProvider(new InternalLogProvider());


                var app = builder.Build();

                // Global exception handler — writes unhandled errors to the internal log
                app.Use(async (context, next) =>
                {
                    try   { await next(context); }
                    catch (Exception ex)
                    {
                        AddLog($"[ERROR 500] {context.Request.Path} — {ex.Message}");
                        if (!context.Response.HasStarted)
                        {
                            context.Response.StatusCode  = 500;
                            context.Response.ContentType = "application/json";
                            var json = $"{{\"error\":\"{ex.Message.Replace("\"","'")}\"}}";
                            await context.Response.WriteAsync(json);
                        }
                    }
                });

                // Serve static files from the wwwroot folder
                var wwwroot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot");
                if (Directory.Exists(wwwroot))
                {
                    var fileProvider = new PhysicalFileProvider(wwwroot);
                    app.UseDefaultFiles(new DefaultFilesOptions
                    {
                        FileProvider = fileProvider,
                        RequestPath = ""
                    });
                    app.UseStaticFiles(new StaticFileOptions
                    {
                        FileProvider = fileProvider,
                        RequestPath = ""
                    });
                }
                else
                {
                    AddLog($"[WARNING] wwwroot folder not found: {wwwroot}");
                }

                // Serve static assets (PDF.js, Bibi EPUB viewer, etc.)
                var assets = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");
                if (Directory.Exists(assets))
                {
                    app.UseStaticFiles(new StaticFileOptions
                    {
                        FileProvider = new PhysicalFileProvider(assets),
                        RequestPath  = "/Assets"
                    });
                    AddLog($"Assets service active: {assets}");
                }

                // Register API routes
                ApiEndpoints.Map(app);

                _cts = new CancellationTokenSource();
                _app  = app;

                await _app.StartAsync(_cts.Token);
                IsRunning = true;
                AddLog($"Server running. Network address: {GetNetworkUrl()}");
                return true;
            }
            catch (Exception ex)
            {
                AddLog($"[ERROR] Could not start: {ex.Message}");
                IsRunning = false;
                _app  = null;
                _cts  = null;
                return false;
            }
        }

        /// <summary>
        /// Gracefully shuts down the Kestrel web server.
        /// Cancels the host's token, waits for active requests to drain, then
        /// clears the <c>_app</c> and <c>_cts</c> references.
        /// </summary>
        public static async Task StopAsync()
        {
            if (!IsRunning || _app == null) return;
            try
            {
                AddLog("Stopping server...");
                _cts?.Cancel();
                await _app.StopAsync();
            }
            catch (Exception ex)
            {
                AddLog($"[ERROR] Stop failed: {ex.Message}");
            }
            finally
            {
                _app = null;
                _cts = null;
                IsRunning = false;
                AddLog("Server stopped.");
            }
        }

        // Captures Kestrel's internal log messages and routes them to AddLog
        private sealed class InternalLogProvider : ILoggerProvider
        {
            public ILogger CreateLogger(string _) => new InternalLogger();
            public void Dispose() { }

            private sealed class InternalLogger : ILogger
            {
                public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
                public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Warning;
                public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
                    Exception? exception, Func<TState, Exception?, string> formatter)
                {
                    if (logLevel >= LogLevel.Warning)
                        WebServer.AddLog($"[{logLevel}] {formatter(state, exception)}");
                }
            }
        }
    }
}
