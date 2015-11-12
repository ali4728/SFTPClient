
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Appender;
using log4net;
using System.Configuration;
using System.Collections.Specialized;
using System.IO;

//
namespace SFTPClient
{
    class Params
    {

        public static String sftpauth { get; set; }
        public static String fingerprint { get; set; }       
        public static String logFile { get; set; }
        public static String LocalDir;
        public static String RemoteDir;        
        public static String EmailRecepients;
        public static String DownloadedFiles;

        public static String Filter;
        public static String RecordInitialFileList;
        
        public static String cmd { get; set; }     


        public static void parseAppConfig()
        {
            var vCustomParameters = ConfigurationManager.GetSection("CustomParameters") as NameValueCollection;
            if (vCustomParameters != null)
            {
                foreach (var pKey in vCustomParameters.AllKeys)
                {
                    string pValue = vCustomParameters.GetValues(pKey).FirstOrDefault();
                    
                    if ("LocalDir".Equals(pKey)) { Params.LocalDir = pValue; }
                    else if ("RemoteDir".Equals(pKey)) { Params.RemoteDir = pValue; }                    
                    else if ("EmailRecepients".Equals(pKey)) { Params.EmailRecepients = pValue; }
                    else if ("logFile".Equals(pKey)) { Params.logFile = pValue; }
                    else if ("sftpauth".Equals(pKey)) { Params.sftpauth = pValue; }
                    else if ("fingerprint".Equals(pKey)) { Params.fingerprint = pValue; }
                    else if ("DownloadedFiles".Equals(pKey)) { Params.DownloadedFiles = pValue; }
                    else if ("Filter".Equals(pKey)) { Params.Filter = pValue; }
                    else if ("RecordInitialFileList".Equals(pKey)) { Params.RecordInitialFileList = pValue; }
                    

                }
            }

            if (!(Params.RecordInitialFileList.Equals("true", StringComparison.InvariantCultureIgnoreCase) || Params.RecordInitialFileList.Equals("false", StringComparison.InvariantCultureIgnoreCase)))
            {             
                throw new Exception("RecordInitialFileList can be either \"true\" or \"false\"");
            }
        }



        public static void ConfigLogging()
        {

            FileAppender fileAppender = new FileAppender();
            fileAppender.AppendToFile = true;
            fileAppender.LockingModel = new FileAppender.MinimalLock();
            fileAppender.File = Params.logFile;

            log4net.Layout.PatternLayout pl = new log4net.Layout.PatternLayout();
            //d:timestamp t:thread p:level c:Class name m:message n:new line
            //pl.ConversionPattern = "%d [%2%t] %-5p [%-10c] %m%n";
            pl.ConversionPattern = "%d %-5p [%-10c] %m%n";
            pl.ActivateOptions();

            fileAppender.Layout = pl;
            fileAppender.ActivateOptions();

            log4net.Config.BasicConfigurator.Configure(fileAppender);

        }
    }
}

