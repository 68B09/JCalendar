/*
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

[Update History]
2017/01/20	ZZO(68B09)	First Release.
2020/12/09	ZZO(68B09)	2021年(令和3年)用の定義を追加
2024/11/23	ZZO(68B09)	連続判定時のパフォーマンス向上のためロジックを刷新
						IsHolidayが国民の休日をFURIKAEで返す不具合を修正
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JCalendars
{
	/// <summary>
	/// 祝祭日タイプ
	/// </summary>
	public enum HolidayTypes : int
	{
		HEIJITU = 0,	// 平日
		SYUKUJITU,		// 国民の祝日
		FURIKAE,		// 振替休日
		KOKUMIN,		// 国民の休日
		USER,			// ユーザー定義の休日
	}

	/// <summary>
	/// 日本用カレンダークラス
	/// </summary>
	/// <remarks>
	/// ・指定される西暦年月日が日本の祝祭日であるかを判定する
	/// ・西暦1950年(昭和25年)から2021年迄の祝祭日をサポートする
	/// ・2021年以降の祝祭日は2021年の条件を使用する
	/// ・春・秋分の日の判定は計算で行うため実際の日とは異なる可能性がある
	/// 　尚、1950年～2021年までの公布された春・秋分の日は計算結果と合致する。
	/// </remarks>
	public class JCalendar
	{
		#region フィールド/プロパティー
		/// <summary>
		/// 振替休日 制定・施行日
		/// /// </summary>
		private static readonly DateTime date1973_04_12 = new DateTime(1973, 4, 12);

		/// <summary>
		/// 国民の休日 制定・施行日
		/// </summary>
		private static readonly DateTime date1985_12_27 = new DateTime(1985, 12, 27);

		/// <summary>
		/// 祝祭日リスト
		/// </summary>
		private List<HolidayData> holidayList;

		/// <summary>
		/// 祝祭日リスト取得/設定
		/// </summary>
		public List<HolidayData> HolidayDataList
		{
			get
			{
				return this.holidayList;
			}
		}

		/// <summary>
		/// 現施行用祝祭日終了年
		/// </summary>
		public const int MAXYEAR = 9999;

		/// <summary>
		/// 年ごとの祝日休日辞書
		/// </summary>
		private Dictionary<int, HolidayCache> yearlyHolidayCache = new Dictionary<int, HolidayCache>();
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ(規定の祝祭日定義を使用)
		/// </summary>
		public JCalendar()
		{
			this.holidayList = CreateDefaultHolidays();
		}

		/// <summary>
		/// コンストラクタ(カスタム祝祭日定義を使用)
		/// </summary>
		public JCalendar(List<HolidayData> pHolidayList)
		{
			this.holidayList = pHolidayList;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// 祝祭日判定
		/// </summary>
		/// <param name="pDate">判定対象年月日</param>
		/// <param name="pName">祝祭日名称(平日などの名称が無い場合はnull)</param>
		/// <returns>HolidayTypes</returns>
		/// <remarks>
		/// pDateの祝祭日判定結果を返す。
		/// 戻り値がHolidayTypes.HEIJITU以外であればpNameに名称が返る。
		/// 祝祭日では無い土・日曜日は平日と見なされる。
		/// </remarks>
		public HolidayTypes IsHoliday(DateTime pDate, out string pName)
		{
			// 作成済みならそれを、未作成なら１年分を作る
			HolidayCache holidayCache = null;
			if (this.yearlyHolidayCache.ContainsKey(pDate.Year)) {
				holidayCache = this.yearlyHolidayCache[pDate.Year];
			} else {
				holidayCache = this.CreateYearlyHolidays(pDate.Year);
				this.yearlyHolidayCache[pDate.Year] = holidayCache;
			}

			// 休日検索
			HolidayCacheRecord holidayCacheData;
			if (holidayCache.TryGetValue(pDate.Date, out holidayCacheData)) {
				pName = holidayCacheData.Name;
				return holidayCacheData.HolidayType;
			}

			pName = null;
			return HolidayTypes.HEIJITU;
		}

		/// <summary>
		/// 指定された年の休日一覧を作る
		/// </summary>
		/// <param name="pYear">西暦年</param>
		/// <returns>HolidayCache</returns>
		private HolidayCache CreateYearlyHolidays(int pYear)
		{
			// pYearと前後１ヶ月の祝日一覧を作る。
			// 振替休日発生と国民の休日が年末年始に発生するケースを想定して前後を作る。
			// ただし2024年現在はそのような休日は発生しない。
			List<HolidayCacheRecord> holidays = new List<HolidayCacheRecord>();
			holidays.AddRange(this.CreateHolidays(pYear - 1, 12, 12));
			holidays.AddRange(this.CreateHolidays(pYear));
			holidays.AddRange(this.CreateHolidays(pYear + 1, 1, 1));

			// 休日一覧辞書
			HolidayCache dic = new HolidayCache();
			holidays.ForEach(item => dic.Add(item.Date, item));

			// 振替休日を追加
			foreach (HolidayCacheRecord data in holidays) {
				// 日曜日以外は対象外
				if (data.Date.DayOfWeek != DayOfWeek.Sunday) {
					continue;
				}

				// 国民の祝日以外は対象外
				if (data.HolidayType != HolidayTypes.SYUKUJITU) {
					continue;
				}

				// 適用日より前は対象外
				if (data.Date.CompareTo(date1973_04_12) < 0) {
					continue;
				}

				// 次の平日を振替休日にする
				DateTime date = data.Date.AddDays(1);
				while (true) {
					if ((dic.ContainsKey(date) == false) && (date.DayOfWeek != DayOfWeek.Sunday)) {
						HolidayCacheRecord furikaeData = new HolidayCacheRecord(date, HolidayTypes.FURIKAE, HolidayData.FurikaeName);
						dic.Add(date, furikaeData);
						break;
					}
					date = date.AddDays(1);
				}
			}

			// 国民の休日を追加
			foreach (HolidayCacheRecord data in holidays) {
				// 挟まれる日が日曜日(挟む日が土曜日)なら対象外
				if (data.Date.DayOfWeek == DayOfWeek.Saturday) {
					continue;
				}

				// 国民の祝日以外は対象外
				if (data.HolidayType != HolidayTypes.SYUKUJITU) {
					continue;
				}

				// 適用日より前は対象外
				if (data.Date.CompareTo(date1985_12_27) < 0) {
					continue;
				}

				DateTime date1 = data.Date.AddDays(1);
				if ((dic.ContainsKey(date1) == false)) {
					DateTime date2 = date1.AddDays(1);
					if (dic.ContainsKey(date2)) {
						if (dic[date2].HolidayType == HolidayTypes.SYUKUJITU) {
							HolidayCacheRecord kokuminData = new HolidayCacheRecord(date1, HolidayTypes.KOKUMIN, HolidayData.KokuminName);
							dic.Add(date1, kokuminData);
						}
					}
				}
			}

			// 指定された年以外のデータを削除
			KeyValuePair<DateTime, HolidayCacheRecord>[] removeData = dic.Where(x => x.Key.Year != pYear).ToArray();
			foreach (KeyValuePair<DateTime, HolidayCacheRecord> item in removeData) {
				dic.Remove(item.Key);
			}

			return dic;
		}

		/// <summary>
		/// 定義を素に指定された年の祝日一覧を作る
		/// </summary>
		/// <param name="pYear">西暦年</param>
		/// <param name="pMonthMin">最小月(1～12)</param>
		/// <param name="pMonthMax">最大月(1～12)</param>
		/// <returns>List<HolidayCacheData></returns>
		private List<HolidayCacheRecord> CreateHolidays(int pYear, int pMonthMin = 1, int pMonthMax = 12)
		{
			// 祝日定義情報から指定された年の祝日の一覧を作成
			bool defineHaru = false;
			bool defineAki = false;
			List<HolidayCacheRecord> holidays = new List<HolidayCacheRecord>();
			foreach (HolidayData data in this.holidayList) {
				// 適用される年以外のデータは無視
				if (data.IsHitYear(pYear) == false) {
					continue;
				}
				// この祝日の指定された年の年月日
				DateTime date = data.CreateDate(pYear);
				// 範囲外の月のデータは無視
				if ((date.Month < pMonthMin) || (data.Month > pMonthMax)) {
					continue;
				}
				// リストに追加
				holidays.Add(new HolidayCacheRecord(date, data.HolidayType, data.Name));
				// 春分の日もしくは秋分の日の定義だった場合はフラグを立てて自動生成させない
				if (data.IsHaru) {
					defineHaru = true;
				} else if (data.IsAki) {
					defineAki = true;
				}
			}
			// 春分の日追加
			if (defineHaru == false) {
				if ((pMonthMin <= 3) && (pMonthMax >= 3)) {
					holidays.Add(new HolidayCacheRecord(new DateTime(pYear, 3, GetSyunbun(pYear)), HolidayTypes.SYUKUJITU, HolidayData.SyunbunName));
				}
			}
			// 秋分の日追加
			if (defineAki == false) {
				if ((pMonthMin <= 9) && (pMonthMax >= 9)) {
					holidays.Add(new HolidayCacheRecord(new DateTime(pYear, 9, GetSyuubun(pYear)), HolidayTypes.SYUKUJITU, HolidayData.SyuubunName));
				}
			}

			// 日付順にソート
			holidays.Sort((x, y) => DateTime.Compare(x.Date, y.Date));

			return holidays;
		}

		/// <summary>
		/// 規定の祝祭日定義作成
		/// </summary>
		/// <returns>祝祭日定義リスト</returns>
		/// <remarks>
		/// 2020/12/09	ZZO(68B09)	2021年(令和3年)用の定義を追加
		/// </remarks>
		static public List<HolidayData> CreateDefaultHolidays()
		{
			List<HolidayData> list = new List<HolidayData>();

			list.Add(HolidayData.CreateByDay("元日", 1949, MAXYEAR, 1, 1));						// 1948年7月20日に公布・施行

			list.Add(HolidayData.CreateByDay("成人の日", 1949, 1999, 1, 15));					// 1948年7月20日に公布・施行
			list.Add(HolidayData.CreateByWeek("成人の日", 2000, MAXYEAR, 1, 2, 1));				// 1998年10月21日に改正、2000年1月1日に施行

			list.Add(HolidayData.CreateByDay("建国記念の日", 1967, MAXYEAR, 2, 11));			// 1966年6月25日に改正・施行

			list.Add(HolidayData.CreateByDay("天皇誕生日", 1949, 1988, 4, 29));					// 1948年7月20日に公布・施行
			list.Add(HolidayData.CreateByDay("天皇誕生日", 1989, 2018, 12, 23));				// 1989年2月17日に改正・施行
			list.Add(HolidayData.CreateByDay("天皇誕生日", 2020, MAXYEAR, 2, 23));				// 1989年2月17日に改正・施行

			list.Add(HolidayData.CreateByDay("みどりの日", 1989, 2006, 4, 29));					// 1989年2月17日に改正・施行
			list.Add(HolidayData.CreateByDay("みどりの日", 2007, MAXYEAR, 5, 4));				// 2005年5月20日に改正、2007年1月1日に施行

			list.Add(HolidayData.CreateByDay("昭和の日", 2007, MAXYEAR, 4, 29));				// 2005年5月20日に改正、2007年1月1日に施行

			list.Add(HolidayData.CreateByDay("憲法記念日", 1949, MAXYEAR, 5, 3));				// 1948年7月20日に公布・施行

			list.Add(HolidayData.CreateByDay("こどもの日", 1949, MAXYEAR, 5, 5));				// 1948年7月20日に公布・施行

			list.Add(HolidayData.CreateByDay("海の日", 1996, 2002, 7, 20));						// 1995年3月8日に改正、1996年1月1日に施行
			list.Add(HolidayData.CreateByWeek("海の日", 2003, 2019, 7, 3, 1));					// 2001年6月22日に改正、2003年1月1日に施行
			list.Add(HolidayData.CreateByDay("海の日", 2020, 2020, 7, 23));						// 2018年6月20日に公布・施行
			list.Add(HolidayData.CreateByDay("海の日", 2021, 2021, 7, 22));						// 2021年公布
			list.Add(HolidayData.CreateByWeek("海の日", 2022, MAXYEAR, 7, 3, 1));				// 2001年6月22日に改正、2003年1月1日に施行

			list.Add(HolidayData.CreateByDay("山の日", 2016, 2019, 8, 11));						// 2014年5月30日に改正、2016年1月1日に施行
			list.Add(HolidayData.CreateByDay("山の日", 2020, 2020, 8, 10));						// 2018年6月20日に公布・施行
			list.Add(HolidayData.CreateByDay("山の日", 2021, 2021, 8, 8));						// 2021年公布
			list.Add(HolidayData.CreateByDay("山の日", 2022, MAXYEAR, 8, 11));					// 2014年5月30日に改正、2016年1月1日に施行

			list.Add(HolidayData.CreateByDay("敬老の日", 1966, 2002, 9, 15));					// 1966年6月25日に改正・施行
			list.Add(HolidayData.CreateByWeek("敬老の日", 2003, MAXYEAR, 9, 3, 1));				// 2001年6月22日に改正、2003年1月1日に施行

			list.Add(HolidayData.CreateByDay("体育の日", 1966, 1999, 10, 10));					// 1966年6月25日に改正・施行
			list.Add(HolidayData.CreateByWeek("体育の日", 2000, 2019, 10, 2, 1));				// 1998年10月21日に改正、2000年1月1日に施行
			list.Add(HolidayData.CreateByDay("スポーツの日", 2020, 2020, 7, 24));				// 2018年6月20日に公布・施行
			list.Add(HolidayData.CreateByDay("スポーツの日", 2021, 2021, 7, 23));				// 2021年公布
			list.Add(HolidayData.CreateByWeek("スポーツの日", 2022, MAXYEAR, 10, 2, 1));		// 2018年6月20日に公布・施行

			list.Add(HolidayData.CreateByDay("文化の日", 1948, MAXYEAR, 11, 3));				// 1948年7月20日に公布・施行

			list.Add(HolidayData.CreateByDay("勤労感謝の日", 1948, MAXYEAR, 11, 23));			// 1948年7月20日に公布・施行

			list.Add(HolidayData.CreateByDay("大喪の礼", 1989, 1989, 2, 24));

			list.Add(HolidayData.CreateByDay("明仁親王の結婚の儀", 1959, 1959, 4, 10));
			list.Add(HolidayData.CreateByDay("徳仁親王の結婚の儀", 1993, 1993, 6, 9));			// 1993年4月30日に公布・施行

			list.Add(HolidayData.CreateByDay("即位礼正殿の儀", 1990, 1990, 11, 12));
			list.Add(HolidayData.CreateByDay("即位礼正殿の儀", 2019, 2019, 10, 22));			// 2018年12月14日に公布・施行

			list.Add(HolidayData.CreateByDay("天皇の即位の日", 2019, 2019, 5, 1));				// 2018年12月14日に公布・施行

			return list;
		}

		/// <summary>
		/// 春分の日算出
		/// </summary>
		/// <param name="pYear">西暦年</param>
		/// <returns>日 1～31</returns>
		/// <remarks>
		/// pYear年の春分の日を計算して返す。
		/// 本メソッドが返す日は予想であり、実際の春分の日は前年に決定・発表されることに注意すること。
		/// 尚、1950～2017年に関しては本メソッドが返す値と実際に施行された日は一致していた。
		/// </remarks>
		static public int GetSyunbun(int pYear)
		{
			int iDay;

			if (pYear < 1949) {
				return 0;
			}

			if (pYear < 1980) {
				iDay = (int)(20.8357 + 0.242194 * (pYear - 1980) - (int)((pYear - 1983) / 4));
			} else if (pYear < 2100) {
				iDay = (int)(20.8431 + 0.242194 * (pYear - 1980) - (int)((pYear - 1980) / 4));
			} else {
				iDay = (int)(21.8510 + 0.242194 * (pYear - 1980) - (int)((pYear - 1980) / 4));
			}

			return iDay;
		}

		/// <summary>
		/// 秋分の日算出
		/// </summary>
		/// <param name="pYear">西暦年</param>
		/// <returns>日 1～31</returns>
		/// <remarks>
		/// pYear年の秋分の日を計算して返す。
		/// 本メソッドが返す日は予想であり、実際の秋分の日は前年に決定・発表されることに注意すること。
		/// 尚、1950～2024年に関しては本メソッドが返す値と実際に施行された日は一致していた。
		/// </remarks>
		static public int GetSyuubun(int pYear)
		{
			int iDay;

			if (pYear < 1948) {
				return 0;
			}

			if (pYear < 1980) {
				iDay = (int)(23.2588 + 0.242194 * (pYear - 1980) - (int)((pYear - 1983) / 4));
			} else if (pYear < 2100) {
				iDay = (int)(23.2488 + 0.242194 * (pYear - 1980) - (int)((pYear - 1980) / 4));
			} else {
				iDay = (int)(24.2488 + 0.242194 * (pYear - 1980) - (int)((pYear - 1980) / 4));
			}

			return iDay;
		}

		/// <summary>
		/// 週番号取得
		/// </summary>
		/// <param name="pDate">調査対象年月日</param>
		/// <returns>1～6</returns>
		/// <remarks>
		/// 日 月 火 水 木 金 土
		///                    1　　<= 第１週　return 1;
		///  2  3  4  5  6  7  8　　<= 第２週　return 2;
		///  9 10 11 12 13 14 15
		/// 16 17 18 19 20 21 22
		/// 23 24 25 26 27 28 29
		/// 30 31
		/// </remarks>
		static public int GetLine(DateTime pDate)
		{
			DateTime day1 = new DateTime(pDate.Year, pDate.Month, 1);
			return (pDate.Day + (int)day1.DayOfWeek - 1) / 7 + 1;
		}

		/// <summary>
		/// 第Ｎ曜日取得
		/// </summary>
		/// <param name="pDay">調査対象年月日</param>
		/// <returns>1～5</returns>
		/// <remarks>
		/// 日 月 火 水 木 金 土
		///                    1　　1日=第１土曜(return 1)
		///  2  3  4  5  6  7  8　　3日=第１月曜(return 1)  8日=第２土曜(return 2)
		///  9 10 11 12 13 14 15
		/// 16 17 18 19 20 21 22
		/// 23 24 25 26 27 28 29
		/// 30 31
		/// </remarks>
		static public int GetCountOfWeek(int pDay)
		{
			// 日曜日にあわせて7で割る
			return ((pDay - 1) / 7) + 1;
		}

		/// <summary>
		/// 第Ｎ、Ｗ曜日の日を取得
		/// </summary>
		/// <param name="pYear">西暦年</param>
		/// <param name="pMonth">月</param>
		/// <param name="pNth">週番号(1～)</param>
		/// <param name="pWeek">曜日(DayOfWeek)</param>
		/// <returns>日(1～31)</returns>
		/// <remarks>
		/// 日 月 火 水 木 金 土
		///                    1
		///  2  3  4  5  6  7  8　　第１月曜日　return 3;
		///  9 10 11 12 13 14 15　　第２月曜日　return 10;
		/// 16 17 18 19 20 21 22
		/// 23 24 25 26 27 28 29
		/// 30 31　　               第５火曜日　ArgumentOutOfRangeException;
		/// </remarks>
		static public int GetDayByNthWeek(int pYear, int pMonth, int pNth, int pWeek)
		{
			int firstWeek = (int)(new DateTime(pYear, pMonth, 1).DayOfWeek);

			// 最初のW曜日の日を計算
			int day = 1 + ((pWeek - firstWeek + 7) % 7);

			// N番目のW曜日の日を計算
			day = day + (pNth - 1) * 7;

			// 日数チェック
			if (day > GetDays(pYear, pMonth)) {
				throw new ArgumentOutOfRangeException();
			}

			return day;
		}

		/// <summary>
		/// 第Ｎ、Ｗ曜日判定
		/// </summary>
		/// <param name="pDate">調査対象年月日</param>
		/// <param name="pCountOfWeek">第Ｎ</param>
		/// <param name="pWeek">Ｗ曜日(0=日、1=月、2=火、3=水、4=木、5=金、6=土)</param>
		/// <returns>true=一致</returns>
		/// <remarks>
		/// 「第３土曜日か？」を判定したい場合は、IsNumberOfWeek( 3, 6 )で呼び出す。
		/// </remarks>
		static public bool IsNumberOfWeek(DateTime pDate, int pCountOfWeek, int pWeek)
		{
			return (GetCountOfWeek(pDate.Day) == pCountOfWeek) && ((int)pDate.DayOfWeek == pWeek);
		}

		/// <summary>
		/// 日数取得
		/// </summary>
		/// <param name="pYear">西暦年</param>
		/// <param name="pMonth">月 1～12</param>
		/// <returns>日数 1～31</returns>
		/// <remarks>
		/// 指定された年月の日数を返す。
		/// </remarks>
		static public int GetDays(int pYear, int pMonth)
		{
			switch (pMonth) {
				case 1:
				case 3:
				case 5:
				case 7:
				case 8:
				case 10:
				case 12:
					return 31;

				case 2:
					if (DateTime.IsLeapYear(pYear)) {
						return 29;
					}
					return 28;

				case 4:
				case 6:
				case 9:
				case 11:
					return 30;
			}

			throw new ArgumentOutOfRangeException("pMonth");
		}
		#endregion
	}

	/// <summary>
	/// 休日キャッシュレコード
	/// </summary>
	public class HolidayCacheRecord
	{
		#region フィールド/プロパティー
		public DateTime Date { get; set; }				// 日付
		public HolidayTypes HolidayType { get; set; }	// 祝祭日タイプ
		public string Name { get; set; }                // 名称
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public HolidayCacheRecord()
		{
			this.Date = DateTime.MinValue;
			this.HolidayType = HolidayTypes.HEIJITU;
			this.Name = "";
		}

		/// <summary>
		/// コンストラクタ(初期値指定)
		/// </summary>
		public HolidayCacheRecord(DateTime pDate, HolidayTypes pType, string pName)
		{
			this.Date = pDate;
			this.HolidayType = pType;
			this.Name = pName;
		}
		#endregion

		/// <summary>
		/// 文字列化
		/// </summary>
		/// <returns>string</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(this.Date.ToString("yyyy/MM/dd "));
			sb.Append(this.HolidayType);
			sb.Append(" ");
			sb.Append(this.Name);
			return sb.ToString();
		}
	}

	/// <summary>
	/// 休日キャッシュ辞書
	/// </summary>
	internal class HolidayCache : Dictionary<DateTime, HolidayCacheRecord> { }
}
