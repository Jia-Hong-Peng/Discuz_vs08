using System;
using System.Reflection;
using System.Text;
using System.Data;
using System.Threading;

using Discuz.Common;
using Discuz.Data;
using Discuz.Plugin.Mail;
using Discuz.Config;

namespace Discuz.Forum
{
    /// <summary>
    /// Discuz!NT郵件發送類的調用封裝類
    /// </summary>
    public class Emails
    {
        protected static GeneralConfigInfo configinfo = GeneralConfigs.GetConfig();

        protected static ISmtpMail ESM;

        protected static EmailConfigInfo emailinfo = EmailConfigs.GetConfig();

        static Emails()
        {
            if (emailinfo.DllFileName.ToLower().IndexOf(".dll") <= 0)
                emailinfo.DllFileName = emailinfo.DllFileName + ".dll";

            LoadEmailPlugin();
        }

        //重設置當前郵件發送類的實例對像
        public static void ReSetISmtpMail()
        {
            emailinfo = EmailConfigs.GetConfig();

            LoadEmailPlugin();
        }

        /// <summary>
        /// 加載email插件
        /// </summary>
        private static void LoadEmailPlugin()
        {
            try
            {
                //讀取相應的DLL信息
                Assembly asm = Assembly.LoadFrom(System.Web.HttpRuntime.BinDirectory + emailinfo.DllFileName);
                ESM = (ISmtpMail)Activator.CreateInstance(asm.GetType(emailinfo.PluginNameSpace, false, true));
            }
            catch
            {
                try
                {
                    //讀取相應的DLL信息
                    Assembly asm = Assembly.LoadFrom(Utils.GetMapPath("/bin/" + emailinfo.DllFileName));
                    ESM = (ISmtpMail)Activator.CreateInstance(asm.GetType(emailinfo.PluginNameSpace, false, true));
                }
                catch
                {
                    ESM = new SmtpMail();
                }
            }
        }

        /// <summary>
        /// 定義郵件內容函數
        /// </summary>
        /// <param name="UserName">用戶名</param>
        /// <param name="Email">EMAIL地址</param>
        /// <param name="pass">相應註冊用戶的密碼(暫時無設置)</param>
        /// <returns></returns>
        public static bool DiscuzSmtpMail(string UserName, string Email, string pass)
        {
            string forumurl = "http://" + DNTRequest.GetCurrentFullHost() + BaseConfigs.GetForumPath.ToLower();

            try
            {
                ESM.RecipientName = UserName;//設定收件人姓名
                ESM.AddRecipient(Email);//設定收件人地址(必須填寫)。
                ESM.MailDomainPort = emailinfo.Port;
                ESM.From = emailinfo.Sysemail;
                ESM.FromName = configinfo.Webtitle;
                ESM.Html = true;
                ESM.Subject = "已成功創建你的 " + configinfo.Forumtitle + "帳戶,請查收.";

                StringBuilder body = new StringBuilder();
                body.Append(emailinfo.Emailcontent.Replace("{webtitle}", configinfo.Webtitle));
                body.Replace("{weburl}", string.Format("<a href=\"{0}\">{0}</a>", configinfo.Weburl));
                body.Replace("{forumurl}", string.Format("<a href=\"{0}\">{0}</a>", forumurl));
                body.Replace("{forumtitle}", configinfo.Forumtitle);

                ESM.Body = "<pre style=\"width:100%;word-wrap:break-word\">" + body.ToString() + "</pre>";
                ESM.MailDomain = emailinfo.Smtp;
                ESM.MailServerUserName = emailinfo.Username;
                ESM.MailServerPassWord = emailinfo.Password;

                //開始發送
                return ESM.Send();
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 定義郵件內容函數
        /// </summary>
        /// <param name="UserName">用戶名</param>
        /// <param name="Email">EMAIL地址</param>
        /// <param name="pass">相應註冊用戶的密碼(暫時無設置)</param>
        /// <param name="authstr">相應註冊用戶的激活鏈接串參數</param>
        /// <returns></returns>
        public static bool DiscuzSmtpMail(string UserName, string Email, string pass, string authstr)
        {
            string forumurl = "http://" + DNTRequest.GetCurrentFullHost() + BaseConfigs.GetForumPath.ToLower();

            try
            {
                ESM.RecipientName = UserName;//設定收件人姓名
                ESM.AddRecipient(Email);
                ESM.MailDomainPort = emailinfo.Port;
                ESM.From = emailinfo.Sysemail;
                ESM.FromName = configinfo.Webtitle;
                ESM.Html = true;
                ESM.Subject = "已成功創建你的 " + configinfo.Forumtitle + "帳戶,請查收.";

                StringBuilder body = new StringBuilder();
                body.Append(emailinfo.Emailcontent.Replace("{webtitle}", configinfo.Webtitle));
                body.Replace("{weburl}", string.Format("<a href=\"{0}\">{0}</a>", configinfo.Weburl));
                body.Replace("{forumurl}", string.Format("<a href=\"{0}\">{0}</a>", forumurl));
                body.Replace("{forumtitle}", configinfo.Forumtitle);

                ESM.Body = "<pre style=\"width:100%;word-wrap:break-word\">" + body.ToString() + "\r\n\r\n" + "激活您帳戶的鏈接為:<a href=" + forumurl + "activationuser.aspx?authstr=" + authstr.Trim() + "  target=_blank>" + forumurl + "activationuser.aspx?authstr=" + authstr.Trim() + "</a></pre>";
                ESM.MailDomain = emailinfo.Smtp;
                ESM.MailServerUserName = emailinfo.Username;
                ESM.MailServerPassWord = emailinfo.Password;

                return ESM.Send();
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 郵件通知服務
        /// </summary>
        /// <param name="Email">email地址</param>
        /// <param name="title">郵件的標題</param>
        /// <param name="body">郵件內容</param>
        /// <returns></returns>
        public static bool SendEmailNotify(string Email, string title, string body)
        {
            try
            {
                ESM.AddRecipient(Email);
                ESM.MailDomainPort = emailinfo.Port;
                ESM.From = emailinfo.Sysemail;//設定發件人地址(必須填寫)
                ESM.FromName = configinfo.Webtitle;
                ESM.Html = true;//設定正文是否HTML格式。
                ESM.Subject = title;

                ESM.Body = "<pre style=\"width:100%;word-wrap:break-word\">" + body.ToString() + "</pre>";
                ESM.MailDomain = emailinfo.Smtp;
                ESM.MailServerUserName = emailinfo.Username;
                ESM.MailServerPassWord = emailinfo.Password;

                //開始發送
                return ESM.Send();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 向指定用戶發送email，每次最多能給100人發郵件
        /// </summary>
        /// <param name="uidlist">用戶ID列表</param>
        /// <param name="subject">主題</param>
        /// <param name="body">內容</param>
        /// <returns></returns>
        public static string SendMailToUsers(string uidlist, string subject, string body)
        {
            DataTable dt = DatabaseProvider.GetInstance().GetMailTable(uidlist);
            if (dt == null || dt.Rows.Count < 1)
                return "";

            Thread[] lThreads = new Thread[dt.Rows.Count];

            int count = 0;
            int percount = 5;
            StringBuilder result = new StringBuilder();

            int successfulCount = 0;
            foreach (DataRow dr in dt.Rows)
            {
                if (successfulCount > 100)
                    break;

                EmailMultiThread emt = new EmailMultiThread(dr["username"].ToString().Trim(), dr["email"].ToString().Trim(), subject, body);
                lThreads[count] = new Thread(new ThreadStart(emt.Send));
                lThreads[count].Start();

                if (count >= percount)
                {
                    Thread.Sleep(5000);
                    count = 0;
                }
                result.Append(dr["uid"].ToString());
                result.Append(",");
                successfulCount++;
                count++;
            }
            return result.ToString().TrimEnd(',');
        }

        /// <summary>
        /// 用戶郵件發送
        /// </summary>
        /// <param name="Email">email地址</param>
        /// <param name="title">郵件的標題</param>
        /// <param name="body">郵件內容</param>
        /// <returns></returns>
        public static bool DiscuzSmtpMailToUser(string Email, string title, string body)
        {
            try
            {
                ESM.AddRecipient(Email);
                ESM.MailDomainPort = emailinfo.Port;
                ESM.From = emailinfo.Sysemail;//設定發件人地址(必須填寫)
                ESM.FromName = configinfo.Webtitle;
                ESM.Html = true;//設定正文是否HTML格式。
                ESM.Subject = title;
                ESM.Body = "<pre style=\"width:100%;word-wrap:break-word\">" + body.ToString() + "</pre>";
                ESM.MailDomain = emailinfo.Smtp;
                //也可將將SMTP信息一次設置完成。寫成
                ESM.MailServerUserName = emailinfo.Username;
                ESM.MailServerPassWord = emailinfo.Password;

                //開始發送
                return ESM.Send();
            }
            catch
            {
                return false;
            }
        }
    }


    /// <summary>
    /// 多線程發送郵箱類
    /// </summary>
    public class EmailMultiThread : Emails
    {
        #region 私有成員
        private string m_username = "";

        private string m_email = "";

        private string m_title = "";

        private string m_body = "";

        private bool m_issuccess = false;
        #endregion

        #region 公有屬性
        /// <summary>
        /// 收件人姓名
        /// </summary>
        public string UserName
        {
            get { return m_username; }
        }

        /// <summary>
        /// 收件人郵箱地址
        /// </summary>
        public string Email
        {
            get { return m_email; }
        }

        /// <summary>
        /// 郵件標題
        /// </summary>
        public string Title
        {
            get { return m_title; }
        }

        /// <summary>
        /// 郵件內容
        /// </summary>
        public string Body
        {
            get { return m_body; }
        }

        /// <summary>
        /// 是否發送成功
        /// </summary>
        public bool IsSuccess
        {
            get { return m_issuccess; }
            set { m_issuccess = value; }
        }
        #endregion

        public EmailMultiThread(string UserName, string Email, string Title, string Body)
        {
            m_username = UserName;
            m_email = Email;
            m_title = Title;
            m_body = Body;
        }

        public void Send()
        {
            lock (emailinfo)
            {
                try
                {
                    ESM.MailDomainPort = emailinfo.Port;
                    ESM.AddRecipient(this.Email);
                    ESM.RecipientName = this.UserName;//設定收件人姓名

                    ESM.From = emailinfo.Sysemail;
                    ESM.FromName = configinfo.Webtitle;
                    ESM.Html = true;
                    ESM.Subject = this.Title;
                    ESM.Body = "<pre style=\"width:100%;word-wrap:break-word\">" + this.Body.ToString() + "</pre>";
                    ESM.MailDomain = emailinfo.Smtp;
                    ESM.MailServerUserName = emailinfo.Username;
                    ESM.MailServerPassWord = emailinfo.Password;

                    //開始發送
                    this.IsSuccess = ESM.Send();
                }
                catch
                { }
            }
            Thread.CurrentThread.Abort();
        }
    }
}
