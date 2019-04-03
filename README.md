# VideoConcatTool
ドライブレコーダ（VANTRUE X2）で録画した動画ファイル（記録設定で一定時間毎に分割されたもの）を、レコーダの録画開始時ごとに結合するプログラムです。

結合に[FFmpeg](https://ffmpeg.org/)を使用するため、ffmpeg.exeへのパスを通しておく必要があります。エンコードは行いません。

## 入力ディレクトリの構成
コマンドライン引数の`input_dir`で指定したディレクトリから、次の構成で格納されたファイルを使用します。

```
{input_dir}\**\yyMMdd[任意の3桁の数字]\hhmmss[任意の2文字].MP4

e.g.) {input_dir}\DCIM\190216000\173641AA.MP4
      {input_dir}\EVENT\190216000\150459AA.MP4
```

## ライセンス
- VideoConcatTool は[MITライセンス](https://github.com/atst1996/video-concat-tool/blob/master/LICENSE)で配布しています。
- 以下のソフトウェアが含まれます。ライセンスについては[NOTICE.md](https://github.com/atst1996/video-concat-tool/blob/master/NOTICE.md)に記載しています。
  - Command Line Parser Library for CLR and NetStandard
