using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace RainbowSimpleNantRunner
{
	public partial class Main : Form
	{
		Process process;

		public Main()
		{
			InitializeComponent();
		}

		void Main_Load(object sender, EventArgs e)
		{
			ConstructCommandLine();
		}

		void text_TextChanged(object sender, EventArgs e)
		{
			ConstructCommandLine();
		}
		
		void ConstructCommandLine()
		{
			textCommandLine.Text = textTarget.Text
				+ " -buildfile:" + textBuildFile.Text
				+ " -D:in.revision=" +textProperty_InRevision.Text
				+ " -D:in.branch=" + comboProperty_InBranch.Text 
				+ " -D:in.webAppTemplate=" + textProperty_InWebAppTemplate.Text
				+ " -logfile:" + textLogFile.Text;
		}

		void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			SafeAddData(textStandardOutput, e);
		}

		void process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
		{
			SafeAddData(textStandardError, e);
		}

		void SafeAddData(TextBox textBox, DataReceivedEventArgs e)
		{
			SafeAddLine(textBox, e.Data);
		}


		delegate void SafeSetTextCallback(TextBox textBox, string text);

		void SafeAddLine(TextBox textBox, string text)
		{
			if (textBox.InvokeRequired)
			{
				SafeSetTextCallback safeSetText = new SafeSetTextCallback(SafeAddLine);
				Invoke(safeSetText, new object[] { textBox, text });
			}
			else
			{
				List<string> list = new List<string>(textBox.Lines);
				list.Add(text);
				textBox.Lines = list.ToArray();
				textBox.SelectionStart = textBox.TextLength;
				textBox.ScrollToCaret();
			}
		}

		void buttonRun_Click(object sender, EventArgs e)
		{
			buttonRun.Enabled = false;
			buttonRun.ForeColor = Color.Red;
			buttonRun.Font = new Font(buttonRun.Font, FontStyle.Bold);
			textStandardOutput.BackColor = Color.LightPink;
			
			textStandardOutput.Text = string.Empty;
			textStandardError.Text = string.Empty;
			process = new Process();
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.FileName = "nant";
			process.StartInfo.Arguments = textCommandLine.Text;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.UseShellExecute = false;
			process.OutputDataReceived += new DataReceivedEventHandler(process_OutputDataReceived);
			process.ErrorDataReceived += new DataReceivedEventHandler(process_ErrorDataReceived);
			process.Exited += new EventHandler(process_Exited);
			process.EnableRaisingEvents = true;
			process.Start();
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();
		}

		void process_Exited(object sender, EventArgs e)
		{
			buttonRun.ForeColor = DefaultForeColor;
			buttonRun.Font = new Font(buttonRun.Font, FontStyle.Regular);
			textStandardOutput.BackColor = DefaultBackColor;
			SafeEnableButton();
		}

		delegate void SafeEnableButtonCallback();
		void SafeEnableButton()
		{
			if (buttonRun.InvokeRequired)
			{
				SafeEnableButtonCallback safeSetText = new SafeEnableButtonCallback(SafeEnableButton);
				Invoke(safeSetText, new object[] {});
			}
			else
			{
				buttonRun.Enabled = true;
			}
		}
	}
}