using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Test.Models
{
    public class AntiSpam
    {
        public string[] SpamWords = { "seo", "сео", "website", "website`s", "websites", "domain", "продвижение сайтов", "продвижение сайта", "создание сайта", "создание сайтов" };
        public bool CheckSpam(string subject, string message)
        {
            foreach (var word in SpamWords)
            {
                if (subject.Contains(word) || message.Contains(word))
                    return true;
               
            }
            return false; 
        }

    }
    public class EmailService
    {
        public void SendMail(string email, string content, string subject, string name)
        {//elenatimofeeva10@yandex.ru
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress(name, $"{email}.com"));
            message.To.Add(new MailboxAddress("hitbat@inbox.ru"));
            message.Subject = "Письмо с контактной формы с блога педагога Елены Тимофеевой";
            var html = $"<div>Пришло новое сообщение!<div>От: {name}</div><div>Почта отправителя: <a href='mailto:{email}'>{email}</a></div><div><p>Тема:</p> <p>{subject}</p><p>Сообщение:</p><p>{content}</p></div></div>";
            message.Body = new BodyBuilder() { HtmlBody = html }.ToMessageBody();
            using (MailKit.Net.Smtp.SmtpClient smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                smtp.Connect("wpl44.hosting.reg.ru", 465, true);
                smtp.Authenticate("blogelenatimofeeva@elenatimofeeva.ru", "%vj8s4A5");
                smtp.Send(message);
                smtp.Disconnect(true);
            }
        }
       
    }
}
