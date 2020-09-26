using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RemoteGitDeploy.Extensions {
    public static class IoExtensions {

        public static void DeleteReadOnly(this FileSystemInfo fileSystemInfo) {
            if (fileSystemInfo is DirectoryInfo directoryInfo) {
                foreach (var childInfo in directoryInfo.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly)) {
                    DeleteReadOnly(childInfo);
                }
            }
            fileSystemInfo.Attributes = FileAttributes.Normal;
            fileSystemInfo.Delete();
        }
    }
}
