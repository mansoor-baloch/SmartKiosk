using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Formatting;
using Newtonsoft.Json;
using System.Collections;
using System.Net;
using System.IO;

namespace SmartKioskApp
{
    public static class ReadWrite
    {
        public const string URL = "https://apibaflnotification.vendingc.com/api/Notification/Find_Notification";
        public const string url = "http://203.135.63.93/api/KioskMangement/PostingOrderSummary";

        //public static string urlParameters = "?QR_Gen_Time=2022-03-29 &PAN=533338625094994";
        public static string urlParameters = "";
        public static decimal PaidAmount;
        public static void Write(string data, string type)
        {
            string connString = ConfigurationManager.AppSettings["LocalCon"].ToString();
            string spName = "SPNVReadWrite";

            using (SqlConnection con = new SqlConnection(connString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd = new SqlCommand(spName, con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    List<SqlParameter> prams = new List<SqlParameter>();
                    cmd.Parameters.Add(new SqlParameter("Data", data));
                    cmd.Parameters.Add(new SqlParameter("Type", type));
                    if (con.State == ConnectionState.Closed) con.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                }

            }
        }

        internal static void Write(string v, object p)
        {
            throw new NotImplementedException();
        }

        public static string Read(string type)
        {
            string connString = ConfigurationManager.AppSettings["LocalCon"].ToString();
            string spName = "SPNVReadWrite";

            using (SqlConnection con = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand();
                DataTable dt = new DataTable();
                cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                List<SqlParameter> prams = new List<SqlParameter>();
                cmd.Parameters.Add(new SqlParameter("Data", "R"));
                cmd.Parameters.Add(new SqlParameter("Type", type));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                    return dt.Rows[0].ItemArray[0].ToString();
                else
                    return "";
            }
        }
        
        public static async Task ReadQRAmountAPIAsync(string Date, string PAN)
        {
            DataTable responseObj = new DataTable();

            try
            {
                using (var client = new HttpClient())
                {
                    // Setting Base address.  
                    client.BaseAddress = new Uri(URL);

                    // Setting content type.  
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Initialization.  
                    HttpResponseMessage response = new HttpResponseMessage();

                    // HTTP GET
                    urlParameters = "?QR_Gen_Time=" + Date + " &PAN=" + PAN;
                    response = await client.GetAsync(urlParameters).ConfigureAwait(false);

                    // Verification  
                    if (response.IsSuccessStatusCode)
                    {

                        // Reading Response.  
                        string result = response.Content.ReadAsStringAsync().Result;
                        result = result.TrimStart('\"');
                        result = result.TrimEnd('\"');
                        result = result.Replace("\\", "");
                        
                        var obj = JsonConvert.DeserializeObject<myDataObject>(result);
                        List<myDataObject> list = new List<myDataObject>();
                        PaidAmount = obj.transactionAmount;
                    }
                }

            }
            catch (Exception ex)
            {

            }
            
        }

        public static void PostDataToLDB(string serialized)
        {
            try
            {
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "POST";

                httpRequest.Accept = "application/json";
                httpRequest.ContentType = "application/json";

                using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    streamWriter.Write(serialized);

                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {

            }


        }
    }
    public  class myDataObject
    {
        public string id ;
        public string merchantId ;
        public string stan ;
        public string p2M_ID;
        public string merchantPAN;
        public string ica;
        public string dataHash;
        public decimal transactionAmount;
        public string postingDateTime;
    }
}
    