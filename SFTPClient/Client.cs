using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinSCP;
using log4net;
using System.IO;

namespace SFTPClient
{
    public class Client
    {
        private ILog Logger;       
        public Client()
        {
            Logger = log4net.LogManager.GetLogger("FTPClient");
            
        }


        public int Execute()
        {
            try
            {
                // Setup session options
                String hostname = Params.sftpauth.Split('@')[1];
                String username = Params.sftpauth.Split(':')[0];
                String password = Params.sftpauth.Split('@')[0].Split(':')[1];

                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = hostname,
                    UserName = username,
                    Password = password,
                    SshHostKeyFingerprint = Params.fingerprint
                };

                //Console.WriteLine("Host {0} user {1} pw {2}", hostname, username, password);

                using (Session session = new Session())
                {                    
                    session.Open(sessionOptions);
                    
                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = TransferMode.Binary;
                   
                    
                    if (Params.RecordInitialFileList.Equals("true", StringComparison.InvariantCultureIgnoreCase)) 
                    {
                        RecordInitialFileList(Params.DownloadedFiles +"_" +  System.Guid.NewGuid().ToString() + ".txt", ListFiles(session, Params.RemoteDir));
                        session.Close();
                        return 0;
                    }



                    List<FileObject> newfiles = GetNewFileNames(ReadLocalFileNames(Params.DownloadedFiles), ListFiles(session, Params.RemoteDir));
                    foreach (FileObject item in newfiles)
                    {
                        DownLoadFile(session, transferOptions, Params.RemoteDir + item.Name, Params.LocalDir);
                        AppendToFile(Params.DownloadedFiles, item.Name);
                    }

                    session.Close();
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                Logger.Error(e.Message);
                throw e;
                
            }

        }

        private static void DownLoadFile(Session session, TransferOptions transferOptions, String remotefile, string localfolder)
        {
            TransferOperationResult transferResult;
            transferResult = session.GetFiles(remotefile, localfolder, false, transferOptions);
            
            transferResult.Check();
           
            foreach (TransferEventArgs transfer in transferResult.Transfers)
            {
                Console.WriteLine("Download of {0} succeeded", transfer.FileName);
            }
        }

        private static void UplaodLoadFile(Session session, TransferOptions transferOptions, String localfile, String remotefolder)
        {
            TransferOperationResult transferResult;
            transferResult = session.PutFiles(localfile, remotefolder, false, transferOptions);
            
            transferResult.Check();
                        
            foreach (TransferEventArgs transfer in transferResult.Transfers)
            {
                Console.WriteLine("Upload of {0} succeeded", transfer.FileName);
            }
        }

        private static List<FileObject> ListFiles(Session session, String remotefolder)
        {

            RemoteDirectoryInfo dirinfo = session.ListDirectory(remotefolder);
            
            List<FileObject> filelist = new List<FileObject>();

            foreach (RemoteFileInfo fileInfo in dirinfo.Files)
            {
                if (fileInfo.Name.Equals(".") || fileInfo.Name.Equals("..")) { continue; }
                if (Params.Filter.Length > 2 && fileInfo.Name.Contains(Params.Filter))
                {
                    filelist.Add(new FileObject(fileInfo.Name, fileInfo.Length, fileInfo.LastWriteTime));
                }
                
                //Console.WriteLine("{0} with size {1}, permissions {2} and last modification at {3}",
                //    fileInfo.Name, fileInfo.Length, fileInfo.FilePermissions, fileInfo.LastWriteTime);
            }

            return filelist;
            
        }


        public List<String> ReadLocalFileNames(String filepath)
        {
            List<String> filelist = new List<string>();
            using (StreamReader sr = new StreamReader(filepath))
            {
                String line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    filelist.Add(line);
                }
            }
            return filelist;
        }

        public void AppendToFile(String filepath, String data)
        {            
            using (StreamWriter sw = new StreamWriter(filepath, true))
            {
                sw.WriteLine(data);       
            }            
        }

        public List<FileObject> GetNewFileNames(List<String> localfilelist, List<FileObject> remotefilelist)
        {
            List<FileObject> newlist = new List<FileObject>();
            foreach (FileObject item in remotefilelist)
            {
                if (!localfilelist.Contains(item.Name))
                {
                    newlist.Add(new FileObject(item.Name, item.Size, item.LastModified));                    
                }
            }
            return newlist;
        }


        public void RecordInitialFileList(String filepath, List<FileObject> remotefilelist)
        {
            using (StreamWriter sw = new StreamWriter(filepath))
            {
                foreach (FileObject item in remotefilelist)
                {
                    sw.WriteLine(item.Name);
                }
               
            }
        }
    
    }
}
