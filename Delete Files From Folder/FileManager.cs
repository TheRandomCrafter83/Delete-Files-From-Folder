using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace Delete_Files_From_Folder
{

    public class ErrorEventArgs : EventArgs
    {
        public String message { get; set; }
    }

    public class FileDeletedEventArgs: EventArgs
    {
        public string fileName { get; set; }
    }

    public class StatusUpdateEventArgs : EventArgs
    {
        public string message { get; set; }
    }


    class FileManager
    {

        public event EventHandler<ErrorEventArgs> Error;
        public event EventHandler<FileDeletedEventArgs> FileDeleted;
        public event EventHandler<StatusUpdateEventArgs> StatusUpdate;

        protected virtual void OnError(ErrorEventArgs e)
        {
            Error?.Invoke((object)this, (ErrorEventArgs)(EventArgs)e);
        }

        protected virtual void OnFileDeleted(FileDeletedEventArgs e)
        {
            FileDeleted?.Invoke((object)this,(FileDeletedEventArgs)(EventArgs)e);
        }

        protected virtual void OnStatusUpdate(StatusUpdateEventArgs e)
        {
            StatusUpdate?.Invoke((object)this,(StatusUpdateEventArgs)(EventArgs)e);
        }

        ErrorEventArgs eArgs = new ErrorEventArgs();
        FileDeletedEventArgs fArgs = new FileDeletedEventArgs();
        StatusUpdateEventArgs sArgs = new StatusUpdateEventArgs();
        

        public void deleteFilesInDirectory(String path)
        {

            new Thread(() => {
                Thread.CurrentThread.IsBackground = true;

                
                sArgs.message = "Checking for Directory";
                OnStatusUpdate(sArgs);
                if (!Directory.Exists(path)) {
                    eArgs.message = "Path [" + path + "] does not exist.";
                    OnError(eArgs);
                    return; 
                }

                FileInfo fileInfo = new FileInfo(path);
                String[] files = Directory.GetFiles(path);
                sArgs.message = "There are [" + files.Length.ToString() + "] to be deleted.";
                OnStatusUpdate(sArgs);
                foreach (var file in files)
                {
                    String filename = Path.GetFileNameWithoutExtension(file);
                    File.Delete(file);
                    if (File.Exists(file))
                    {
                        eArgs.message = "There was an error deleting file: " + filename;
                        OnError(eArgs);
                    }
                    else
                    {
                        fArgs.fileName = filename;
                        OnFileDeleted(fArgs);
                    }
                }
                sArgs.message = "All files have been deleted.";
                OnStatusUpdate(sArgs);
            }).Start();


        }

    }
}
