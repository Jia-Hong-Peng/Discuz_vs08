using System;
using System.Text;

namespace Discuz.Plugin.Mail
{

    #region 郵件收發代碼接口

    public interface ISmtpMail
    {
        /// <summary>
        /// 端口
        /// </summary>
        int MailDomainPort { set; }

        /// <summary>
        /// 發件人地址
        /// </summary>
        string From { set; get; }

        /// <summary>
        /// 發件人姓名
        /// </summary>
        string FromName { set; get; }

        /// <summary>
        /// 是否支持Html
        /// </summary>
        bool Html { set; get; }

        /// <summary>
        /// 郵件標題
        /// </summary>
        string Subject { set; get; }

        /// <summary>
        /// 郵件內容
        /// </summary>
        string Body { set; get; }

        /// <summary>
        ///  郵件服務器地址
        /// </summary>
        string MailDomain { set; }

        /// <summary>
        /// 用戶名
        /// </summary>
        string MailServerUserName { set; }

        /// <summary>
        /// 口令
        /// </summary>
        string MailServerPassWord { set; }

        /// <summary>
        /// 收件人姓名
        /// </summary>
        string RecipientName { set; get; }

        /// <summary>
        /// 收件人列表
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        bool AddRecipient(params string[] username);

        /// <summary>
        /// 發送
        /// </summary>
        /// <returns></returns>
        bool Send();

    }

    #endregion

}
