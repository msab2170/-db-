using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Serilog;

namespace UpdateMailInfo
{
    class HandleConfiguration
    {
        public string configFilePath = "config.txt";

        // 설정 정보를 저장할 Dictionary 생성
        public static Dictionary<string, string> configData = new Dictionary<string, string>();
        
        // config.txt 파일 내 설정 정보를 읽어 Dictionary 인스턴스인 configData에 담는다.
        public void readConfig()
        {
            if (File.Exists(configFilePath))
            {
                try
                {
                    // 파일의 모든 텍스트를 읽어옴
                    string[] lines = File.ReadAllLines(configFilePath);

                    // 각 줄을 순회하면서 설정 정보를 추출하여 딕셔너리에 저장
                    foreach (string line in lines)
                    {
                        // 빈 줄이면 패스, #는 config.txt 주석 처리
                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                            continue;

                        string key = line.Substring(0, line.IndexOf("="));
                        string value = line.Substring(line.IndexOf("=") + 1);
                        configData[key] = value;
                        
                    }

                    foreach (var aData in configData)
                    {
                        Log.Information($"HandleConfiguration.readConfig - {aData.Key}= {aData.Value}");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"HandleConfiguration.readConfig - An error occurred while reading the config file: {ex.Message}");
                }
            }
            else
            {
                Log.Error("HandleConfiguration.readConfig - [config.txt] file not found.");
            } 
        }

        // ,로 여러개 입력가능한 config data를 배열로 가공
        public static string[] transformConfigData(string configString)
        {
            string[] ConfigDataArr;
            if (!string.IsNullOrEmpty(HandleConfiguration.configData[configString])
                && HandleConfiguration.configData[configString].Contains(","))
            {
                ConfigDataArr = HandleConfiguration.configData[configString].Split(',');
            }
            else if (!string.IsNullOrEmpty(HandleConfiguration.configData[configString]))
            {
                ConfigDataArr = new string[1];
                ConfigDataArr[0] = HandleConfiguration.configData[configString];
            }
            else
            {
                ConfigDataArr = null;
            }
            return ConfigDataArr;
        }
    }
}
