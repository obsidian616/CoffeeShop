using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace CoffeeShop.Models.Infra
{
    public class EmailHelper
    {
        // 寄件者，可以從 db 或 web.config 取得
        private string senderEmail = "g01.webapp@gmail.com";

        // 1個參數
        public void SendForgotPasswordEmail(string url, string name, string email)
        {
            var subject = "[重設密碼通知]";
            var body = $@"
                Hi {name},<br />
                請點擊此連結 <a href='{url}' target='_blank'>我要重設密碼</a>，以進行重設密碼，如果您沒有提出申請，請忽略本信，謝謝。
            ";

            var from = senderEmail;
            var to = email;

            SendFromGmail(from, to, subject, body);
        }

        public virtual void SendFromGmail(string from, string to, string subject, string body)
        {
            //todo以下是開發時,測試之用,只是建立text file,不真的寄出信
            var path = HttpContext.Current.Server.MapPath("~/files/");
            CreateTextFile(path, from, to, subject, body);
            return;
            //todo實作程式,可以視需要真的寄出信,或者只是單純建立text file,供開發時使用
        }

        private void CreateTextFile(string path, string from, string to, string subject, string body)
        {
            var fileName = $"{to.Replace("@", "_")} {DateTime.Now.ToString("yyyyMMdd_HHmmss")}.txt";
            var fullPath = Path.Combine(path, fileName);
            var contents = $@"from:{from}
to:{to}
subject:{subject}

{body}";

            File.WriteAllText(fullPath, contents, Encoding.UTF8);
        }
    }
}
