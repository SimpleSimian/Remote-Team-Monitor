using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

using System.IO;
using System.Threading;
using System.Drawing.Imaging;

namespace Remote_Team_Monitor
{
    public partial class Form1 : Form
    {
        public static string SessionKey = "";
        string username = "";
        string password = "";

        //web pages to validate and receive captures
        string validating_URL = "";
        static string screenCapture_URL = "";

        public Form1()
        {
            InitializeComponent();
        }
        /*
         * Remote worker begins a session
         */ 
        private void BtnBegin_Click(object sender, EventArgs e)
        {
            if (txtUsername.Text.Trim() == "")
            {
                errUsername.SetError(txtUsername, "Field cannot be blank");
                return;
            }
            if (txtPassword.Text.Trim() == "")
            {
                errPassword.SetError(txtUsername, "Field cannot be blank");
                return;
            }
            StartLoginThread(txtUsername.Text, txtPassword.Text);
        }

        public Thread StartLoginThread(String param1, String param2)
        {
            var t = new Thread(() => AttemptLogin(param1, param2));
            t.Start();
            return t;
        }

        private void AttemptLogin(String username, String password)
        {
            try
            {
                string postString = "username=" + username + "password=" + password;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(validating_URL + postString);

                byte[] contentArray = Encoding.UTF8.GetBytes(postString);

                request.Method = "POST";
                request.ContentLength = contentArray.Length;
                request.Credentials = CredentialCache.DefaultCredentials;
                request.ContentType = "application/x-www-form-urlencoded";

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(contentArray, 0, contentArray.Length);
                requestStream.Flush();
                requestStream.Close();


                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream receiveStream = response.GetResponseStream();

                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

                while (!readStream.EndOfStream)
                {
                    //Read the response and get any variable you may have asked the server to send
                    //e.g. A session key to identify logs sent
                    //SessionKey = readStream.ReadLine();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Call this after login with custom message to let the worker know that the session is starting
        void ShowTrayNotification(string Message)
        {
            NotificationPopup popUp = new NotificationPopup(Message);
            popUp.Show();

        }

        //Actual screen capture
        private void CaptureScreen()
        {

            Rectangle bounds = Screen.GetBounds(Point.Empty);
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }

                MemoryStream ms = new MemoryStream();

                bitmap.Save(ms, ImageFormat.Jpeg);

                StartSendingThread(ms.ToArray());
            }
            

        }
        public Thread StartSendingThread(byte[] param1)
        {
            var t = new Thread(() => SendViaHTTP(param1));
            t.Start();
            return t;
        }

        public static void SendViaHTTP(byte[] binarydata)
        {
            try
            {

                string poststring = screenCapture_URL + "?session=" + SessionKey;


                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(poststring);

                req.Method = "POST";
                req.ContentLength = binarydata.Length;
                req.AllowWriteStreamBuffering = false;

                Stream reqStream = req.GetRequestStream();

               
                reqStream.Write(binarydata, 0, (int)binarydata.Length);
                
                req.GetResponse();
 
                reqStream.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }
        //Set interval to time you want between sending
        private void CountDownToSend_Tick(object sender, EventArgs e)
        {
            CaptureScreen();
        }
    }
}
