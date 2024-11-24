The MIT License (MIT)

Copyright (c) 2017 ZZO

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

FOR EXAMPLE

1.判定方法
	using JCalendars;

	JCalendar jc = new JCalendar();

	// 祝祭日データの変更が必要ならココで変更処理を行う
	// もしくはコンストラクタにリストを渡してインスタンスを生成する

	string name;
	HolidayTypes type = jc.IsHoliday(new DateTime(2017, 1, 1), out name);

	// type = HolidayTypes.SYUKUJITU
	// name = "元日"

2.祝祭日データ
	・各祝祭日はHolidayDataクラスで定義します

	・独自の祝祭日データを使用したい場合は祝祭日データのリスト(List<HolidayData>)をコンストラクタに渡すか、インスタンス生成後にHolidayDataListプロパティーを経由してデータを編集してください

	・祝祭日データの作り方はJCalendar.CreateDefaultHolidaysメソッドを参考にしてください
	  「月日型」と「第n週m曜日」の２パターンが登録可能です。

	・春、秋分の日を定義する場合は、HolidayData.CreateBySyunSyuubunメソッドでHolidayDataインスタンスを作成してください
	　(例) 計算では2099年の春分の日は3/20だが実際は3/21が春分の日だった場合
	　　　 hol = HolidayData.CreateBySyunSyuubun(2017, 21, HolidayData.HaruAkiFlags.HARU);
		　 「月」と「名称」は自動で設定されます。

------------------------------------------------------------------------------
[Update History]
2017/1/20	ZZO(68B09)	First Release.
