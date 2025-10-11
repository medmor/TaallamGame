using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace TaallamGame.EditorTools
{
    public static class InkBidiCleaner
    {
        // Bidi control characters to strip from source files
        private static readonly char[] BidiControls = new[]
        {
            '\u2066', // LRI
            '\u2067', // RLI
            '\u2068', // FSI
            '\u2069', // PDI
            '\u202A', // LRE
            '\u202B', // RLE
            '\u202D', // LRO
            '\u202E', // RLO
            '\u202C', // PDF
            '\u200E', // LRM
            '\u200F', // RLM
            '\u061C', // ALM (Arabic Letter Mark)
        };

        [MenuItem("Tools/Ink/Clean Bidi Controls (All .ink)")]
        public static void CleanAllInkBidi()
        {
            // You can change this root to scan other folders as needed
            var root = Path.Combine(Application.dataPath, "Missions/BookMission/Dialogues");
            if (!Directory.Exists(root))
            {
                Debug.LogWarning($"[InkBidiCleaner] Folder not found: {root}");
                return;
            }

            var files = Directory.GetFiles(root, "*.ink", SearchOption.AllDirectories);
            if (files.Length == 0)
            {
                Debug.Log("[InkBidiCleaner] No .ink files found to process.");
                return;
            }

            int totalFiles = 0;
            int modifiedFiles = 0;
            int totalRemoved = 0;

            foreach (var file in files)
            {
                totalFiles++;
                string original = File.ReadAllText(file, Encoding.UTF8);
                string cleaned = StripBidiControls(original, out int removedCount);

                if (removedCount > 0)
                {
                    File.WriteAllText(file, cleaned, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
                    modifiedFiles++;
                    totalRemoved += removedCount;
                    Debug.Log($"[InkBidiCleaner] Cleaned {removedCount} bidi chars -> {file}");
                }
            }

            Debug.Log($"[InkBidiCleaner] Done. Files scanned: {totalFiles}, modified: {modifiedFiles}, bidi chars removed: {totalRemoved}");
            AssetDatabase.Refresh();
        }

        private static string StripBidiControls(string s, out int removed)
        {
            removed = 0;
            if (string.IsNullOrEmpty(s)) return s;

            // Fast path: if string contains none of the bidi controls, return original
            if (!BidiControls.Any(s.Contains)) return s;

            var sb = new StringBuilder(s.Length);
            foreach (var ch in s)
            {
                if (Array.IndexOf(BidiControls, ch) >= 0)
                {
                    removed++;
                    continue;
                }
                sb.Append(ch);
            }
            return sb.ToString();
        }
    }
}
