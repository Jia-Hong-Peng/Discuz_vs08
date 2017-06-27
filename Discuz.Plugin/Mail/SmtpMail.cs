using System;
using System.Text;
using System.Collections;
using System.Net.Sockets;
using System.Web.Mail;
using System.Net;

namespace Discuz.Plugin.Mail
{

    #region 郵件發送程序

    /// <summary>
    /// 郵件發送程序
    /// </summary>    
    public class SmtpMail : ISmtpMail
    {

        private string enter = "\r\n";

        /// <summary>
        /// 設定語言代碼,默認設定為GB2312,如不需要可設置為""
        /// </summary>
        private string _charset = "UTF-8";

        /// <summary>
        /// 發件人地址
        /// </summary>
        private string _from = "";

        /// <summary>
        /// 發件人姓名
        /// </summary>
        private string _fromName = "";

        /// <summary>
        /// 回復郵件地址
        /// </summary>
        ///public string ReplyTo="";

        /// <summary>
        /// 收件人姓名
        /// </summary>	
        private string _recipientName = "";

        /// <summary>
        /// 收件人列表
        /// </summary>
        private Hashtable Recipient = new Hashtable();

        /// <summary>
        /// 郵件服務器域名
        /// </summary>	
        private string mailserver = "";

        /// <summary>
        /// 郵件服務器端口號
        /// </summary>	
        private int mailserverport = 25;

        /// <summary>
        /// SMTP認證時使用的用戶名
        /// </summary>
        private string username = "";

        /// <summary>
        /// SMTP認證時使用的密碼
        /// </summary>
        private string password = "";

        /// <summary>
        /// 是否需要SMTP驗證
        /// </summary>		
        private bool ESmtp = false;

        /// <summary>
        /// 是否Html郵件
        /// </summary>		
        private bool _html = false;


        /// <summary>
        /// 郵件附件列表
        /// </summary>
        private IList Attachments;

        /// <summary>
        /// 郵件發送優先級,可設置為"High","Normal","Low"或"1","3","5"
        /// </summary>
        private string priority = "Normal";

        /// <summary>
        /// 郵件主題
        /// </summary>		
        private string _subject;

        /// <summary>
        /// 郵件正文
        /// </summary>		
        private string _body;

        /// <summary>
        /// 密送收件人列表
        /// </summary>
        ///private Hashtable RecipientBCC=new Hashtable();

        /// <summary>
        /// 收件人數量
        /// </summary>
        private int RecipientNum = 0;

        /// <summary>
        /// 最多收件人數量
        /// </summary>
        private int recipientmaxnum = 5;

        /// <summary>
        /// 密件收件人數量
        /// </summary>
        ///private int RecipientBCCNum=0;

        /// <summary>
        /// 錯誤消息反饋
        /// </summary>
        private string errmsg;

        /// <summary>
        /// TcpClient對像,用於連接服務器
        /// </summary>	
        private TcpClient tc;

        /// <summary>
        /// NetworkStream對像
        /// </summary>	
        private NetworkStream ns;

        /// <summary>
        /// 服務器交互記錄
        /// </summary>
        private string logs = "";

        /// <summary>
        /// SMTP錯誤代碼哈希表
        /// </summary>
        private Hashtable ErrCodeHT = new Hashtable();

        /// <summary>
        /// SMTP正確代碼哈希表
        /// </summary>
        private Hashtable RightCodeHT = new Hashtable();


        /// <summary>
        /// </summary>
        public SmtpMail()
        {
            Attachments = new System.Collections.ArrayList();
            SMTPCodeAdd();
        }

        #region Properties


        /// <summary>
        /// 郵件主題
        /// </summary>
        public string Subject
        {
            get
            {
                return this._subject;
            }
            set
            {
                this._subject = value;
            }
        }

        /// <summary>
        /// 郵件正文
        /// </summary>
        public string Body
        {
            get
            {
                return this._body;
            }
            set
            {
                this._body = value;
            }
        }


        /// <summary>
        /// 發件人地址
        /// </summary>
        public string From
        {
            get
            {
                return _from;
            }
            set
            {
                this._from = value;
            }
        }

        /// <summary>
        /// 設定語言代碼,默認設定為GB2312,如不需要可設置為""
        /// </summary>
        public string Charset
        {
            get
            {
                return this._charset;
            }
            set
            {
                this._charset = value;
            }
        }

        /// <summary>
        /// 發件人姓名
        /// </summary>
        public string FromName
        {
            get
            {
                return this._fromName;
            }
            set
            {
                this._fromName = value;
            }
        }

        /// <summary>
        /// 收件人姓名
        /// </summary>
        public string RecipientName
        {
            get
            {
                return this._recipientName;
            }
            set
            {
                this._recipientName = value;
            }
        }

        /// <summary>
        /// 郵件服務器域名和驗證信息
        /// 形如："user:pass@www.server.com:25",也可省略次要信息。如"user:pass@www.server.com"或"www.server.com"
        /// </summary>	
        public string MailDomain
        {
            set
            {
                string maidomain = value.Trim();
                int tempint;

                if (maidomain != "")
                {
                    tempint = maidomain.IndexOf("@");
                    if (tempint != -1)
                    {
                        string str = maidomain.Substring(0, tempint);
                        MailServerUserName = str.Substring(0, str.IndexOf(":"));
                        MailServerPassWord = str.Substring(str.IndexOf(":") + 1, str.Length - str.IndexOf(":") - 1);
                        maidomain = maidomain.Substring(tempint + 1, maidomain.Length - tempint - 1);
                    }

                    tempint = maidomain.IndexOf(":");
                    if (tempint != -1)
                    {
                        mailserver = maidomain.Substring(0, tempint);
                        mailserverport = System.Convert.ToInt32(maidomain.Substring(tempint + 1, maidomain.Length - tempint - 1));
                    }
                    else
                    {
                        mailserver = maidomain;

                    }


                }

            }
        }



        /// <summary>
        /// 郵件服務器端口號
        /// </summary>	
        public int MailDomainPort
        {
            set
            {
                mailserverport = value;
            }
        }



        /// <summary>
        /// SMTP認證時使用的用戶名
        /// </summary>
        public string MailServerUserName
        {
            set
            {
                if (value.Trim() != "")
                {
                    username = value.Trim();
                    ESmtp = true;
                }
                else
                {
                    username = "";
                    ESmtp = false;
                }
            }
        }

        /// <summary>
        /// SMTP認證時使用的密碼
        /// </summary>
        public string MailServerPassWord
        {
            set
            {
                password = value;
            }
        }


        public string ErrCodeHTMessage(string code)
        {
            return ErrCodeHT[code].ToString();
        }
        /// <summary>
        /// 郵件發送優先級,可設置為"High","Normal","Low"或"1","3","5"
        /// </summary>
        public string Priority
        {
            set
            {
                switch (value.ToLower())
                {
                    case "high":
                        priority = "High";
                        break;

                    case "1":
                        priority = "High";
                        break;

                    case "normal":
                        priority = "Normal";
                        break;

                    case "3":
                        priority = "Normal";
                        break;

                    case "low":
                        priority = "Low";
                        break;

                    case "5":
                        priority = "Low";
                        break;

                    default:
                        priority = "High";
                        break;
                }
            }
        }

        /// <summary>
        ///  是否Html郵件
        /// </summary>
        public bool Html
        {
            get
            {
                return this._html;
            }
            set
            {
                this._html = value;
            }
        }


        /// <summary>
        /// 錯誤消息反饋
        /// </summary>		
        public string ErrorMessage
        {
            get
            {
                return errmsg;
            }
        }

        /// <summary>
        /// 服務器交互記錄
        /// </summary>
        public string Logs
        {
            get
            {
                return logs;
            }
        }

        /// <summary>
        /// 最多收件人數量
        /// </summary>
        public int RecipientMaxNum
        {
            set
            {
                recipientmaxnum = value;
            }
        }


        #endregion

        #region Methods


        /// <summary>
        /// 添加郵件附件
        /// </summary>
        /// <param name="FilePath">附件絕對路徑</param>
        public void AddAttachment(params string[] FilePath)
        {
            if (FilePath == null)
            {
                throw (new ArgumentNullException("FilePath"));
            }
            for (int i = 0; i < FilePath.Length; i++)
            {
                Attachments.Add(FilePath[i]);
            }
        }

        /// <summary>
        /// 添加一組收件人(不超過recipientmaxnum個),參數為字符串數組
        /// </summary>
        /// <param name="Recipients">保存有收件人地址的字符串數組(不超過recipientmaxnum個)</param>	
        public bool AddRecipient(params string[] Recipients)
        {
            if (Recipient == null)
            {
                Dispose();
                throw (new ArgumentNullException("Recipients"));
            }
            for (int i = 0; i < Recipients.Length; i++)
            {
                string recipient = Recipients[i].Trim();
                if (recipient == String.Empty)
                {
                    Dispose();
                    throw new ArgumentNullException("Recipients[" + i + "]");
                }
                if (recipient.IndexOf("@") == -1)
                {
                    Dispose();
                    throw new ArgumentException("Recipients.IndexOf(\"@\")==-1", "Recipients");
                }
                if (!AddRecipient(recipient))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 發送郵件方法,所有參數均通過屬性設置。
        /// </summary>
        public bool Send()
        {
            if (mailserver.Trim() == "")
            {
                throw (new ArgumentNullException("Recipient", "必須指定SMTP服務器"));
            }

            return SendEmail();

        }


        /// <summary>
        /// 發送郵件方法
        /// </summary>
        /// <param name="smtpserver">smtp服務器信息,如"username:password@www.smtpserver.com:25",也可去掉部分次要信息,如"www.smtpserver.com"</param>
        public bool Send(string smtpserver)
        {
            MailDomain = smtpserver;
            return Send();
        }


        /// <summary>
        /// 發送郵件方法
        /// </summary>
        /// <param name="smtpserver">smtp服務器信息,如"username:password@www.smtpserver.com:25",也可去掉部分次要信息,如"www.smtpserver.com"</param>
        /// <param name="from">發件人mail地址</param>
        /// <param name="fromname">發件人姓名</param>
        /// <param name="to">收件人地址</param>
        /// <param name="toname">收件人姓名</param>
        /// <param name="html">是否HTML郵件</param>
        /// <param name="subject">郵件主題</param>
        /// <param name="body">郵件正文</param>
        public bool Send(string smtpserver, string from, string fromname, string to, string toname, bool html, string subject, string body)
        {
            MailDomain = smtpserver;
            From = from;
            FromName = fromname;
            AddRecipient(to);
            RecipientName = toname;
            Html = html;
            Subject = subject;
            Body = body;
            return Send();
        }


        #endregion

        #region Private Helper Functions

        /// <summary>
        /// 添加一個收件人
        /// </summary>	
        /// <param name="Recipients">收件人地址</param>
        private bool AddRecipient(string Recipients)
        {
            //修改等待郵件驗證的用戶重複發送的問題
            Recipient.Clear();
            RecipientNum = 0;
            if (RecipientNum < recipientmaxnum)
            {
                Recipient.Add(RecipientNum, Recipients);
                RecipientNum++;
                return true;
            }
            else
            {
                Dispose();
                throw (new ArgumentOutOfRangeException("Recipients", "收件人過多不可多於 " + recipientmaxnum + " 個"));
            }
        }

        void Dispose()
        {
            if (ns != null)
                ns.Close();
            if (tc != null)
                tc.Close();
        }

        /// <summary>
        /// SMTP回應代碼哈希表
        /// </summary>
        private void SMTPCodeAdd()
        {
            ErrCodeHT.Add("500", "郵箱地址錯誤");
            ErrCodeHT.Add("501", "參數格式錯誤");
            ErrCodeHT.Add("502", "命令不可實現");
            ErrCodeHT.Add("503", "服務器需要SMTP驗證");
            ErrCodeHT.Add("504", "命令參數不可實現");
            ErrCodeHT.Add("421", "服務未就緒,關閉傳輸信道");
            ErrCodeHT.Add("450", "要求的郵件操作未完成,郵箱不可用(例如,郵箱忙)");
            ErrCodeHT.Add("550", "要求的郵件操作未完成,郵箱不可用(例如,郵箱未找到,或不可訪問)");
            ErrCodeHT.Add("451", "放棄要求的操作;處理過程中出錯");
            ErrCodeHT.Add("551", "用戶非本地,請嘗試<forward-path>");
            ErrCodeHT.Add("452", "系統存儲不足, 要求的操作未執行");
            ErrCodeHT.Add("552", "過量的存儲分配, 要求的操作未執行");
            ErrCodeHT.Add("553", "郵箱名不可用, 要求的操作未執行(例如郵箱格式錯誤)");
            ErrCodeHT.Add("432", "需要一個密碼轉換");
            ErrCodeHT.Add("534", "認證機制過於簡單");
            ErrCodeHT.Add("538", "當前請求的認證機制需要加密");
            ErrCodeHT.Add("454", "臨時認證失敗");
            ErrCodeHT.Add("530", "需要認證");

            RightCodeHT.Add("220", "服務就緒");
            RightCodeHT.Add("250", "要求的郵件操作完成");
            RightCodeHT.Add("251", "用戶非本地, 將轉發向<forward-path>");
            RightCodeHT.Add("354", "開始郵件輸入, 以<enter>.<enter>結束");
            RightCodeHT.Add("221", "服務關閉傳輸信道");
            RightCodeHT.Add("334", "服務器響應驗證Base64字符串");
            RightCodeHT.Add("235", "驗證成功");
        }


        /// <summary>
        /// 將字符串編碼為Base64字符串
        /// </summary>
        /// <param name="str">要編碼的字符串</param>
        private string Base64Encode(string str)
        {
            byte[] barray;
            barray = Encoding.Default.GetBytes(str);
            return Convert.ToBase64String(barray);
        }


        /// <summary>
        /// 將Base64字符串解碼為普通字符串
        /// </summary>
        /// <param name="str">要解碼的字符串</param>
        private string Base64Decode(string str)
        {
            byte[] barray;
            barray = Convert.FromBase64String(str);
            return Encoding.Default.GetString(barray);
        }


        /// <summary>
        /// 得到上傳附件的文件流
        /// </summary>
        /// <param name="FilePath">附件的絕對路徑</param>
        private string GetStream(string FilePath)
        {
            //建立文件流對像
            System.IO.FileStream FileStr = new System.IO.FileStream(FilePath, System.IO.FileMode.Open);
            byte[] by = new byte[System.Convert.ToInt32(FileStr.Length)];
            FileStr.Read(by, 0, by.Length);
            FileStr.Close();
            return (System.Convert.ToBase64String(by));
        }

        /// <summary>
        /// 發送SMTP命令
        /// </summary>	
        private bool SendCommand(string str)
        {
            byte[] WriteBuffer;
            if (str == null || str.Trim() == String.Empty)
            {
                return true;
            }
            logs += str;
            WriteBuffer = Encoding.Default.GetBytes(str);
            try
            {
                ns.Write(WriteBuffer, 0, WriteBuffer.Length);
            }
            catch
            {
                errmsg = "網絡連接錯誤";
                return false;
            }
            return true;
        }

        /// <summary>
        /// 接收SMTP服務器回應
        /// </summary>
        private string RecvResponse()
        {
            int StreamSize;
            string ReturnValue = String.Empty;
            byte[] ReadBuffer = new byte[1024];
            try
            {
                StreamSize = ns.Read(ReadBuffer, 0, ReadBuffer.Length);
            }
            catch
            {
                errmsg = "網絡連接錯誤";
                return "false";
            }

            if (StreamSize == 0)
            {
                return ReturnValue;
            }
            else
            {
                ReturnValue = Encoding.Default.GetString(ReadBuffer).Substring(0, StreamSize);
                logs += ReturnValue + this.enter;
                return ReturnValue;
            }
        }

        /// <summary>
        /// 與服務器交互,發送一條命令並接收回應。
        /// </summary>
        /// <param name="str">一個要發送的命令</param>
        /// <param name="errstr">如果錯誤,要反饋的信息</param>
        private bool Dialog(string str, string errstr)
        {
            if (str == null || str.Trim() == "")
            {
                return true;
            }
            if (SendCommand(str))
            {
                string RR = RecvResponse();
                if (RR == "false")
                {
                    return false;
                }
                string RRCode = RR.Substring(0, 3);
                if (RightCodeHT[RRCode] != null)
                {
                    return true;
                }
                else
                {
                    if (ErrCodeHT[RRCode] != null)
                    {
                        errmsg += (RRCode + ErrCodeHT[RRCode].ToString());
                        errmsg += enter;
                    }
                    else
                    {
                        errmsg += RR;
                    }
                    errmsg += errstr;
                    return false;
                }
            }
            else
            {
                return false;
            }

        }


        /// <summary>
        /// 與服務器交互,發送一組命令並接收回應。
        /// </summary>

        private bool Dialog(string[] str, string errstr)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (!Dialog(str[i], ""))
                {
                    errmsg += enter;
                    errmsg += errstr;
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// SendEmail
        /// </summary>
        /// <returns></returns>
        private bool SendEmail()
        {
            //連接網絡
            try
            {
                tc = new TcpClient(mailserver, mailserverport);
            }
            catch (Exception e)
            {
                errmsg = e.ToString();
                return false;
            }

            ns = tc.GetStream();


            //驗證網絡連接是否正確
            if (RightCodeHT[RecvResponse().Substring(0, 3)] == null)
            {
                errmsg = "網絡連接失敗";
                return false;
            }


            string[] SendBuffer;
            string SendBufferstr;

            //進行SMTP驗證
            if (ESmtp)
            {
                SendBuffer = new String[4];
                SendBuffer[0] = "EHLO " + mailserver + enter;
                SendBuffer[1] = "AUTH LOGIN" + enter;
                SendBuffer[2] = Base64Encode(username) + enter;
                SendBuffer[3] = Base64Encode(password) + enter;
                if (!Dialog(SendBuffer, "SMTP服務器驗證失敗,請核對用戶名和密碼。"))
                    return false;
            }
            else
            {
                SendBufferstr = "HELO " + mailserver + enter;
                if (!Dialog(SendBufferstr, ""))
                    return false;
            }

            //
            SendBufferstr = "MAIL FROM:<" + From + ">" + enter;
            if (!Dialog(SendBufferstr, "發件人地址錯誤,或不能為空"))
                return false;

            //
            SendBuffer = new string[recipientmaxnum];
            for (int i = 0; i < Recipient.Count; i++)
            {
                SendBuffer[i] = "RCPT TO:<" + Recipient[i].ToString() + ">" + enter;

            }
            if (!Dialog(SendBuffer, "收件人地址有誤"))
                return false;

            /*
                        SendBuffer=new string[10];
                        for(int i=0;i<RecipientBCC.Count;i++)
                        {

                            SendBuffer[i]="RCPT TO:<" + RecipientBCC[i].ToString() +">" + enter;

                        }

                        if(!Dialog(SendBuffer,"密件收件人地址有誤"))
                            return false;
            */
            SendBufferstr = "DATA" + enter;
            if (!Dialog(SendBufferstr, ""))
                return false;

            SendBufferstr = "From:" + FromName + "<" + From + ">" + enter;

            //if(ReplyTo.Trim()!="")
            //{
            //	SendBufferstr+="Reply-To: " + ReplyTo + enter;
            //}

            //SendBufferstr += "To:" + (Discuz.Common.Utils.StrIsNullOrEmpty(RecipientName)?"":Base64Encode(RecipientName)) + "<" + Recipient[0] + ">" + enter;
            SendBufferstr += "To:=?" + Charset.ToUpper() + "?B?" + (Discuz.Common.Utils.StrIsNullOrEmpty(RecipientName) ? "" : Base64Encode(RecipientName)) + "?=" + "<" + Recipient[0] + ">" + enter;

            //註釋掉抄送代碼
            SendBufferstr += "CC:";
            for (int i = 1; i < Recipient.Count; i++)
            {
                SendBufferstr += Recipient[i].ToString() + "<" + Recipient[i].ToString() + ">,";
            }
            SendBufferstr += enter;

            SendBufferstr += ((Subject == String.Empty || Subject == null) ? "Subject:" : ((Charset == "") ? ("Subject:" + Subject) : ("Subject:" + "=?" + Charset.ToUpper() + "?B?" + Base64Encode(Subject) + "?="))) + enter;
            SendBufferstr += "X-Priority:" + priority + enter;
            SendBufferstr += "X-MSMail-Priority:" + priority + enter;
            SendBufferstr += "Importance:" + priority + enter;
            SendBufferstr += "X-Mailer: Lion.Web.Mail.SmtpMail Pubclass [cn]" + enter;
            SendBufferstr += "MIME-Version: 1.0" + enter;
            if (Attachments.Count != 0)
            {
                SendBufferstr += "Content-Type: multipart/mixed;" + enter;
                SendBufferstr += "	boundary=\"=====" + (Html ? "001_Dragon520636771063_" : "001_Dragon303406132050_") + "=====\"" + enter + enter;
            }

            if (Html)
            {
                if (Attachments.Count == 0)
                {
                    SendBufferstr += "Content-Type: multipart/alternative;" + enter;//內容格式和分隔符
                    SendBufferstr += "	boundary=\"=====003_Dragon520636771063_=====\"" + enter + enter;

                    SendBufferstr += "This is a multi-part message in MIME format." + enter + enter;
                }
                else
                {
                    SendBufferstr += "This is a multi-part message in MIME format." + enter + enter;
                    SendBufferstr += "--=====001_Dragon520636771063_=====" + enter;
                    SendBufferstr += "Content-Type: multipart/alternative;" + enter;//內容格式和分隔符
                    SendBufferstr += "	boundary=\"=====003_Dragon520636771063_=====\"" + enter + enter;
                }
                SendBufferstr += "--=====003_Dragon520636771063_=====" + enter;
                SendBufferstr += "Content-Type: text/plain;" + enter;
                SendBufferstr += ((Charset == "") ? ("	charset=\"iso-8859-1\"") : ("	charset=\"" + Charset.ToLower() + "\"")) + enter;
                SendBufferstr += "Content-Transfer-Encoding: base64" + enter + enter;
                SendBufferstr += Base64Encode("郵件內容為HTML格式,請選擇HTML方式查看") + enter + enter;

                SendBufferstr += "--=====003_Dragon520636771063_=====" + enter;



                SendBufferstr += "Content-Type: text/html;" + enter;
                SendBufferstr += ((Charset == "") ? ("	charset=\"iso-8859-1\"") : ("	charset=\"" + Charset.ToLower() + "\"")) + enter;
                SendBufferstr += "Content-Transfer-Encoding: base64" + enter + enter;
                SendBufferstr += Base64Encode(Body) + enter + enter;
                SendBufferstr += "--=====003_Dragon520636771063_=====--" + enter;
            }
            else
            {
                if (Attachments.Count != 0)
                {
                    SendBufferstr += "--=====001_Dragon303406132050_=====" + enter;
                }
                SendBufferstr += "Content-Type: text/plain;" + enter;
                SendBufferstr += ((Charset == "") ? ("	charset=\"iso-8859-1\"") : ("	charset=\"" + Charset.ToLower() + "\"")) + enter;
                SendBufferstr += "Content-Transfer-Encoding: base64" + enter + enter;
                SendBufferstr += Base64Encode(Body) + enter;
            }

            //SendBufferstr += "Content-Transfer-Encoding: base64"+enter;




            if (Attachments.Count != 0)
            {
                for (int i = 0; i < Attachments.Count; i++)
                {
                    string filepath = (string)Attachments[i];
                    SendBufferstr += "--=====" + (Html ? "001_Dragon520636771063_" : "001_Dragon303406132050_") + "=====" + enter;
                    //SendBufferstr += "Content-Type: application/octet-stream"+enter;
                    SendBufferstr += "Content-Type: text/plain;" + enter;
                    SendBufferstr += "	name=\"=?" + Charset.ToUpper() + "?B?" + Base64Encode(filepath.Substring(filepath.LastIndexOf("\\") + 1)) + "?=\"" + enter;
                    SendBufferstr += "Content-Transfer-Encoding: base64" + enter;
                    SendBufferstr += "Content-Disposition: attachment;" + enter;
                    SendBufferstr += "	filename=\"=?" + Charset.ToUpper() + "?B?" + Base64Encode(filepath.Substring(filepath.LastIndexOf("\\") + 1)) + "?=\"" + enter + enter;
                    SendBufferstr += GetStream(filepath) + enter + enter;
                }
                SendBufferstr += "--=====" + (Html ? "001_Dragon520636771063_" : "001_Dragon303406132050_") + "=====--" + enter + enter;
            }



            SendBufferstr += enter + "." + enter;

            if (!Dialog(SendBufferstr, "錯誤信件信息"))
                return false;


            SendBufferstr = "QUIT" + enter;
            if (!Dialog(SendBufferstr, "斷開連接時錯誤"))
                return false;


            ns.Close();
            tc.Close();
            return true;
        }


        #endregion

       

        
    }
    #endregion

}