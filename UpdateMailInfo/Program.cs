using System;
using System.Windows.Forms;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.File;

namespace UpdateMailInfo
{
    static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            string[] todayYMD = today.Split('-');
            string logFilePath = $"logs/{todayYMD[0]}/{todayYMD[0]}-{todayYMD[1]}/log-.txt";

            HandleConfiguration hc = new HandleConfiguration(); // 설정 정보 그릇
            hc.readConfig();

            string MinimumLogLevel = "0";
            if (!string.IsNullOrEmpty(HandleConfiguration.configData["MINIMUM_LOG_LEVEL"])) {
                MinimumLogLevel = HandleConfiguration.configData["MINIMUM_LOG_LEVEL"];
            }

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(logFilePath, 
                    rollingInterval: RollingInterval.Day, 
                    retainedFileCountLimit: null, 
                    fileSizeLimitBytes: 50 * 1024 * 1024,
                    rollOnFileSizeLimit: true,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}")
                .MinimumLevel.Is(MinimumLogLevelInConfigFile(MinimumLogLevel))
                .CreateLogger();
            
            Log.Information($"================UpdateMailInfo Application start================");
            Log.Information($"Minimum log level is [{MinimumLogLevelInConfigFile(MinimumLogLevel)}]");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new UpdateMailInfo());

            // 애플리케이션 종료 후 Serilog 정리
            Log.CloseAndFlush();
        }

        private static LogEventLevel MinimumLogLevelInConfigFile(string input)
        {
            if (int.TryParse(input, out int level))
            {
                switch (level)
                {
                    case 1:
                        return LogEventLevel.Debug;
                    case 2:
                        return LogEventLevel.Information;
                    case 3:
                        return LogEventLevel.Warning;
                    case 4:
                        return LogEventLevel.Error;
                    case 5:
                        return LogEventLevel.Fatal;
                }
            }
            return LogEventLevel.Information; // 기본적으로 Information으로 설정
        }
    }
}
