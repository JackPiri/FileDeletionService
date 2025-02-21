using System;
using System.IO;
using System.Linq;
using FileDeletionService;

namespace FileDeletionService
{
    internal class FileType
    {
        #region Public Properties

        /// <summary>
        /// File type extension (with dot)
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// File type max duration (in days), breaking this limit would trigger file deletion
        /// </summary>
        public int MaxDuration { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// File type constructor
        /// </summary>
        /// <param name="extension"> File type extension (with dot) </param>
        /// <param name="maxDuration"> File type max duration (in days), breaking this limit would trigger file deletion </param>
        public FileType(string extension, int maxDuration)
        {
            if (extension.Contains('.') == false)
                extension = "." + extension;
            Extension = extension;
            MaxDuration = maxDuration;
        }

        /// <summary>
        /// Check if a file should be deleted
        /// </summary>
        /// <param name="fileFullPath"> File full path </param>
        /// <param name="isFileToBeDeleted"> Flag for deletion </param>
        /// <returns> Error code </returns>
        public FileDeletionServiceErrorCodes IsFileToBeDeleted(string fileFullPath, ref bool isFileToBeDeleted)
        {
            FileDeletionServiceErrorCodes errorCode = FileDeletionServiceErrorCodes.NoError;

            isFileToBeDeleted = false;

            if (File.Exists(fileFullPath))
            {
                FileInfo fileInfo = new FileInfo(fileFullPath);
                if (fileInfo.Extension == Extension)
                {
                    int fileTotalHours = (((fileInfo.LastWriteTime.Year) * 12 +
                                             fileInfo.LastWriteTime.Month) * 30 +
                                             fileInfo.LastWriteTime.Day) * 24 +
                                             fileInfo.LastWriteTime.Hour;
                    int nowTotalHours = (((DateTime.Now.Year) * 12 +
                                            DateTime.Now.Month) * 30 +
                                            DateTime.Now.Day) * 24 +
                                            DateTime.Now.Hour;
                    if (nowTotalHours - fileTotalHours > MaxDuration * 24)
                        isFileToBeDeleted = true;
                }
            }
            else
                errorCode = FileDeletionServiceErrorCodes.FileNotExisting;

            return errorCode;
        }

        #endregion
    }
}