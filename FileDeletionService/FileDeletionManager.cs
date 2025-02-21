using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FileDeletionService;

namespace FileDeletionService
{
    /// <summary>
    /// File deletion manager
    /// </summary>
    public class FileDeletionManager
    {
        #region Private Fields

        /// <summary>
        /// Deletion trigger time (in hours)
        /// </summary>
        private int _triggerTime;

        /// <summary>
        /// List of disk types targetable for deletion
        /// </summary>
        private List<DiskType> _diskTypesList;

        /// <summary>
        /// File deletion task
        /// </summary>
        private Task<FileDeletionServiceErrorCodes> _fileDeletionTask;

        /// <summary>
        /// Cancellation token for file deletion task
        /// </summary>
        private CancellationTokenSource _tokenSource;

        #endregion

        #region Public Methods

        /// <summary>
        /// File deletion manager constructor
        /// </summary>
        /// <param name="triggerTime"> Deletion trigger time (in hours) </param>
        public FileDeletionManager(int triggerTime)
        {
            _triggerTime = triggerTime;
            _diskTypesList = new List<DiskType>();
        }

        /// <summary>
        /// Add a disk type to the disks targeted for deletion
        /// </summary>
        /// <param name="diskName"> Disk name (with colon) </param>
        /// <param name="minimumFreeSpaceRequired"> Minimum free space required in the disks targeted (in GBs) </param>
        /// <returns> Error code </returns>
        public FileDeletionServiceErrorCodes AddDisk(string diskName, int minimumFreeSpaceRequired = 0)
        {
            FileDeletionServiceErrorCodes errorCode = FileDeletionServiceErrorCodes.NoError;

            if (_diskTypesList.Any(disktype => disktype.DiskName == diskName) == false)
                _diskTypesList.Add(new DiskType(diskName, minimumFreeSpaceRequired));
            else
                errorCode = FileDeletionServiceErrorCodes.DiskTypeAlreadyPresent;

            return errorCode;
        }

        /// <summary>
        /// Add a folder type to the disk type
        /// </summary>
        /// <param name="diskName"> Disk name (with colon) </param>
        /// <param name="folderName"> Folder name </param>
        /// <param name="deleteFolderIfEmpty"> Folder deletion flag </param>
        /// <param name="deleteSubfoldersIfEmpty"> Subfolders deletion flag </param>
        /// <param name="firstSubfolderDeepnessToInspect"> First subfolder inspection deepness </param>
        /// <param name="lastSubfolderDeepnessToInspect"> Last subfolder inspection deepness </param>
        /// <returns> Error code </returns>
        public FileDeletionServiceErrorCodes AddFolderToDisk(string diskName, string folderName, bool deleteFolderIfEmpty = false, bool deleteSubfoldersIfEmpty = false, int firstSubfolderDeepnessToInspect = 0, int lastSubfolderDeepnessToInspect = 0)
        {
            FileDeletionServiceErrorCodes errorCode = FileDeletionServiceErrorCodes.NoError;

            if (_diskTypesList.Any(disktype => disktype.DiskName == diskName) == true)
                errorCode = _diskTypesList.First(disktype => disktype.DiskName == diskName).AddFolder(folderName, deleteFolderIfEmpty, deleteSubfoldersIfEmpty, firstSubfolderDeepnessToInspect, lastSubfolderDeepnessToInspect);
            else
                errorCode = FileDeletionServiceErrorCodes.DiskTypeMissing;

            return errorCode;
        }

        /// <summary>
        /// Add a file type to the folder type of the disk type
        /// </summary>
        /// <param name="diskName"> Disk name (with colon) </param>
        /// <param name="folderName"> Folder name </param>
        /// <param name="fileExtension"> File type extension (with dot) </param>
        /// <param name="fileMaxDuration"> File type max duration (in days), breaking this limit would trigger file deletion </param>
        /// <returns> Error code </returns>
        public FileDeletionServiceErrorCodes AddFileToFolderToDisk(string diskName, string folderName, string fileExtension, int fileMaxDuration)
        {
            FileDeletionServiceErrorCodes errorCode = FileDeletionServiceErrorCodes.NoError;

            if (_diskTypesList.Any(disktype => disktype.DiskName == diskName) == true)
            {
                if (_diskTypesList.First(disktype => disktype.DiskName == diskName).FolderTypesList.Any(foldertype => foldertype.FolderName == folderName) == true)
                {
                    errorCode = _diskTypesList.First(disktype => disktype.DiskName == diskName).FolderTypesList.First(foldertype => foldertype.FolderName == folderName).AddFile(fileExtension, fileMaxDuration);
                }
                else
                    errorCode = FileDeletionServiceErrorCodes.FolderTypeMissing;
            }
            else
                errorCode = FileDeletionServiceErrorCodes.DiskTypeMissing;

            return errorCode;
        }

        /// <summary>
        /// Start (restart) deletion
        /// </summary>
        /// <returns></returns>
        public FileDeletionServiceErrorCodes Start()
        {
            FileDeletionServiceErrorCodes errorCode = FileDeletionServiceErrorCodes.NoError;

            try
            {
                if (_fileDeletionTask == null || _fileDeletionTask.IsCompleted == true)
                {
                    _tokenSource = new CancellationTokenSource();
                    _fileDeletionTask = Task.Run(() => Delete(_tokenSource.Token), _tokenSource.Token);
                }
            }
            catch (Exception ex)
            {
                errorCode = FileDeletionServiceErrorCodes.StartingDeletion;
            }

            return errorCode;
        }

        /// <summary>
        /// Stop deletion
        /// </summary>
        /// <returns></returns>
        public FileDeletionServiceErrorCodes Stop()
        {
            FileDeletionServiceErrorCodes errorCode = FileDeletionServiceErrorCodes.NoError;

            try
            {
                _tokenSource.Cancel();
            }
            catch (Exception)
            {
                errorCode = FileDeletionServiceErrorCodes.StoppingDeletion;
            }

            return errorCode;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Check folders content for deletion
        /// </summary>
        /// <param name="token"> Cancellation token for stopping the executing task </param>
        /// <returns> Error code </returns>
        private FileDeletionServiceErrorCodes Delete(CancellationToken token)
        {
            FileDeletionServiceErrorCodes errorCode = FileDeletionServiceErrorCodes.NoError;

            while (token.IsCancellationRequested == false)
            {
                try
                {
                    //check based on folders searching for files older than their max duration
                    foreach (DiskType disktype in _diskTypesList)
                    {
                        foreach (FolderType foldertype in disktype.FolderTypesList)
                        {
                            if (Directory.Exists(foldertype.FolderName) == true)
                            {
                                string currentSubfolder = foldertype.FolderName;
                                int currentDeepness = 0;

                                errorCode = DeleteInsideFolderIterative(foldertype, currentSubfolder, currentDeepness);
                            }
                        }
                    }

                    //check based on folders granting for minimum free space required
                    foreach (DiskType disktype in _diskTypesList)
                    {
                        DriveInfo driveInfo = new DriveInfo(disktype.DiskName);
                        long minimumFreeSpaceRequiredInBytes = 0, freeSpaceInBytes = 0;
                        minimumFreeSpaceRequiredInBytes = ((long)disktype.MinimumFreeSpaceRequired * 1024 * 1024 * 1024);
                        freeSpaceInBytes = driveInfo.TotalFreeSpace;
                        if (freeSpaceInBytes < minimumFreeSpaceRequiredInBytes)
                        {
                            List<FileInfo> fileInfosList = new List<FileInfo>();
                            foreach (FolderType foldertype in disktype.FolderTypesList)
                            {
                                if (Directory.Exists(foldertype.FolderName) == true)
                                {
                                    string currentSubfolder = foldertype.FolderName;
                                    int currentDeepness = 0;

                                    errorCode = SearchInsideFolderIterative(foldertype, currentSubfolder, currentDeepness, fileInfosList);
                                }
                            }
                            fileInfosList.Sort((file1, file2) => file1.LastWriteTime.CompareTo(file2.LastWriteTime));
                            long totalBytesDeleted = 0;
                            for (int i = 0; i < fileInfosList.Count; i++)
                            {
                                if (totalBytesDeleted < minimumFreeSpaceRequiredInBytes - freeSpaceInBytes)
                                {
                                    totalBytesDeleted += fileInfosList[i].Length;
                                    fileInfosList[i].Delete();
                                }
                                else
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorCode = FileDeletionServiceErrorCodes.Deletion;
                }

                token.WaitHandle.WaitOne(_triggerTime * 60 * 60 * 1000);
            }

            return errorCode;
        }

        /// <summary>
        /// Delete iteratively inside folder
        /// </summary>
        /// <param name="folderType"> Folder type to target </param>
        /// <param name="currentFolder"> Current folder to delete into </param>
        /// <param name="currentDeepness"> Current folder deepness inspection </param>
        /// <returns> Error code </returns>
        private FileDeletionServiceErrorCodes DeleteInsideFolderIterative(FolderType folderType, string currentFolder, int currentDeepness)
        {
            FileDeletionServiceErrorCodes errorCode = FileDeletionServiceErrorCodes.NoError;

            string[] filesOfFolder = Directory.GetFiles(currentFolder);
            string[] subfoldersOfFolder;
            bool toBeDeleted = false;

            //delete target files in current folder
            if (currentDeepness >= folderType.FirstSubfolderDeepnessToInspect && currentDeepness <= folderType.LastSubfolderDeepnessToInspect)
            {
                foreach (string file in filesOfFolder)
                {
                    FileInfo fileInfo = new FileInfo(file);

                    foreach (FileType filetype in folderType.FileTypeList)
                    {
                        errorCode = filetype.IsFileToBeDeleted(fileInfo.FullName, ref toBeDeleted);
                        if (errorCode == FileDeletionServiceErrorCodes.FileNotExisting || toBeDeleted == true)
                            break;
                    }

                    if (toBeDeleted == true)
                        fileInfo.Delete();
                }
            }

            //inspect subfolders
            if (currentDeepness + 1 <= folderType.LastSubfolderDeepnessToInspect)
            {
                subfoldersOfFolder = Directory.GetDirectories(currentFolder);
                foreach (string subfolder in subfoldersOfFolder)
                {
                    DeleteInsideFolderIterative(folderType, subfolder, currentDeepness + 1);
                }
            }

            //delete current folder if empty
            toBeDeleted = false;
            if (currentDeepness == 0)
                errorCode = folderType.IsFolderToBeDeleted(currentFolder, ref toBeDeleted);
            else
                errorCode = folderType.IsFolderToBeDeleted(currentFolder, ref toBeDeleted, true);
            if (toBeDeleted == true)
                Directory.Delete(currentFolder);

            return errorCode;
        }

        /// <summary>
        /// Search iteratively inside folder
        /// </summary>
        /// <param name="folderType"> Folder type to target </param>
        /// <param name="currentFolder"> Current folder to delete into </param>
        /// <param name="currentDeepness"> Current folder deepness inspection </param>
        /// <param name="fileInfosList"> List of searched file infos </param>
        /// <returns> Error code </returns>
        private FileDeletionServiceErrorCodes SearchInsideFolderIterative(FolderType folderType, string currentFolder, int currentDeepness, List<FileInfo> fileInfosList)
        {
            FileDeletionServiceErrorCodes errorCode = FileDeletionServiceErrorCodes.NoError;

            string[] filesOfFolder = Directory.GetFiles(currentFolder);
            string[] subfoldersOfFolder;

            //search target files in current folder
            if (currentDeepness >= folderType.FirstSubfolderDeepnessToInspect &&
                currentDeepness <= folderType.LastSubfolderDeepnessToInspect)
            {
                foreach (string file in filesOfFolder)
                {
                    FileInfo fileInfo = new FileInfo(file);

                    foreach (FileType filetype in folderType.FileTypeList)
                    {
                        if (filetype.Extension == fileInfo.Extension)
                            fileInfosList.Add(fileInfo);
                    }
                }
            }

            //inspect subfolders
            if (currentDeepness + 1 <= folderType.LastSubfolderDeepnessToInspect)
            {
                subfoldersOfFolder = Directory.GetDirectories(currentFolder);
                foreach (string subfolder in subfoldersOfFolder)
                {
                    SearchInsideFolderIterative(folderType, subfolder, currentDeepness + 1, fileInfosList);
                }
            }

            return errorCode;
        }

        #endregion
    }
}