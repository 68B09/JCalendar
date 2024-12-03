using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using JCalendars;
using System.Xml.Linq;
using System.Diagnostics;

namespace JCalendar
{
	static class Program
	{
		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main()
		{
			Sub();
			//Sub2();
			//Sub3();
			//Sub4();
			//Sub5();

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
		}

		static private void Sub()
		{
			JCalendars.JCalendar calen = new JCalendars.JCalendar();

			if (true) {
				using (FileStream fs = new FileStream(@"C:\TEMP\TEST.xml", FileMode.Create)) {
					XmlSerializer serializer = new XmlSerializer(typeof(List<HolidayData>));
					serializer.Serialize(fs, calen.HolidayDataList);

					//foreach (HolidayData item in calen.HolidayDataList) {
					//    serializer.Serialize(fs, item);
					//}
				}
			}

			if (false) {
				using (FileStream fs = new FileStream(@"C:\TEMP\TEST.xml", FileMode.Open)) {
					XmlSerializer serializer = new XmlSerializer(typeof(HolidayData));
					HolidayData data = (HolidayData)serializer.Deserialize(fs);
				}
			}
		}

#if false
		static private void Sub2()
		{
			JCalendars.JCalendar calen = new JCalendars.JCalendar();

			//HolidayData hdata;
			//hdata = HolidayData.CreateUserByDay("USER", 0, 9999, 6, 11);
			//calen.HolidayDataList.Add(hdata);
			//hdata = HolidayData.CreateUserByDay("USER", 0, 9999, 6, 13);
			//calen.HolidayDataList.Add(hdata);

			int startYear = 1955;
			int endYear = 2099;

			bool error = false;
			DateTime date = new DateTime(startYear, 1, 1);
			while (date.Year <= endYear) {
				string name1;
				HolidayTypes holidaytype1 = calen.IsHoliday(date, out name1);

				string name2;
				HolidayTypes holidaytype2 = calen.IsHolidayNew(date, out name2);

				if (holidaytype1 != holidaytype2) {
					System.Diagnostics.Debug.Write(date.ToString("yyyy/MM/dd "));
					System.Diagnostics.Debug.WriteLine("タイプ不一致 " + holidaytype1 + " != " + holidaytype2);
					error = true;
					break;
				}

				if (name1 != name2) {
					System.Diagnostics.Debug.Write(date.ToString("yyyy/MM/dd "));
					System.Diagnostics.Debug.WriteLine("名前不一致 " + name1 + " != " + name2);
					error = true;
					break;
				}

				date = date.AddDays(1);
			}
			if (error) {
				System.Diagnostics.Debug.WriteLine("× エラー");
			} else {
				System.Diagnostics.Debug.WriteLine("● ok");
			}
		}

		static private void Sub3()
		{
			JCalendars.JCalendar calen = new JCalendars.JCalendar();

			int startYear = 1955;
			int endYear = 2099;

			DateTime date = new DateTime(startYear, 1, 1);
			Stopwatch sw1 = Stopwatch.StartNew();
			while (date.Year <= endYear) {
				string name1;
				HolidayTypes holidaytype1 = calen.IsHoliday(date, out name1);
				date = date.AddDays(1);
			}
			sw1.Stop();

			date = new DateTime(startYear, 1, 1);
			Stopwatch sw2 = Stopwatch.StartNew();
			while (date.Year <= endYear) {
				string name2;
				HolidayTypes holidaytype1 = calen.IsHolidayNew(date, out name2);
				date = date.AddDays(1);
			}
			sw2.Stop();

			System.Diagnostics.Debug.WriteLine("1 " + sw1.ElapsedMilliseconds);
			System.Diagnostics.Debug.WriteLine("2 " + sw2.ElapsedMilliseconds);
		}
		static private void Sub4()
		{
			JCalendars.JCalendar calen = new JCalendars.JCalendar();

			int startYear = 1955;
			int endYear = 2099;

			DateTime date = new DateTime(startYear, 1, 1);
			Stopwatch sw1 = Stopwatch.StartNew();
			while (date.Year <= endYear) {
				string name1;
				HolidayTypes holidaytype1 = calen.IsHoliday(date, out name1);
				date = date.AddDays(1);
			}
			sw1.Stop();

			System.Diagnostics.Debug.WriteLine("1 " + sw1.ElapsedMilliseconds);
		}

		static void Sub5()
		{
			int before = 0;
			DateTime date = new DateTime(2025, 1, 1);
			while (date.Year <= 2027) {
				int no = JCalendars.JCalendar.GetWeekOfYear(date);
				if ((before == 0) || (before != no)) {
					before = no;
					System.Diagnostics.Debug.WriteLine("no:" + before + " Date:" + date.ToString("yyyy/MM/dd"));

					if((date.DayOfWeek != DayOfWeek.Sunday) && ((date.Month != 1) && (date.Day != 1)) ){
						System.Diagnostics.Debug.WriteLine("×Error");
						break;
					}
				}

				date = date.AddDays(1);
			}
		}
#endif	
	}
}
