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
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

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
	/// ・西暦1950年(昭和25年)から2017年迄の祝祭日をサポートする
	/// ・2017年以降の祝祭日は2017年の条件を使用する
	/// ・春・秋分の日の判定は計算で行うため実際の日とは異なる可能性がある
	/// 　尚、1950年～2017年までの春・秋分の日は計算結果と合致する。
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
			pName = null;
			DateTime date;

			// 祝祭日チェック
			HolidayData holidayData = this.SearchHoliday(pDate);
			if (holidayData != null) {
				pName = holidayData.Name;
				return holidayData.HolidayType;
			}

			// 振替チェック
			if (pDate.CompareTo(date1973_04_12) >= 0) {
				if (pDate.DayOfWeek != DayOfWeek.Sunday) {
					// 直前の日曜日が祝日なら振り替えを探す
					date = pDate.AddDays(-(int)pDate.DayOfWeek);	// 直前の日曜日の日付
					holidayData = this.SearchHoliday(date, true);
					if (holidayData != null) {								// 直前の日曜日が祝祭日なら
						while (true) {
							date = date.AddDays(1);
							holidayData = this.SearchHoliday(date, true);
							if (holidayData != null) {
								continue;
							}

							if ((date.Year == pDate.Year) && (date.Month == pDate.Month) && (date.Day == pDate.Day)) {
								pName = HolidayData.FurikaeName;
								return HolidayTypes.FURIKAE;
							}
							break;
						}
					}
				}
			}

			// 国民の休日
			if (pDate.CompareTo(date1985_12_27) >= 0) {
				if (pDate.DayOfWeek != DayOfWeek.Sunday) {
					date = pDate.AddDays(-1);
					holidayData = this.SearchHoliday(date);
					if (holidayData != null) {
						date = pDate.AddDays(1);
						holidayData = this.SearchHoliday(date);
						if (holidayData != null) {
							pName = HolidayData.KokuminName;
							return HolidayTypes.FURIKAE;
						}
					}
				}
			}

			return HolidayTypes.HEIJITU;
		}

		/// <summary>
		/// 祝祭日検索
		/// </summary>
		/// <param name="pDate">対象年月日</param>
		/// <param name="pSkipFurikae">true=振替としない設定にはヒットさせない</param>
		/// <returns>HolidayData</returns>
		private HolidayData SearchHoliday(DateTime pDate, bool pSkipFurikae = false)
		{
			// 定義リストから祝祭日を探す
			// 同時に春分・秋分の日の定義もチェックする
			bool defineHaru = false;
			bool defineAki = false;
			foreach (HolidayData data in this.holidayList) {
				if (data.IsHit(pDate)) {
					if ((pSkipFurikae == false) || (data.IgnoreFurikae == false)) {
						return data;
					}
				}

				if (data.IsHaru) {
					if (data.IsHitYear(pDate.Year)) {
						defineHaru = true;				// 春分の日の定義有り
					}
				} else if (data.IsAki) {
					if (data.IsHitYear(pDate.Year)) {
						defineAki = true;				// 秋分の日の定義有り
					}
				}
			}

			// 春分の日をチェック
			if (pDate.Month == 3) {
				// 春分の日がリストに登録されていない場合に算出する
				if (defineHaru == false) {
					int day = GetSyunbun(pDate.Year);
					if (pDate.Day == day) {
						HolidayData data = HolidayData.CreateBySyunSyuubun(pDate.Year, day, HolidayData.HaruAkiFlags.HARU);
						return data;
					}
				}
			}

			// 秋分の日をチェック
			if (pDate.Month == 9) {
				// 春分の日がリストに登録されていない場合に算出する
				if (defineAki == false) {
					int day = GetSyuubun(pDate.Year);
					if (pDate.Day == day) {
						HolidayData data = HolidayData.CreateBySyunSyuubun(pDate.Year, day, HolidayData.HaruAkiFlags.AKI);
						return data;
					}
				}
			}

			return null;
		}

		/// <summary>
		/// 規定の祝祭日定義作成
		/// </summary>
		/// <returns>2017/1/1時点を基準とする祝祭日定義リスト</returns>
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
			list.Add(HolidayData.CreateByWeek("海の日", 2021, MAXYEAR, 7, 3, 1));				// 2001年6月22日に改正、2003年1月1日に施行

			list.Add(HolidayData.CreateByDay("山の日", 2016, 2019, 8, 11));						// 2014年5月30日に改正、2016年1月1日に施行
			list.Add(HolidayData.CreateByDay("山の日", 2020, 2020, 8, 10));						// 2018年6月20日に公布・施行
			list.Add(HolidayData.CreateByDay("山の日", 2021, MAXYEAR, 8, 11));					// 2014年5月30日に改正、2016年1月1日に施行

			list.Add(HolidayData.CreateByDay("敬老の日", 1966, 2002, 9, 15));					// 1966年6月25日に改正・施行
			list.Add(HolidayData.CreateByWeek("敬老の日", 2003, MAXYEAR, 9, 3, 1));				// 2001年6月22日に改正、2003年1月1日に施行

			list.Add(HolidayData.CreateByDay("体育の日", 1966, 1999, 10, 10));					// 1966年6月25日に改正・施行
			list.Add(HolidayData.CreateByWeek("体育の日", 2000, 2019, 10, 2, 1));				// 1998年10月21日に改正、2000年1月1日に施行
			list.Add(HolidayData.CreateByDay("スポーツの日", 2020, 2020, 7, 24));				// 2018年6月20日に公布・施行
			list.Add(HolidayData.CreateByWeek("スポーツの日", 2021, MAXYEAR, 10, 2, 1));		// 2018年6月20日に公布・施行

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
		/// 尚、1950～2017年に関しては本メソッドが返す値と実際に施行された日は一致していた。
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
}
