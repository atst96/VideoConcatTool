using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoConcatTool
{
    internal class CommandParameters
    {
        [Option('i', "input_dir", Required = true, HelpText = "入力ディレクトリを指定します。")]
        public string InputDir { get; set; }

        [Option('o', "output_dir", Required = true, HelpText = "映像の出力先を指定します。")]
        public string OutputDir { get; set; }

        [Option('t', "interval", Required = false, HelpText = "分割された1動画ファイルあたりの尺（秒）を指定します。", Default = 180)]
        public int Interval { get; set; }
    }
}
