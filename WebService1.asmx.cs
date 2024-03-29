using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;


namespace WebRole1
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [ScriptService]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {


        public static Trie trie = new Trie();
        private PerformanceCounter memProcess = new PerformanceCounter("Memory", "Available MBytes");


        [WebMethod]
        public void DownloadFileFromBlobToDisk()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["hw2"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("wikititles");
            if (container.Exists())
            {
                foreach (IListBlobItem item in container.ListBlobs(null, false)) //if have multiple files
                {
                    if (item.GetType() == typeof(CloudBlockBlob))
                    {
                        CloudBlockBlob blob = (CloudBlockBlob)item;
                        String path = HostingEnvironment.ApplicationPhysicalPath + "\\WikiTitlesDownloadedFromBlob.txt";
                        using (FileStream fs = new FileStream(path, FileMode.Create))
                        {
                            blob.DownloadToStream(fs);
                        }
                        readFile(path);
                    }
                }
            }
        }


        //reads wikiTitles.txt line by line
        //adds it into trie
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod]
        public void readFile(String path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                int counter = 0;
                float am = memProcess.NextValue(); //assume 1GB memory
                float reserveMemory = am - 950;
                while (sr.EndOfStream == false)
                {
                    if (counter >= 1000)
                    {
                        if (memProcess.NextValue() < reserveMemory)
                        {
                            sr.Close();
                            return;
                        }
                        counter = 0;
                    }
                    string title = sr.ReadLine();
                    trie.AddTitle(title);
                }
                sr.Close();
            }
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public String[] search(String s)
        {
            return trie.SearchForPrefix(s).ToArray();
        }


    }
}
