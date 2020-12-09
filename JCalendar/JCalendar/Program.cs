using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using JCalendars;

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
	}
}
