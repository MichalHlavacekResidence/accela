using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using sib_api_v3_sdk.Api;
//using sib_api_v3_sdk.Client;
//using sib_api_v3_sdk.Model;
using System.Diagnostics;
using System.Net.Mail;

namespace accela.Models
{
    public class Sendinblue
    {

        public SmtpClient CreateConection()
        {
            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
            client.Host = "smtp-relay.sendinblue.com";
            System.Net.NetworkCredential basicauthenticationinfo = new System.Net.NetworkCredential("web@residencev.com", "DGaCVZgU8wBs1mpx");
            client.Port = int.Parse("587");
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = basicauthenticationinfo;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            return client;





        }

        /* static void Main(string[] args)
         {
             Configuration.Default.ApiKey.Add("api-key", "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");

             var apiInstance = new EmailCampaignsApi();
             string type = "classic";
             string status = "sent";
             string startDate = "2020-08-22T09:43:51.970+05:30";
             string endDate = "2020-12-24T16:03:51.000+05:30";
             long? offset = 0;
             int limit = 500;
             try
             {
                 GetEmailCampaigns result = apiInstance.GetEmailCampaigns(type, status, startDate, endDate, limit, offset);
                 Debug.WriteLine(result.ToJson());
                 Console.WriteLine(result.ToJson());
                 Console.ReadLine();
             }
             catch (Exception e)
             {
                 Debug.WriteLine(e.Message);
                 Console.WriteLine(e.Message);
                 Console.ReadLine();
             }
         }*/
    }
   
}
