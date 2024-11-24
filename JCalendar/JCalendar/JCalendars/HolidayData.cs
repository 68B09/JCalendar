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
	/// 祝祭日定義クラス
	/// </summary>
	[XmlRoot("Root")]
	public class HolidayData
	{
		#region フィールド/プロパティー
		/// <summary>
		/// 「春分の日」名称
		/// </summary>
		public const string SyunbunName = "春分の日";

		/// <summary>
		/// 「秋分の日」名称
		/// </summary>
		public const string SyuubunName = "秋分の日";

		/// <summary>
		/// 「振替休日」名称
		/// </summary>
		public const string FurikaeName = "振替休日";

		/// <summary>
		/// 「国民の休日」名称
		/// </summary>
		public const string KokuminName = "国民の休日";

		/// <summary>
		/// 定義タイプ
		/// </summary>
		public enum DefineTypes : int
		{
			MONTHDAY = 0,		// 月日を定義
			WEEK = 1,			// 第n週m曜日を定義
		}

		/// <summary>
		/// 春・秋分の日定義フラグ
		/// </summary>
		public enum HaruAkiFlags : int
		{
			NONE = 0,
			HARU = 1,			// 春分の日の定義
			AKI = 2,			// 秋分の日の定義
		}

		public DefineTypes DefineType { get; set; }		// 定義タイプ
		public String Name { get; set; }				// 名称
		public HolidayTypes HolidayType { get; set; }	// 休日タイプ
		public int YearFrom { get; set; }				// 適用開始年
		public int YearTo { get; set; }					// 適用終了年
		public int Month { get; set; }					// 月
		public int Day { get; set; }					// 日
		public int NumberOfWeek { get; set; }			// 第n,m曜日
		public int Week { get; set; }					// m曜日 0-日、1-月…
		public bool IgnoreFurikae { get; set; }			// true=振替しない
		public HaruAkiFlags HaruAkiFlag { get; set; }	// 春分/秋分の日定義フラグ

		/// <summary>
		/// 春分の日定義判定
		/// </summary>
		[XmlIgnore]
		public bool IsHaru
		{
			get
			{
				return this.HaruAkiFlag == HaruAkiFlags.HARU;
			}

			set
			{
				this.HaruAkiFlag = (value == true) ? (HaruAkiFlags.HARU) : (HaruAkiFlags.NONE);
			}
		}

		/// <summary>
		/// 秋分の日定義判定
		/// </summary>
		[XmlIgnore]
		public bool IsAki
		{
			get
			{
				return this.HaruAkiFlag == HaruAkiFlags.AKI;
			}

			set
			{
				this.HaruAkiFlag = (value == true) ? (HaruAkiFlags.AKI) : (HaruAkiFlags.NONE);
			}
		}
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public HolidayData()
		{
			this.DefineType = DefineTypes.MONTHDAY;
			this.Name = "";
			this.HolidayType = HolidayTypes.HEIJITU;
			this.YearFrom = 0;
			this.YearTo = 0;
			this.Month = 0;
			this.Day = 0;
			this.NumberOfWeek = 0;
			this.Week = 0;
			this.IgnoreFurikae = false;
			this.HaruAkiFlag = HaruAkiFlags.NONE;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// 合致判定
		/// </summary>
		/// <param name="pDate">調査対象年月日</param>
		/// <returns>true=合致</returns>
		public bool IsHit(DateTime pDate)
		{
			if (this.DefineType == DefineTypes.MONTHDAY) {
				// 月日の定義の場合
				if (this.IsHitYear(pDate.Year)) {
					if ((this.Month == pDate.Month) && (this.Day == pDate.Day)) {
						return true;
					}
				}
			} else if (this.DefineType == DefineTypes.WEEK) {
				// 第n,m曜日の定義の場合
				if (this.IsHitYear(pDate.Year)) {
					if (this.Month == pDate.Month) {
						if (JCalendar.IsNumberOfWeek(pDate, this.NumberOfWeek, this.Week)) {
							return true;
						}
					}
				}
			}

			return false;
		}

		/// <summary>
		/// 年合致判定
		/// </summary>
		/// <param name="pYear">調査対象西暦年</param>
		/// <returns>true=合致</returns>
		public bool IsHitYear(int pYear)
		{
			return (this.YearFrom <= pYear) && (this.YearTo >= pYear);
		}

		/// <summary>
		/// 日付作成
		/// </summary>
		/// <param name="pYear">西暦年</param>
		/// <returns>DateTime</returns>
		/// <remarks>定義の日付や条件を指定された年の日付に変換して返します。</remarks>
		public DateTime CreateDate(int pYear)
		{
			if (this.DefineType == DefineTypes.MONTHDAY) {
				// 定義が月日
				return new DateTime(pYear, this.Month, this.Day);
			}

			// 定義が週＋曜日
			return new DateTime(pYear, this.Month, JCalendar.GetDayByNthWeek(pYear, this.Month, this.NumberOfWeek, this.Week));
		}

		/// <summary>
		/// 月日型定義作成
		/// </summary>
		/// <param name="pName">祝祭日名</param>
		/// <param name="pYearFrom">適用開始西暦年</param>
		/// <param name="pYearTo">適用終了西暦年</param>
		/// <param name="pMonth">月 1～12</param>
		/// <param name="pDay">日 1～31</param>
		/// <param name="pHaruAkiFlag">春・秋分の日フラグ</param>
		/// <returns>HolidayData</returns>
		/// <remarks>
		/// 春・秋分の日を定義する場合はかならずpHaruAkiFlagにどちらの定義であるかを渡すこと。
		/// 春・秋分の日の定義にもかかわらずpHaruAkiFlag.NONEを設定した場合は同月に春もしくは秋分の日が複数回現れる事がある。
		/// </remarks>
		static public HolidayData CreateByDay(string pName, int pYearFrom, int pYearTo, int pMonth, int pDay, HaruAkiFlags pHaruAkiFlag = HaruAkiFlags.NONE)
		{
			HolidayData data = new HolidayData();
			data.Name = pName;
			data.HolidayType = HolidayTypes.SYUKUJITU;
			data.YearFrom = pYearFrom;
			data.YearTo = pYearTo;
			data.Month = pMonth;
			data.Day = pDay;
			data.HaruAkiFlag = pHaruAkiFlag;

			return data;
		}

		/// <summary>
		/// 第n,m曜日型定義作成
		/// </summary>
		/// <param name="pName">祝祭日名</param>
		/// <param name="pYearFrom">適用開始西暦年</param>
		/// <param name="pYearTo">適用終了西暦年</param>
		/// <param name="pMonth">月 1～12</param>
		/// <param name="pNumberOfWeek">第n回</param>
		/// <param name="pWeek">曜日</param>
		/// <returns>HolidayData</returns>
		static public HolidayData CreateByWeek(string pName, int pYearFrom, int pYearTo, int pMonth, int pNumberOfWeek, int pWeek)
		{
			HolidayData data = new HolidayData();
			data.DefineType = DefineTypes.WEEK;
			data.Name = pName;
			data.HolidayType = HolidayTypes.SYUKUJITU;
			data.YearFrom = pYearFrom;
			data.YearTo = pYearTo;
			data.Month = pMonth;
			data.NumberOfWeek = pNumberOfWeek;
			data.Week = pWeek;

			return data;
		}

		/// <summary>
		/// 春・秋分の日定義作成
		/// </summary>
		/// <param name="pYear">西暦年</param>
		/// <param name="pDay">日 1～31</param>
		/// <param name="pHaruAkiFlag">HaruAkiFlags.HARU、AKIのどちらか</param>
		/// <returns>HolidayData</returns>
		static public HolidayData CreateBySyunSyuubun(int pYear, int pDay, HaruAkiFlags pHaruAkiFlag)
		{
			HolidayData data = new HolidayData();
			
			switch (pHaruAkiFlag) {
				case HaruAkiFlags.HARU:
					data.Name = SyunbunName;
					data.Month = 3;
					break;

				case HaruAkiFlags.AKI:
					data.Name = SyuubunName;
					data.Month = 9;
					break;

				default:
					throw new ArgumentOutOfRangeException("pHaruAkiFlag");
			}

			data.HolidayType = HolidayTypes.SYUKUJITU;
			data.YearFrom =
			data.YearTo = pYear;
			data.Day = pDay;
			data.HaruAkiFlag = pHaruAkiFlag;

			return data;
		}

		/// <summary>
		/// 独自定義用月日型定義作成
		/// </summary>
		/// <param name="pName">祝祭日名</param>
		/// <param name="pYearFrom">適用開始西暦年</param>
		/// <param name="pYearTo">適用終了西暦年</param>
		/// <param name="pMonth">月 1～12</param>
		/// <param name="pDay">日 1～31</param>
		/// <returns>HolidayData</returns>
		static public HolidayData CreateUserByDay(string pName, int pYearFrom, int pYearTo, int pMonth, int pDay)
		{
			HolidayData data = new HolidayData();
			data.DefineType = DefineTypes.MONTHDAY;
			data.Name = pName;
			data.HolidayType = HolidayTypes.USER;
			data.YearFrom = pYearFrom;
			data.YearTo = pYearTo;
			data.Month = pMonth;
			data.Day = pDay;
			data.IgnoreFurikae = true;

			return data;
		}
		#endregion
	}
}
