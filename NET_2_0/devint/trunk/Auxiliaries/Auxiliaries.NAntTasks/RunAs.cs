using System.Diagnostics;
using System.Security;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace Auxiliaries.NAntTasks
{
    [TaskName("RunAs")]
    public class RunasTask : Task
    {
        string program;
        string commandline;
        string domain = string.Empty;
        string username;
        string password;
        string workingdirectory;

        [TaskAttribute("program", Required = false)]
        public string Program
        {
            get { return program; }
            set { program = value; }
        }

        [TaskAttribute("commandline", Required = false)]
        public string CommandLine
        {
            get { return commandline; }
            set { commandline = value; }
        }

        [TaskAttribute("domain")]
        public string Domain
        {
            get { return domain; }
            set { domain = value; }
        }

        [TaskAttribute("username", Required = false)]
        public string UserName
        {
            get { return username; }
            set { username = value; }
        }

        [TaskAttribute("password", Required = false)]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        [TaskAttribute("workingdirectory", Required = false)]
        public string WorkingDirectory
        {
            get { return workingdirectory; }
            set { workingdirectory = value; }
        }

        protected override void ExecuteTask()
        {
            Log(Level.Info, "Runas running");
            Process process = new Process();
            process.StartInfo.FileName = Program;
            process.StartInfo.Arguments = CommandLine;
            process.StartInfo.Domain = Domain;
            process.StartInfo.UserName = UserName;
            process.StartInfo.Password = GetSecureString(Password);
            process.StartInfo.WorkingDirectory = WorkingDirectory;

            process.StartInfo.UseShellExecute = false;
            process.EnableRaisingEvents = true;

            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Exited += delegate
              {
                  Log(Level.Info, process.StandardOutput.ReadToEnd());
                  Log(Level.Error, process.StandardError.ReadToEnd());
                  Log(Level.Info, "Process complete.");
              };

            Log(Level.Info, "Process starting...");
            process.Start();
            process.WaitForExit();
            Log(Level.Info, "Runas complete.");
            string exitCodeMessage = string.Format("Exit code: {0}", process.ExitCode);
            Log(Level.Info, exitCodeMessage);
            if (process.ExitCode > 0)
            {
                throw new BuildException(exitCodeMessage);
            }
        }

        static SecureString GetSecureString(string s)
        {
            SecureString ss = new SecureString();
            for (int i = 0; i < s.Length; ++i)
            {
                //Log(Level.Info, s[i].ToString());
                ss.AppendChar(s[i]);
            }
            return ss;
        }
    }
}
