using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
namespace WindowsFormsAppDownload
{

    //https://stackoverflow.com/questions/35056500/download-file-from-google-drive-using-c-sharp-without-google-api
    //https://drive.google.com/uc?export=download&id=1DoW7gWRENwNUYpd7vdMwsmejy7-rnbSg
    //https://gist.github.com/dhlavaty/6121814
    public partial class btnRead : Form
    {

        string sheetUrl = "https://docs.google.com/spreadsheet/ccc?key=1a-QbKiQdqEh0CeyobRhk04mn79d2t450ZSM6DbzcFiE&usp=sharing&output=csv";
        string fileName = @"D:\setting.txt";
        static string strKey = "Ovi";
        public btnRead()
        {
            InitializeComponent();
        }

        private void BtnDownload_Click(object sender, EventArgs e)
        {

            WebClientEx wc = new WebClientEx(new CookieContainer());
            wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:22.0) Gecko/20100101 Firefox/22.0");
            wc.Headers.Add("DNT", "1");
            wc.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            wc.Headers.Add("Accept-Encoding", "deflate");
            wc.Headers.Add("Accept-Language", "en-US,en;q=0.5");
            byte[] dt = wc.DownloadData(sheetUrl);
            //var outputCSVdata = System.Text.Encoding.UTF8.GetString(dt ?? new byte[] { });
            string dataString = Encoding.ASCII.GetString(dt);
            string hashString = Encrypt(dataString);
            System.IO.File.WriteAllText(fileName, hashString);
            MessageBox.Show("Download Complete");
        }


        private void Button1_Click(object sender, EventArgs e)
        {

            var readFile = Decrypt(File.ReadAllText(fileName));
            string[] lines = readFile.Split(
                                    new[] { Environment.NewLine },
                                    StringSplitOptions.None
                                );
            bool IsActiveLicense = false;
            foreach (var item in lines.Skip(1))
            {
                var array = item.Split(',');
                var startDate = Convert.ToDateTime(array[0]);
                var endDate = Convert.ToDateTime(array[1]);
                if ((startDate <= DateTime.UtcNow) && (DateTime.UtcNow <= endDate))
                {
                    IsActiveLicense = true;
                    break;
                }
            }
            MessageBox.Show(IsActiveLicense.ToString());
        }
        /// <summary>
        /// Encrypt the given string using the specified key.
        /// </summary>
        /// <param name="strToEncrypt">The string to be encrypted.</param>
        /// <param name="strKey">The encryption key.</param>
        /// <returns>The encrypted string.</returns>
        public static string Encrypt(string strToEncrypt)
        {

            try
            {
                TripleDESCryptoServiceProvider objDESCrypto =
                    new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();
                byte[] byteHash, byteBuff;
                string strTempKey = strKey;
                byteHash = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strTempKey));
                objHashMD5 = null;
                objDESCrypto.Key = byteHash;
                objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB
                byteBuff = ASCIIEncoding.ASCII.GetBytes(strToEncrypt);
                return Convert.ToBase64String(objDESCrypto.CreateEncryptor().
                    TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            }
            catch (Exception ex)
            {
                return "Wrong Input. " + ex.Message;
            }
        }

        /// <summary>
        /// Decrypt the given string using the specified key.
        /// </summary>
        /// <param name="strEncrypted">The string to be decrypted.</param>
        /// <param name="strKey">The decryption key.</param>
        /// <returns>The decrypted string.</returns>
        public static string Decrypt(string strEncrypted)
        {
            try
            {
                TripleDESCryptoServiceProvider objDESCrypto =
                    new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();
                byte[] byteHash, byteBuff;
                string strTempKey = strKey;
                byteHash = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strTempKey));
                objHashMD5 = null;
                objDESCrypto.Key = byteHash;
                objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB
                byteBuff = Convert.FromBase64String(strEncrypted);
                string strDecrypted = ASCIIEncoding.ASCII.GetString
                (objDESCrypto.CreateDecryptor().TransformFinalBlock
                (byteBuff, 0, byteBuff.Length));
                objDESCrypto = null;
                return strDecrypted;
            }
            catch (Exception ex)
            {
                return "Wrong Input. " + ex.Message;
            }
        }
        public class WebClientEx : WebClient
        {
            public WebClientEx(CookieContainer container)
            {
                this.container = container;
            }

            private readonly CookieContainer container = new CookieContainer();

            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest r = base.GetWebRequest(address);
                var request = r as HttpWebRequest;
                if (request != null)
                {
                    request.CookieContainer = container;
                }
                return r;
            }

            protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
            {
                WebResponse response = base.GetWebResponse(request, result);
                ReadCookies(response);
                return response;
            }

            protected override WebResponse GetWebResponse(WebRequest request)
            {
                WebResponse response = base.GetWebResponse(request);
                ReadCookies(response);
                return response;
            }

            private void ReadCookies(WebResponse r)
            {
                var response = r as HttpWebResponse;
                if (response != null)
                {
                    CookieCollection cookies = response.Cookies;
                    container.Add(cookies);
                }
            }
        }

        private void ProgressBar1_Click(object sender, EventArgs e)
        {

        }
    }



}

