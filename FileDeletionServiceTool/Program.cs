using FileDeletionService;

namespace FileDeletionServiceTool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int triggerTime = 24; // hours
            FileDeletionManager fileDeletionManager;


            //manager initialization
            string[] commandLineArgs = Environment.GetCommandLineArgs();
            foreach (string commandLineArg in commandLineArgs)
            {
                if (commandLineArg.ToUpper().Contains("TRIGGERTIME="))
                {
                    if (commandLineArg.Split('=').Length > 1)
                        int.TryParse(commandLineArg.Split('=')[1], out triggerTime);
                }
            }
            fileDeletionManager = new FileDeletionManager(triggerTime);


            //disks initialization
            foreach (string commandLineArg in commandLineArgs)
            {
                if (commandLineArg.ToUpper().Contains("DISK="))
                {
                    if (commandLineArg.Split('=').Length > 1)
                    {
                        string diskName = "";
                        int minimumFreeSpaceRequired = 0;
                        int maximumUsedSpace = 0;
                        if ((commandLineArg.Split('=')[1]).Split('|').Length > 0)
                            diskName = (commandLineArg.Split('=')[1]).Split('|')[0];
                        if ((commandLineArg.Split('=')[1]).Split('|').Length > 1)
                            int.TryParse((commandLineArg.Split('=')[1]).Split('|')[1], out minimumFreeSpaceRequired);
                        if ((commandLineArg.Split('=')[1]).Split('|').Length > 2)
                            int.TryParse((commandLineArg.Split('=')[1]).Split('|')[2], out maximumUsedSpace);

                        if (diskName != "")
                            fileDeletionManager.AddDisk(diskName, minimumFreeSpaceRequired, maximumUsedSpace);
                    }
                }
            }


            //folders initialization
            foreach (string commandLineArg in commandLineArgs)
            {
                if (commandLineArg.ToUpper().Contains("FOLDER="))
                {
                    if (commandLineArg.Split('=').Length > 1)
                    {
                        string diskName = "", folderName = "";
                        bool deleteFolderIfEmpty = false, deleteSubfoldersIfEmpty = false;
                        int firstSubfoldersDeepnessToInspect = 0, lastSubfoldersDeepnessToInspect = 0;

                        if ((commandLineArg.Split('=')[1]).Split('|').Length > 0)
                            diskName = (commandLineArg.Split('=')[1]).Split('|')[0];
                        if ((commandLineArg.Split('=')[1]).Split('|').Length > 1)
                            folderName = (commandLineArg.Split('=')[1]).Split('|')[1];
                        if ((commandLineArg.Split('=')[1]).Split('|').Length > 2)
                            bool.TryParse((commandLineArg.Split('=')[1]).Split('|')[2], out deleteFolderIfEmpty);
                        if ((commandLineArg.Split('=')[1]).Split('|').Length > 3)
                            bool.TryParse((commandLineArg.Split('=')[1]).Split('|')[3], out deleteSubfoldersIfEmpty);
                        if ((commandLineArg.Split('=')[1]).Split('|').Length > 4)
                            int.TryParse((commandLineArg.Split('=')[1]).Split('|')[4], out firstSubfoldersDeepnessToInspect);
                        if ((commandLineArg.Split('=')[1]).Split('|').Length > 5)
                            int.TryParse((commandLineArg.Split('=')[1]).Split('|')[5], out lastSubfoldersDeepnessToInspect);

                        if (diskName != "" && folderName != "")
                            fileDeletionManager.AddFolderToDisk(diskName, folderName, deleteFolderIfEmpty, deleteSubfoldersIfEmpty, firstSubfoldersDeepnessToInspect, lastSubfoldersDeepnessToInspect);
                    }
                }
            }


            //files initialization
            foreach (string commandLineArg in commandLineArgs)
            {
                if (commandLineArg.ToUpper().Contains("FILE="))
                {
                    if (commandLineArg.Split('=').Length > 1)
                    {
                        string diskName = "", folderName = "", fileExtension = "";
                        int fileMaxDuration = 7;

                        if ((commandLineArg.Split('=')[1]).Split('|').Length > 0)
                            diskName = (commandLineArg.Split('=')[1]).Split('|')[0];
                        if ((commandLineArg.Split('=')[1]).Split('|').Length > 1)
                            folderName = (commandLineArg.Split('=')[1]).Split('|')[1];
                        if ((commandLineArg.Split('=')[1]).Split('|').Length > 2)
                            fileExtension = (commandLineArg.Split('=')[1]).Split('|')[2];
                        if ((commandLineArg.Split('=')[1]).Split('|').Length > 3)
                            int.TryParse((commandLineArg.Split('=')[1]).Split('|')[3], out fileMaxDuration);

                        if (diskName != "" && folderName != "" && fileExtension != "")
                            fileDeletionManager.AddFileToFolderToDisk(diskName, folderName, fileExtension, fileMaxDuration);
                    }
                }
            }


            //deletion start
            fileDeletionManager.Start();
            Thread.Sleep(-1);
        }
    }
}
