using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;

namespace JCalendar
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (this.DesignMode) {
				return;
			}

			int yy = DateTime.Now.Year;
			for (int i = 1950; i <= 2099; i++) {
				this.cmbYear.Items.Add(i.ToString());
				this.cmbYearTo.Items.Add(i.ToString());
				if (i == yy) {
					this.cmbYear.SelectedIndex =
					this.cmbYearTo.SelectedIndex = this.cmbYear.Items.Count - 1;
				}
			}
		}

		private void cmbYear_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.cmbYear.Text.Length > 0) {
				string result = this.Calc(int.Parse(this.cmbYear.Text));
				this.txtResult.Text = result;
			}
		}

		private void btnMakeList_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(this.cmbYear.Text) || string.IsNullOrEmpty(this.cmbYearTo.Text)) {
				return;
			}

			int ss = int.Parse(this.cmbYear.Text);
			int ee = int.Parse(this.cmbYearTo.Text);
			if (ss > ee) {
				int wk = ss;
				ss = ee;
				ee = wk;
			}

			StringBuilder sb = new StringBuilder();

			for (int i = ss; i <= ee; i++) {
				string result = this.Calc(i);
				if (sb.Length > 0) {
					sb.Append("\r\n");
				}
				sb.Append(result);
			}

			this.txtResult.Text = sb.ToString();

			MessageBox.Show("done");
		}

		private string Calc(int pYear)
		{
			StringBuilder sb = new StringBuilder();
			JCalendars.JCalendar jc = new JCalendars.JCalendar();
			DateTime date = new DateTime(pYear, 1, 1);

			int days = 365;
			if (DateTime.IsLeapYear(pYear)) {
				days++;
			}

			for (int i = 0; i < days; i++) {
				string name;
				if (jc.IsHoliday(date, out name) != JCalendars.HolidayTypes.HEIJITU) {
					sb.Append(date.Year);
					sb.Append("/");
					sb.Append(date.Month);
					sb.Append("/");
					sb.Append(date.Day);
					sb.Append(' ');
					sb.Append(name);
					sb.Append("\r\n");
				}

				date = date.AddDays(1);
			}

			return sb.ToString();
		}
	}
}
