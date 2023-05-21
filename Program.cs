using System;
using System.IO;

class Program
{
    static long CalculateDirectorySize(string directory, string[] filePatterns)
    {
        long totalSize = 0;

        try
        {
            DirectoryInfo dirInfo = new DirectoryInfo(directory);

            foreach (FileInfo file in dirInfo.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                // Перевірка файлу за шаблонами
                foreach (string pattern in filePatterns)
                {
                    if (MatchesPattern(file.Name, pattern))
                    {
                        if (!IsFileVisible(file) && !IsFileReadable(file) && IsFileArchived(file))
                        {
                            totalSize += file.Length;
                            break;
                        }
                    }
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Ігноруємо доступ до файлу / каталогу, якщо немає прав доступу
        }

        return totalSize;
    }
    static bool IsFileVisible(FileInfo file)
    {
        return file.Attributes == FileAttributes.Hidden;
    }

    static bool IsFileReadable(FileInfo file)
    {
        return file.Attributes == FileAttributes.ReadOnly;
    }

    static bool IsFileArchived(FileInfo file)
    {
        return file.Attributes == FileAttributes.Archive;
    }


    static bool MatchesPattern(string filename, string pattern)
    {
        string[] parts = pattern.Split('*');

        foreach (string part in parts)
        {
            if (!string.IsNullOrEmpty(part))
            {
                if (!filename.Contains(part))
                {
                    return false;
                }

                filename = filename.Substring(filename.IndexOf(part) + part.Length);
            }
        }

        return true;
    }

    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Usage: program_name directory_path [file_pattern1 file_pattern2 ...]");
            return;
        }

        string directory = args[0];

        if (!Directory.Exists(directory))
        {
            Console.WriteLine("Directory does not exist.");
            return;
        }

        string[] filePatterns = new string[args.Length - 1];
        Array.Copy(args, 1, filePatterns, 0, args.Length - 1);

        long totalSize = CalculateDirectorySize(directory, filePatterns);

        Console.WriteLine("Total size of matching directories: " + totalSize + " bytes");
    }
}