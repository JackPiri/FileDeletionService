using System;
using System.Collections.Generic;
using System.Linq;
using FileDeletionService;

namespace FileDeletionService
{
    internal class DiskType
    {
        #region Public Properties

        /// <summary>
        /// Disk name (with colon)
        /// </summary>
        public string DiskName { get; set; }

        /// <summary>
        /// Minimum free space required in the targeted disk (in GBs)
        /// </summary>
        public int MinimumFreeSpaceRequired { get; set; }

        /// <summary>
        /// Maximum used space (by the targeted folders/files) in the targeted disk (in GBs)
        /// </summary>
        public int MaximumUsedSpace { get; set; }

        /// <summary>
        /// List of folder types targetable for deletion
        /// </summary>
        public List<FolderType> FolderTypesList { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Disk type constructor
        /// </summary>
        /// <param name="diskName"> Disk name (with colon) </param>
        /// <param name="minimumFreeSpaceRequired"> Minimum free space required in the targeted disk (in GBs) </param>
        /// <param name="maximumUsedSpace"> Maximum used space (by the targeted folders/files) in the targeted disk (in GBs) </param>
        public DiskType(string diskName, int minimumFreeSpaceRequired, int maximumUsedSpace)
        {
            if (diskName.Contains(':') == false)
                diskName = diskName + ':';
            DiskName = diskName;
            MinimumFreeSpaceRequired = minimumFreeSpaceRequired;
            MaximumUsedSpace = maximumUsedSpace;
            FolderTypesList = new List<FolderType>();
        }

        /// <summary>
        /// Add a folder type to the folders targeted for deletion
        /// </summary>
        /// <param name="folderName"> Folder name </param>
        /// <param name="deleteFolderIfEmpty"> Folder deletion flag </param>
        /// <param name="deleteSubfoldersIfEmpty"> Subfolders deletion flag </param>
        /// <param name="firstSubfolderDeepnessToInspect"> First subfolder inspection deepness (starting from) </param>
        /// <param name="lastSubfolderDeepnessToInspect"> Last subfolder inspection deepness (finishing at) </param>
        /// <returns> Error code </returns>
        public FileDeletionServiceErrorCodes AddFolder(string folderName, bool deleteFolderIfEmpty = false, bool deleteSubfoldersIfEmpty = false, int firstSubfolderDeepnessToInspect = 0, int lastSubfolderDeepnessToInspect = 0)
        {
            FileDeletionServiceErrorCodes errorCode = FileDeletionServiceErrorCodes.NoError;

            if (FolderTypesList.Any(foldertype => foldertype.FolderName == folderName) == false)
                FolderTypesList.Add(new FolderType(folderName, deleteFolderIfEmpty, deleteSubfoldersIfEmpty, firstSubfolderDeepnessToInspect, lastSubfolderDeepnessToInspect));
            else
                errorCode = FileDeletionServiceErrorCodes.FolderTypeAlreadyPresent;

            return errorCode;
        }

        #endregion
    }
}