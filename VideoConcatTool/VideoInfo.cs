using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VideoConcatTool
{
    internal class VideoInfo
    {
        private static Regex _fileFilterRegex = new Regex(@"\\(?<date>\d{6})\d{3}\\(?<time>\d{6})[a-z]{2}\..+$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public string Path { get; }
        public DateTime DateTime { get; }
        public VideoInfo(string path)
        {
            this.Path = path;

            var match = _fileFilterRegex.Match(path);
            if (!match.Success)
            {
                throw new NotSupportedException();
            }

            var date = match.Groups["date"].Value;
            var time = match.Groups["time"].Value;

            int year = int.Parse("20" + date.Substring(0, 2));
            int month = int.Parse(date.Substring(2, 2));
            int day = int.Parse(date.Substring(4, 2));
            int hour = int.Parse(time.Substring(0, 2));
            int minute = int.Parse(time.Substring(2, 2));
            int second = int.Parse(time.Substring(4, 2));

            this.DateTime = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Local);
        }
    }
}
