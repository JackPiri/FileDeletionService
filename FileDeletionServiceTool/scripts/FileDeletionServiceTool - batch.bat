REM "TRIGGERTIME=%d" for time interval (in hours) to subsequent trigger
REM "DISK=%s|%d|%d" for disk name, minimum free space in GBs, maximum used space in GBs
REM "FOLDER=%s|%s|%s|%s|%d|%d" for disk name, folder name, deleting folder if empty, delete subfolders if empty, subfolders first deepness to inspect, subfolders last deepness to inspect
REM "FILE=%s|%s|%s|%d" for disk name, folder name, file extension, file maximum duration in days

start "FileDeletionServiceTool" "FileDeletionServiceTool.exe" "TRIGGERTIME=1" "DISK=C:|100|10" "FOLDER=C:|C:\Users\MyUser\Desktop|false|false|0|0" "FILE=C:|C:\Users\MyUser\Desktop|.txt|1"

PAUSE