using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateMailInfo
{
    class MsgProperties
    {
        public int DocNumber { get; set; }
        public string FullPath { get; set; }
        
        public string Originator { get; set; } // 보낸사람
        public string Addressee { get; set; } // 받는사람
        public string CC { get; set; }
        public string BCC { get; set; }
        public DateTime SentDate { get; set; }
        public DateTime ReceivedDate { get; set; }

        public int Version { get; set; }

        public override string ToString()
        {
            return $"MsgProperties - DocNumber = {DocNumber + ""}, FullPath = {FullPath}, " +
                $"Originator = {Originator + ""}, Addressee = {Addressee + ""}, CC = {CC + ""}, BCC = {BCC + ""}, " +
                $"SentDate = {SentDate + ""}, ReceivedDate = {ReceivedDate + ""}, Version = {Version + ""}";
        }
    }
}
