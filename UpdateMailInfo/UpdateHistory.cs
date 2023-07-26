using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace UpdateMailInfo
{
    class UpdateHistory
    {
        public string historyFilePath = "";
        public string historyFileName = "history";
        public string ext = ".txt";
        // 설정 정보를 저장할 Dictionary 생성
        public static Dictionary<string, string> historyData = new Dictionary<string, string>();

        // history.txt 읽어와 값 return, history.txt 파일이 없거나 오류 발생시 -1 리턴
        public int readHistory(string library)
        {
            string historyFile = historyFilePath + historyFileName + "_" + library + ext;
            int result = -1;
            
            if (File.Exists(historyFile))
            {
                try
                {
                    string[] lines = File.ReadAllLines(historyFile);

                    string[] parts = lines[0].Split('=');
                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();
                        historyData[key] = value;

                        try
                        {
                            result = Int32.Parse(value);
                        }
                        catch (Exception e)
                        {
                            Log.Error($"UpdateHistory.readHistory - the value in {historyFile} cannot parse - Int32 \n{e}");
                        }
                    }

                    foreach (var aData in historyData)
                    {
                        Log.Information($"UpdateHistory.readHistory - {aData.Key}= {aData.Value} in {historyFile}");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"UpdateHistory.readHistory - An error occurred while reading the file {historyFile} : {ex.Message}");
                }
            }
            else
            {
                Log.Warning($"UpdateHistory.readHistory - [{historyFile}] file not found.");
            }
            return result;
        }

        // update 후 history.txt에 기록 저장하는 메소드
        public string writeToHistoryTxtFile(string library, string updateData)
        {
            string historyFile = historyFilePath + historyFileName + "_" + library + ext;
            try
            {
                File.WriteAllText(historyFile, "PREVIOUS_UPDATE_COUNT="  + updateData);
                Log.Information($"UpdateHistory.writeToHistoryTxtFile - Data written to file [{historyFile}]: PREVIOUS_UPDATE_COUNT={updateData}");
            }
            catch (Exception ex)
            {
                Log.Error($"UpdateHistory.writeToHistoryTxtFile - An error occurred while writing data to file [{historyFile}]: {ex.Message}");
            }
            return historyFile;
        }
    }
}
