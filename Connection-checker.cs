using System;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.IO;

namespace CertCheck
{
    class Program
    {
        static int Main(string[] args)
        {
			
			var writepath = "c:\\temp\\output.txt";
		
            
				if (args.Length == 0){
				  GetProxy(writepath);
				  Console.WriteLine("Running Default google.com checks");
				  WriteFile(writepath, "Running Default google.com checks");
				  getHTTPResponse("http://google.com",writepath);
				  getCertThumbprint("https://google.com",writepath);
				  WriteFile(writepath, "completed");
				  Console.WriteLine("Usage: checkcert.exe <cert|http|proxy> <url> <output_file>");
				  
				  return 1;
				}
					
					if(args[0] == "cert"){
						getCertThumbprint(args[1], args[2]);
						
					}
					if(args[0] == "proxy"){
						GetProxy(args[2]);
						
					}
					if(args[0] == "http"){
						getHTTPResponse(args[1], args[2]);
					}
					else{
					 Console.WriteLine("error");
					}
				
			return 0;
        }

        public static void getCertThumbprint(string url, string path)
        {
            WriteFile(path, "start cert");
			try{
			var request = (HttpWebRequest)WebRequest.Create(url);
            request.ServerCertificateValidationCallback += (sender, validatedCert, chain, errors) => {
                var cert = new X509Certificate2(validatedCert);
                foreach (var chainElement in chain.ChainElements)
                {
                    var cn = chainElement.Certificate.Subject.Split(',').First().Replace("CN=", "");
                   // Console.WriteLine("Cert Name: {0}", cn);
				   WriteFile(path,cn);
                    //Console.WriteLine("Thumbprint: {0}\n", chainElement.Certificate.Thumbprint);
					WriteFile(path,chainElement.Certificate.Thumbprint);
                }
                return errors == SslPolicyErrors.None;
            };
			WriteFile(path, "end cert");
            request.GetResponse();
			}catch(Exception ex){
				WriteFile(path,ex.ToString());
			}
         }
    
	
	        public static void getHTTPResponse(string url, string path){
		    WriteFile(path, "start http");
			
			string proxy = "http://proxy.example.com:8080";
		
			var proxyObject = new WebProxy(proxy);
           

			try
            {
			//ServicePointManager.CertificatePolicy = delegate { return true; };
			ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create (url);
			request.Proxy = proxyObject;

            // Set some reasonable limits on resources used by this request
            request.MaximumAutomaticRedirections = 4;
            request.MaximumResponseHeadersLength = 4;
            // Set credentials to use for this request.
			NetworkCredential myCredentials = new NetworkCredential("usernamebad","passwdbad");
			myCredentials.Domain = "baddomain";
			request.Credentials = myCredentials;
            //request.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse ();

            //Console.WriteLine ("Content length is {0}", response.ContentLength);
			WriteFile(path,response.ContentLength.ToString());
           // Console.WriteLine ("Content type is {0}", response.ContentType);
			WriteFile(path,response.ContentType.ToString());

            // Get the stream associated with the response.
            Stream receiveStream = response.GetResponseStream ();

            // Pipes the stream to a higher level stream reader with the required encoding format.
            StreamReader readStream = new StreamReader (receiveStream, Encoding.UTF8);

            //Console.WriteLine (readStream.ReadToEnd ());
			WriteFile(path,readStream.ReadToEnd ());
            response.Close ();
            readStream.Close ();
			}catch(Exception ex){
				WriteFile(path,ex.ToString());
			}
			WriteFile(path, "end http");
    }
	public static void WriteFile(string fullPath, string data){
		
		using (StreamWriter writer = new StreamWriter(fullPath,append: true))  
		{  
		   Console.WriteLine (data);
		   writer.WriteLine(data);  
		 
		}
		
	}
	
		public static void GetProxy(string path){
		// Create a new request to the mentioned URL.				
        HttpWebRequest myWebRequest = (HttpWebRequest)WebRequest.Create("http://www.google.com");
		IWebProxy proxy = myWebRequest.Proxy;
		if (proxy != null)
{
			Console.WriteLine("Proxy: {0}", proxy.GetProxy(myWebRequest.RequestUri));
			WriteFile(path,proxy.GetProxy(myWebRequest.RequestUri).ToString());
			
		}
		else
		{
			Console.WriteLine("Proxy is null; no proxy will be used");
			
            WriteFile(path,"Proxy is null; no proxy will be used");
		}
	


		
	}
  
 }
}
