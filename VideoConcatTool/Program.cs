using CommandLine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VideoConcatTool
{
    internal static class Program
    {
        private const string FFmpegPath = "ffmpeg.exe";

        internal static void Main(string[] args)
        {
            var parser = Parser.Default.ParseArguments<CommandParameters>(args);

            if (parser is NotParsed<CommandParameters> notParsed)
            {
                return;
            }

            var parameters = ((Parsed<CommandParameters>)parser).Value;

            var InputDir = parameters.InputDir;
            var OutputDir = parameters.OutputDir;
            var splitDuration = TimeSpan.FromMinutes(parameters.Interval);

            var fileGroups = Directory.GetFiles(InputDir, "*.mp4", SearchOption.AllDirectories)
                .AsParallel()
                .Select(path => GetVideoInfo(path))
                .ExcludeDefault()
                .Collect(splitDuration);

            if (!Directory.Exists(OutputDir))
            {
                Directory.CreateDirectory(OutputDir);
            }

            foreach (var files in fileGroups)
            {
                Console.WriteLine("====");

                if (files.Length > 1)
                {
                    ProcMultipleFiles(files, OutputDir);
                }
                else
                {
                    ProcSingleFile(files[0], OutputDir);
                }

                Console.WriteLine();
            }
        }

        private static void ProcSingleFile(VideoInfo info, string outputDir)
        {
            var baseName = Path.GetFileName(Path.GetDirectoryName(info.Path));

            var outputFileName = $"{baseName}#{Path.GetFileName(info.Path)}";
            var outputPath = Path.Combine(outputDir, outputFileName);

            Console.WriteLine(" : " + info.Path);
            Console.WriteLine(" -> " + outputPath);

            if (File.Exists(outputPath))
            {
                Console.WriteLine("コピー先にファイルが存在します。処理をスキップします。");
                return;
            }

            File.Copy(info.Path, outputPath);
        }

        private static void ProcMultipleFiles(VideoInfo[] videos, string outputDir)
        {
            var firstInfo = videos[0];
            var lastInfo = videos[videos.Length - 1];

            var baseName = Path.GetFileName(Path.GetDirectoryName(firstInfo.Path));
            var outputFileName = $"{baseName}#{Path.GetFileNameWithoutExtension(lastInfo.Path)}-{Path.GetFileName(lastInfo.Path)}";
            var outputPath = Path.Combine(outputDir, outputFileName);

            foreach (var info in videos)
            {
                Console.WriteLine(" : " + info.Path);
            }

            Console.WriteLine(" -> " + outputPath);

            ConcatFiles(videos, outputPath);
        }

        private static void ConcatFiles(VideoInfo[] files, string outputPath)
        {
            var concatListFile = Path.GetTempFileName();

            using (var tempStr = File.OpenWrite(concatListFile))
            using (var writer = new StreamWriter(tempStr, new UTF8Encoding(false)))
            {
                var fileArgs = files.Select(v => $"file '{v.Path}'");

                writer.Write(string.Join("\n", fileArgs));
            }

            var args = $"-f concat -safe 0 -i \"{concatListFile}\" -c copy \"{outputPath}\"";

            using (var p = CreateCLIProcess(FFmpegPath, args))
            {
                p.Start();
                p.WaitForExit();
            }

            File.Delete(concatListFile);
        }

        private static Process CreateCLIProcess(string executePath, string arguments)
        {
            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = executePath,
                    Arguments = arguments,
                    UseShellExecute = false,
                }
            };
        }

        private static IEnumerable<VideoInfo[]> Collect(this IEnumerable<VideoInfo> videos, TimeSpan splitDuration)
        {
            var marginedSplitDuration = splitDuration.Add(TimeSpan.FromSeconds(5.0d));

            var list = new LinkedList<VideoInfo>();

            foreach (var info in videos.OrderBy(v => v.DateTime))
            {
                if (list.Count >= 1)
                {
                    var duration = info.DateTime - list.Last.Value.DateTime;

                    if (duration > marginedSplitDuration)
                    {
                        yield return list.ToArray();
                        list.Clear();
                    }
                }

                list.AddLast(info);
            }

            yield return list.ToArray();
            list.Clear();

            list = null;
        }

        private static VideoInfo GetVideoInfo(string path)
        {
            try
            {
                return new VideoInfo(path);
            }
            catch
            {
                return null;
            }
        }
    }
}
