using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MsgReader.Outlook;
using System.Security.Cryptography;
using System.IO;
using Serilog;

namespace UpdateMailInfo
{
    public partial class UpdateMailInfo : Form
    {
        readonly HandleConfiguration hc = new HandleConfiguration(); // 설정 정보 그릇
        UpdateHistory uh = new UpdateHistory(); // 기록 정보 그릇
        int previousDocNumber = -1; // 기록 정보
        int currentDocNumber = -1;
        List<MsgProperties> updateTargets = new List<MsgProperties>(); // 업데이트 대상
        bool isRangeUpdate = false;

        // 암호화 관련 클래스 내용 삭제
        public static class EncryptionHelper
        {
            public const string 암호화키 = "내용삭제";

            // 복호화 메소드 내용 삭제
            public static string Decrypt(string 문자열)
            {
                
                return 문자열;
             }
         }

        public UpdateMailInfo()
        {
            InitializeComponent();

            // 최대화 불가, 상단 닫기 버튼 제거
            ControlBox = false;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;

            // 로그박스 스크롤바 생성 및 읽기전용
            Load += selectDBandUpdateMailInfo;
        }

        private void selectDBandUpdateMailInfo(object sender, EventArgs e)
        {
            // read config.txt and put HandleConfiguration.configData[{key}]
            // config.txt 파일을 읽어 Dictionary 변수 HandleConfiguration.configData[{key}]에 저장
            hc.readConfig();

            /* for using LIBRARY in config.txt, read HandleConfiguration.configData["LIBRARY"]
             * if the value exists, Initialize the 'libraries[]' with a value or values, 
             * else, Initialize the 'libraries[]' with null   
             * 
             * config.txt 내 LIBRARY 변수를 사용하기 위해
             * HandleConfiguration.configData["LIBRARY"]를 읽어
             * value가 존재하면 값 또는 값들을 libraries 배열에 저장
             * value가 존재하지 않으면 null로 초기화
             */
            string[] libraries = HandleConfiguration.transformConfigData("LIBRARY");
            if(libraries == null) { 
                try
                {
                    throw new Exception("LIBRARY in config.txt is incorrect");
                }catch(Exception ex)
                {
                    Log.Error($"UpdateMailInfo - {ex.Message} \n code [{ex.HResult}] Error Message : {ex.StackTrace}");
                    Application.Exit();
                }
            }
     
            // 설정파일의 SQL_CONN을 이용해 db 연결
            if(!String.IsNullOrEmpty(HandleConfiguration.configData["SQL_CONN"]))
            {
                string DecryptConnectLocation = EncryptionHelper.Decrypt(HandleConfiguration.configData["SQL_CONN"]);

                foreach (string library in libraries) {
                    DBConnector dBConnector = new DBConnector(DecryptConnectLocation.Replace("DB_NAME", library));
                    int MaxDocNumber = -1;

                    // config.txt에 SPECIAL_DOCNUM에 대한 값이 null이나 빈칸이 아니면
                    if (!String.IsNullOrEmpty(HandleConfiguration.configData["SPECIAL_DOCNUM"]))
                    {
                        try {
                            /* config.txt에 범위 탐색 업데이트 숫자(SPECIAL_DOCNUM)가 형식에 맞게 적혀 있으면
                                * SPECIAL_DOCNUM의 값을 split하여 범위를 탐색하는 메소드를 실행 */
                            isRangeUpdate = true;
                            string[] range = HandleConfiguration.configData["SPECIAL_DOCNUM"].Split(',');
                            updateTargets = dBConnector.selectRangeDBInfo(range[0], range[1]);

                        } catch (Exception ex)
                        {
                            Log.Error($"SPECIAL_DOCNUM in config.txt is incorrect - {ex.Message} \n code [{ex.HResult}] Error Message : {ex.StackTrace}");
                            isRangeUpdate = false;
                            Application.Exit();
                        }
                    }
                    else
                    {
                        isRangeUpdate = false;
                        // readHistory() read history.txt
                        // If the attempt to read histroy.txt fails, the method returns -1
                        previousDocNumber = uh.readHistory(library);
                        if (previousDocNumber < 0) {
                            Log.Information($"the file history_{library}.txt not exists and start with DocNumber = 0");
                            previousDocNumber = 0;
                        } else {
                            // 기록파일의 정보를 이용해 db 읽어오기
                            Log.Information($"UpdateMailInfo - previous updated DocNumber in history_{library}.txt = {previousDocNumber}");
                        }
                        updateTargets = dBConnector.selectDBInfo(previousDocNumber);
                        MaxDocNumber = dBConnector.countDBInfo(previousDocNumber);
                    }

                    if (updateTargets != null && updateTargets.Count > 0)
                    {
                        TotalCount.Text = updateTargets.Count.ToString();
                        ReadCount.Text = "0";

                        foreach (var target in updateTargets)
                        {
                            MsgProperties msgProperties = new MsgProperties();
                            string filePath = target.FullPath;
                            currentDocNumber = target.DocNumber;

                            try
                            {
                                FileInfo fileInfo = new FileInfo(filePath);
                                {
                                    if (fileInfo.Exists)
                                    {
                                        long fileSizeInBytes = fileInfo.Length;
                                        if(fileSizeInBytes == 0)
                                        {
                                            throw new Exception($"currentDocNumber[{currentDocNumber}] file in filepath is incorrect - file size is {fileSizeInBytes} bytes");
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception($"currentDocNumber[{currentDocNumber}] file doesn't exists in filepath - {filePath}");
                                    }
                                }
                                Log.Information($"UpdateMailInfo - currentDocNumber[{currentDocNumber}] file exists");

                                if (filePath.Substring(filePath.LastIndexOf('.') + 1).ToUpper().Equals("MSG")) { 
                                    // .msg 파일 변수에 담기
                                    using (var msg = new MsgReader.Outlook.Storage.Message(filePath))
                                    {
                                        Log.Information($"UpdateMailInfo - currentDocNumber[{currentDocNumber}] file .MSG try reading");
                                        // 메일 속성 읽어오기
                                        msgProperties.Originator = msg.Sender?.DisplayName;
                                        msgProperties.Addressee = msg.GetEmailRecipients(RecipientType.To, false, false);
                                        msgProperties.CC = msg.GetEmailRecipients(RecipientType.Cc, false, false);
                                        msgProperties.BCC = msg.GetEmailRecipients(RecipientType.Bcc, false, false);
                                        msgProperties.SentDate = msg.SentOn ?? DateTime.MinValue;
                                        msgProperties.ReceivedDate = msg.ReceivedOn ?? DateTime.MinValue;

                                        Log.Information($"UpdateMailInfo - currentDocNumber[{currentDocNumber}] file - read .Msg properties : {msgProperties}");
                                    }
                                } 
                                else if (filePath.Substring(filePath.LastIndexOf('.') + 1).ToUpper().Equals("EML")) 
                                {
                                    // EML 파일 변수에 담기
                                    var eml = MsgReader.Mime.Message.Load(fileInfo);
                                    Log.Information($"UpdateMailInfo - currentDocNumber[{currentDocNumber}] file .EML try reading");
                                    // 메일 속성 읽어오기
                                    if (eml.Headers != null){
                                        if (eml.Headers.From != null && !string.IsNullOrEmpty(eml.Headers.From.DisplayName))
                                        { msgProperties.Originator = eml.Headers.From.DisplayName.ToString(); }
                                        else if (eml.Headers.From != null && !string.IsNullOrEmpty(eml.Headers.From.Address))
                                        { msgProperties.Originator = eml.Headers.From.Address.ToString(); }
                                        else { msgProperties.Originator = ""; }

                                        eml.Headers.To.ForEach(to => msgProperties.Addressee += "," + to.ToString());
                                        if (msgProperties.Addressee == null) { msgProperties.Addressee = ""; }
                                        else { msgProperties.Addressee = msgProperties.Addressee.Substring(1).Replace("\"", string.Empty); }

                                        eml.Headers.Cc.ForEach(cc => msgProperties.CC += "," + cc.ToString());
                                        if (msgProperties.CC == null) { msgProperties.CC = ""; }
                                        else { msgProperties.CC = msgProperties.CC.Substring(1).Replace("\"", string.Empty); }

                                        eml.Headers.Bcc.ForEach(bcc => msgProperties.BCC += "," + bcc.ToString());
                                        if (msgProperties.BCC == null) { msgProperties.BCC = ""; }
                                        else { msgProperties.BCC = msgProperties.BCC.Substring(1).Replace("\"", string.Empty); }

                                        if (eml.Headers.DateSent != null) { msgProperties.SentDate = eml.Headers.DateSent; }
                                        else { msgProperties.SentDate = DateTime.MinValue; }

                                        if (eml.Headers.Received != null && eml.Headers.Received.Count > 0) {
                                            msgProperties.ReceivedDate = eml.Headers.Received.Last().Date;
                                        } else { 
                                            msgProperties.ReceivedDate = DateTime.MinValue; 
                                        }
                                        Log.Information($"UpdateMailInfo - currentDocNumber[{currentDocNumber}] file - read .eml properties : {msgProperties}");
                                    }
                                    else
                                    {
                                        throw new Exception($"currentDocNumber[{currentDocNumber}] file's eml.Headers is null");
                                    }
                                    
                                } else
                                {
                                    throw new Exception($"currentDocNumber[{currentDocNumber}] file - db filepath does not end with (msg or eml)");
                                }
                                int updateResult = -1;
                                updateResult = dBConnector.updateDBInfo(currentDocNumber, msgProperties);

                                if (updateResult > 0)
                                {
                                    /*범위 업데이트 시에는 기록파일을 update하지 않으므로 
                                        * 범위 업데이트가 아닐때 최근에 업데이트한 DocNumber를 기록한다.
                                        * */
                                    if (!isRangeUpdate)
                                    {
                                        uh.writeToHistoryTxtFile(library, currentDocNumber.ToString());
                                    }
                                    ReadCount.Text = (Int32.Parse(ReadCount.Text) + 1).ToString();

                                    Log.Information($"UpdateMailInfo.selectDBandUpdateMailInfo - update success : DocNumber = [{currentDocNumber}]" +
                                        $" ({ReadCount.Text} / {TotalCount.Text} row(s) updated. in LIBRARY={library})");
                                }
                            } catch (Exception ex) {
                                Log.Error($"UpdateMailInfo - DocNumber[{currentDocNumber}] failed to read Msg Properties : Error Message - {ex.Message} \n code [{ex.HResult}] Error Message : {ex.StackTrace}");
                            }           
                        }
                    }
                    else // target이 null일때 - 검색에 실패했거나 업데이트 대상이 없는 경우
                    {
                        Log.Warning($"UpdateMailInfo - the target doesn't exist");
                    }

                    if (!isRangeUpdate && MaxDocNumber > 0)
                    {
                        uh.writeToHistoryTxtFile(library, MaxDocNumber.ToString());
                        Log.Information($"UpdateMailInfo - update done until DocNumber = {MaxDocNumber} in history_{library}.txt");
                    }
                }
            }
            else // config.txt에서 불러온 HandleConfiguration.configData["SQL_CONN"]가 null 또는 빈값인 경우
            {
                Log.Error($"SQL_CONN in [config.txt] is empty");
                Application.Exit();
            }
            isRangeUpdate = false;
            Application.Exit();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Log.Information($"=================UpdateMailInfo Application end=================");
            base.OnFormClosed(e);
        }

        private void UpdateMailInfo_Load(object sender, EventArgs e){}
    }
}
