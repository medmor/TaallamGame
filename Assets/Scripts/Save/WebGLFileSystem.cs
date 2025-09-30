using System.Runtime.InteropServices;

namespace TaallamGame.Save
{
    public static class WebGLFileSystem
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")] private static extern void SyncFiles();
        [DllImport("__Internal")] private static extern void LoadFiles();
#else
        private static void SyncFiles() {}
        private static void LoadFiles() {}
#endif

        public static void RequestSync()  { SyncFiles(); }
        public static void RequestLoad()  { LoadFiles(); }
    }
}
