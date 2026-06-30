#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public sealed class LocalizedInkTagValidator : IPreprocessBuildWithReport
{
    private static readonly string[] MachineTagPrefixes =
    {
        "#speaker:",
        "#id:",
        "#action:",
        "#load_scene:",
        "#bgm:",
        "#portrait:",
        "#sfx:",
        "#prop:"
    };

    public int callbackOrder => -1000;

    public void OnPreprocessBuild(BuildReport report)
    {
        ValidateOrThrow();
    }

    [MenuItem("Tools/HToJ/Validate Localized Ink Tags")]
    public static void ValidateFromMenu()
    {
        ValidateOrThrow();
        Debug.Log("[Localization] All localized Ink machine tags are ASCII-safe.");
    }

    private static void ValidateOrThrow()
    {
        string[] roots =
        {
            "Assets/Resources/Story",
            "Assets/Story"
        };

        List<string> errors = new List<string>();

        foreach (string root in roots)
        {
            if (!Directory.Exists(root))
            {
                continue;
            }

            foreach (string file in Directory.GetFiles(
                root,
                "*.ink",
                SearchOption.AllDirectories))
            {
                string name = Path.GetFileNameWithoutExtension(file);
                if (!IsLocalizedFile(name))
                {
                    continue;
                }

                string[] lines = File.ReadAllLines(file);
                for (int index = 0; index < lines.Length; index++)
                {
                    ValidateLine(
                        file,
                        index + 1,
                        lines[index],
                        errors);
                }
            }
        }

        if (errors.Count == 0)
        {
            return;
        }

        string message =
            "[Localization] Non-English machine tags found:\n" +
            string.Join("\n", errors);

        throw new BuildFailedException(message);
    }

    private static bool IsLocalizedFile(string fileName)
    {
        string upper = fileName.ToUpperInvariant();
        return upper.EndsWith("_ZH")
            || upper.EndsWith("_JP")
            || upper.EndsWith("_KR")
            || upper.EndsWith("ZH")
            || upper.EndsWith("JP")
            || upper.EndsWith("KR");
    }

    private static void ValidateLine(
        string file,
        int lineNumber,
        string line,
        List<string> errors)
    {
        foreach (string prefix in MachineTagPrefixes)
        {
            int start = line.IndexOf(
                prefix,
                StringComparison.OrdinalIgnoreCase);

            if (start < 0)
            {
                continue;
            }

            start += prefix.Length;
            string value = line.Substring(start).Trim();

            if (prefix.Equals(
                    "#id:",
                    StringComparison.OrdinalIgnoreCase))
            {
                int boundary = value.IndexOfAny(
                    new[] { ' ', '\t', ']' });
                if (boundary >= 0)
                {
                    value = value.Substring(0, boundary);
                }
            }

            if (value.Any(character => character > 127))
            {
                errors.Add(
                    $"{file}:{lineNumber}  {prefix}{value}");
            }
        }
    }
}
#endif
