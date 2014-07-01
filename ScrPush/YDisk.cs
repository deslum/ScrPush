using System;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Xml;
using System.Configuration;

namespace YDisk
{
	public static class YDisk
	{
		public static HttpWebResponse Response;
		public static String urlToken = "https://oauth.yandex.ru/authorize?response_type=code&client_id=42795a09ef254d71b574eeb1a59f3610";
		static String token = String.Empty;

		public static void SetToken(String key){
			token = key;
		}

        public static String GetToken()
        {
            
            return token;
        }

		public static String Login(String login, String pass)
		{
			String key;
			HttpWebRequest httpWReq =
				(HttpWebRequest)WebRequest.Create("https://passport.yandex.ru/passport?mode=auth&retpath=https%3A//oauth.yandex.ru/authorize%3Fresponse_type%3Dcode%26client_id%3D42795a09ef254d71b574eeb1a59f3610");
			ASCIIEncoding encoding = new ASCIIEncoding();
			CookieContainer CookieContainer = new CookieContainer();
			httpWReq.CookieContainer = CookieContainer;

			string postData = "login="+login;
			postData += "&passwd="+pass;
			postData += "&twoweeks=yes";
			long ticks = DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks;
			ticks /= 10000;
			postData += "&timestamp="+ticks.ToString();
			byte[] data = encoding.GetBytes(postData);
			httpWReq.Method = "POST";
			httpWReq.ContentType = "application/x-www-form-urlencoded";
			httpWReq.ContentLength = data.Length;
			using (Stream stream = httpWReq.GetRequestStream())
			{
				stream.Write(data,0,data.Length);
			}
			HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
			key = response.ResponseUri.ToString ().Split ('=')[2];
			String sendStr = "grant_type=authorization_code&code=" + key +
				"&client_id=42795a09ef254d71b574eeb1a59f3610&client_secret=d2057cb59dfc41c7a819683da9c36568";
			WebRequest post = WebRequest.Create ("https://oauth.yandex.ru/token");
			post.Method = "POST";
			post.ContentType = "application/x-www-form-urlencoded";
			data = Encoding.GetEncoding (1251).GetBytes (sendStr);
			post.ContentLength = data.Length;
			Stream sendStream = post.GetRequestStream ();
			sendStream.Write (data, 0, data.Length);
			sendStream.Close ();
			WebResponse result = post.GetResponse ();
			Stream stream1 = result.GetResponseStream ();
			StreamReader sr = new StreamReader (stream1);
			string s = sr.ReadLine ();
			token = s.Substring (18, 32);
            return token;
		}


		public static bool Put(String nameFile)
		{
			String url = "https://webdav.yandex.ru/ScreenShots";
			HttpWebResponse Response;
			long fileLen = new FileInfo(nameFile).Length;
			url = url.TrimEnd(new char[] { '/' }) + @"/" + System.IO.Path.GetFileName(nameFile);
			HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create(url);
			Request.Method = WebRequestMethods.Http.Put;
			Request.Host = "webdav.yandex.ru";
			Request.Accept = "*/*";
			Request.Headers.Add("Authorization: OAuth "+token);
			Request.ServicePoint.Expect100Continue = true;
			Request.ContentType = "application/binary";
			Request.ContentLength = fileLen;
			Request.SendChunked = true;
			Request.KeepAlive = false;
			Request.Headers.Add(@"Overwrite", @"T");
			Request.AllowWriteStreamBuffering = true;
			System.IO.Stream stream = Request.GetRequestStream();
			FileStream fileStrem = new FileStream(nameFile, FileMode.Open, FileAccess.Read);
			int transferRate = 4096;
			byte[] data = new byte[transferRate];
			int read = 0;
			long totalRead = 0;
			try
			{
				do
				{
					read = fileStrem.Read(data, 0, data.Length);
					if (read > 0)
					{
						totalRead += read;
						stream.Write(data, 0, read);
					}
				} while (read > 0);
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				stream.Close();
				stream.Dispose();
				stream = null;

				fileStrem.Close();
				fileStrem.Dispose();
				fileStrem = null;
			}

			try
			{
				Response = (HttpWebResponse)Request.GetResponse();
			}
			catch (WebException e)
			{
				if (e.Response == null)
				{
					Console.WriteLine("Error accessing Url ");
					throw;
				}

				HttpWebResponse errorResponse = (HttpWebResponse)e.Response;
				if (errorResponse.StatusCode == HttpStatusCode.NotModified)
				{
					e.Response.Close();
					return false;
				}
				else
				{
					e.Response.Close();
					Console.WriteLine("Error accessing Url ");
					Console.ReadLine();
					throw;
				}
			}
			Response.Close();

			if (totalRead == fileLen)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	

	public static String getLink(String fileName)
	{
		try
		{
			string strBody = "<propertyupdate xmlns=\"DAV:\">" +
				"<set>" +
					"<prop>" +
					"<public_url xmlns=\"urn:yandex:disk:meta\">true</public_url>" +
					"</prop>" +
					"</set>" +
					"</propertyupdate>";
			byte[] bytes = null;
			WebResponse Response;
			HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create("https://webdav.yandex.ru/ScreenShots/"+fileName);
			Stream RequestStream;
			Request.Method = "PROPPATCH";
			Request.UserAgent = "ScrPush/0.0.1";
			Request.Host = "webdav.yandex.ru";
			Request.Headers.Add("Authorization: OAuth "+token);
			bytes = Encoding.UTF8.GetBytes((string)strBody);
			Request.ContentLength = bytes.Length;
			RequestStream = Request.GetRequestStream();
			RequestStream.Write(bytes, 0, bytes.Length);
			RequestStream.Close();
			Response = (HttpWebResponse)Request.GetResponse();
			StreamReader reader = new StreamReader(Response.GetResponseStream());
			String str = reader.ReadToEnd();
			XmlDocument xml = new XmlDocument();
			xml.LoadXml(str);
			XmlNodeList xnList = xml.SelectNodes("/");
			str = "http:"+xnList[0]["d:multistatus"].InnerText.Split(':')[1];
			return str;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return "error";
		}
	}


    public static String getSpace()
    {
        try
        {
            string strBody = "<D:propfind xmlns:D=\"DAV:\">"+
                    "<D:prop>"+
    "<D:quota-available-bytes/>"+
    "<D:quota-used-bytes/>"+
  "</D:prop>"+
"</D:propfind>";
            byte[] bytes = null;
            WebResponse Response;
            HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create("https://webdav.yandex.ru");
            Stream RequestStream;
            Request.Method = "PROPFIND";
            Request.Host = "webdav.yandex.ru";
            Request.Accept = "*/*";
            Request.Headers.Add("Depth: 0");
            Request.Headers.Add("Authorization: OAuth " + token);
            bytes = Encoding.UTF8.GetBytes((string)strBody);
            Request.ContentLength = bytes.Length;
            RequestStream = Request.GetRequestStream();
            RequestStream.Write(bytes, 0, bytes.Length);
            RequestStream.Close();
            Response = (HttpWebResponse)Request.GetResponse();
            StreamReader reader = new StreamReader(Response.GetResponseStream());
            String str = reader.ReadToEnd();
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(str);
            XmlNodeList xnList = xml.SelectNodes("//quota-available-bytes[contains]");
            str = "http:" + xnList[0].InnerText;
      
            return str;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return "error";
        }
    }

    public static String Login()
    {
        if (token != String.Empty)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://webdav.yandex.ru/?userinfo");
            request.Host = "webdav.yandex.ru";
            request.Accept = "*/*";
            request.Headers.Add("Authorization: OAuth " + token);
            Response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(Response.GetResponseStream());
            String str = reader.ReadToEnd();
            return str.Split(':')[1];
        }
        else
            return String.Empty;
         
    }

	public static void MkDir()
	{
		HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://webdav.yandex.ru/ScreenShots");
		request.Host = "webdav.yandex.ru";
		request.Method = "MKCOL";
		request.Accept = "*/*";
		request.Headers.Add("Authorization: OAuth "+token);
		request.GetResponse();

	}

	}
}
