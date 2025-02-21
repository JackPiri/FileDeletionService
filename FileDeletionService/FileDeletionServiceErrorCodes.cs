namespace FileDeletionService
{
    /// <summary>
    /// Error codes
    /// </summary>
    public enum FileDeletionServiceErrorCodes
    {
        /// <summary>
        /// No error
        /// </summary>
        NoError = 0,
        /// <summary>
        /// File not existing
        /// </summary>
        FileNotExisting = -10,
        /// <summary>
        /// Folder not existing
        /// </summary>
        FolderNotExisting = -20,
        /// <summary>
        /// File type already present
        /// </summary>
        FileTypeAlreadyPresent = -21,
        /// <summary>
        /// Folder type already present
        /// </summary>
        FolderTypeAlreadyPresent = -30,
        /// <summary>
        /// Disk type already present
        /// </summary>
        DiskTypeAlreadyPresent = -40,
        /// <summary>
        /// Disk type missing
        /// </summary>
        DiskTypeMissing = -41,
        /// <summary>
        /// Folder type missing
        /// </summary>
        FolderTypeMissing = -42,
        /// <summary>
        /// Starting deletion
        /// </summary>
        StartingDeletion = -43,
        /// <summary>
        /// Stopping deletion
        /// </summary>
        StoppingDeletion = -44,
        /// <summary>
        /// Deletion
        /// </summary>
        Deletion = -45
    }
}