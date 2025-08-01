namespace FileDeletionService
{
    internal class FolderType
    {
        #region Public Properties

        /// <summary>
        /// Folder name
        /// </summary>
        public string FolderName { get; set; }

        /// <summary>
        /// Folder deletion flag
        /// </summary>
        public bool DeleteFolderIfEmpty { get; set; }

        /// <summary>
        /// Subfolders deletion flag
        /// </summary>
        public bool DeleteSubfoldersIfEmpty { get; set; }

        /// <summary>
        /// First subfolder inspection deepness (greater or equal 0, lower or equal LastSubfolderDeepnessToInspect)
        /// </summary>
        public int FirstSubfolderDeepnessToInspect { get; set; }

        /// <summary>
        /// Last subfolder inspection deepness (greater or equal 0, greater or equal FirstSubfolderDeepnessToInspect)
        /// </summary>
        public int LastSubfolderDeepnessToInspect { get; set; }

        /// <summary>
        /// List of file types targetable for deletion
        /// </summary>
        public List<FileType> FileTypeList { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Folder type constructor
        /// </summary>
        /// <param name="folderName"> Folder name </param>
        /// <param name="deleteFolderIfEmpty"> Folder deletion flag </param>
        /// <param name="deleteSubfoldersIfEmpty"> Subfolders deletion flag </param>
        /// <param name="firstSubfolderDeepnessToInspect"> First subfolder inspection deepness </param>
        /// <param name="lastSubfolderDeepnessToInspect"> Last subfolder inspection deepness </param>
        public FolderType(string folderName, bool deleteFolderIfEmpty = false, bool deleteSubfoldersIfEmpty = false, int firstSubfolderDeepnessToInspect = 0, int lastSubfolderDeepnessToInspect = 0)
        {
            FolderName = folderName;
            DeleteFolderIfEmpty = deleteFolderIfEmpty;
            DeleteSubfoldersIfEmpty = deleteSubfoldersIfEmpty;
            FirstSubfolderDeepnessToInspect = firstSubfolderDeepnessToInspect;
            LastSubfolderDeepnessToInspect = lastSubfolderDeepnessToInspect;
            if (FirstSubfolderDeepnessToInspect < 0 || LastSubfolderDeepnessToInspect < 0 || FirstSubfolderDeepnessToInspect > LastSubfolderDeepnessToInspect)
            {
                FirstSubfolderDeepnessToInspect = 0;
                LastSubfolderDeepnessToInspect = 0;
            }
            FileTypeList = new List<FileType>();
        }

        /// <summary>
        /// Add file type to the files targeted for deletion
        /// </summary>
        /// <param name="fileExtension"> File type extension (with dot) </param>
        /// <param name="fileMaxDuration"> File type max duration (in days), breaking this limit would trigger file deletion </param>
        /// <returns> Error code </returns>
        public FileDeletionServiceErrorCodes AddFile(string fileExtension, int fileMaxDuration)
        {
            FileDeletionServiceErrorCodes errorCode = FileDeletionServiceErrorCodes.NoError;

            if (fileExtension.Contains('.') == true)
                fileExtension = fileExtension.Remove(0, 1);

            if (FileTypeList.Any(filetype => filetype.Extension == fileExtension) == false)
                FileTypeList.Add(new FileType(fileExtension, fileMaxDuration));
            else
                errorCode = FileDeletionServiceErrorCodes.FileTypeAlreadyPresent;

            return errorCode;
        }

        /// <summary>
        /// Check if a folder should be deleted
        /// </summary>
        /// <param name="folderFullPath"> Folder full path </param>
        /// <param name="isFolderToBeDeleted"> Flag for deletion </param>
        /// <param name="isSubfolder"> Option for indicating the folder is a subfolder </param>
        /// <returns> Error code </returns>
        public FileDeletionServiceErrorCodes IsFolderToBeDeleted(string folderFullPath, ref bool isFolderToBeDeleted, bool isSubfolder = false)
        {
            FileDeletionServiceErrorCodes errorCode = FileDeletionServiceErrorCodes.NoError;

            isFolderToBeDeleted = false;

            if (Directory.Exists(folderFullPath))
            {
                string[] filesOfFolder = Directory.GetFiles(folderFullPath);
                string[] subfoldersOfFolder = Directory.GetDirectories(folderFullPath);
                if (filesOfFolder.Length == 0 && subfoldersOfFolder.Length == 0)
                {
                    if (isSubfolder == false && DeleteFolderIfEmpty == true)
                        isFolderToBeDeleted = true;
                    if (isSubfolder == true && DeleteSubfoldersIfEmpty == true)
                        isFolderToBeDeleted = true;
                }
            }
            else
                errorCode = FileDeletionServiceErrorCodes.FolderNotExisting;

            return errorCode;
        }

        #endregion
    }
}
