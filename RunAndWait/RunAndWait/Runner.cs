using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using TaskScheduler;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Timers;



namespace RunAndWait
{
    class Runner
    {
        /// Here how to connect to remote machine to get all the task details ???????
        static ScheduledTasks st = null;

        static void ListTriggers(Task t)
        {
            Console.WriteLine("  " + t.ToString());
            foreach (Trigger tr in t.Triggers)
            {
                Console.WriteLine("  " + tr.ToString());
            }
        }

        static void ListTasks(ScheduledTasks st)
        {
            string[] taskNames = st.GetTaskNames();
            // Open each task, dump info to console
            foreach (string name in taskNames)
            {
                Task t = st.OpenTask(name);
                //Console.WriteLine("--> " + name + " ");
                if (t != null)
                {
                    Console.WriteLine(t.ToString() + "\n");
                    t.Close();
                }
            }
        }

        static void GoComputer(string machine)
        {
            try
            {
                if (machine == "")
                    st = new ScheduledTasks();
                else
                    st = new ScheduledTasks(machine);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not access task scheduler on " + machine +
                                  "\n  >> " + e.Message);
                st = null;
            }
        }

        static void ShowProperties(Task t)
        {
            Console.WriteLine(t.Name);
            try
            {
                Console.WriteLine("  Acct- {0}", t.AccountName);
            }
            catch (Exception e)
            {
                Console.WriteLine("  Acct- Exception: " + e.Message);
            }

            Console.WriteLine("  App- {0}", t.ApplicationName);
            Console.WriteLine("  Parms- {0}", t.Parameters);
            Console.WriteLine("  Comment- {0}", t.Comment);
            Console.WriteLine("  Creator- {0}", t.Creator);
            try
            {
                Console.WriteLine("  ExitCode- {0}", t.ExitCode);
            }
            catch (Exception e)
            {
                Console.WriteLine("  ExitCode- Exception: " + e.Message);
            }

            if (t.Hidden)
                Console.WriteLine("  Hidden");
            Console.WriteLine("  Flags- {0}", t.Flags);
            Console.WriteLine("  IdleWaitDeadline- {0}", t.IdleWaitDeadlineMinutes);
            Console.WriteLine("  IdleWait- {0}", t.IdleWaitMinutes);
            Console.WriteLine("  MaxRunTime- {0}", t.MaxRunTime);
            Console.WriteLine("  LastRun- {0}", t.MostRecentRunTime);
            Console.WriteLine("  NextRun- {0}", t.NextRunTime);
            Console.WriteLine("  Priority- {0}", t.Priority);
            Console.WriteLine("  Status- {0}", t.Status.ToString());
            Console.WriteLine("  WorkingDir- {0}", t.WorkingDirectory);
        }

        static Task CreateTask(string name)
        {
            Task t;
            try
            {
                t = st.CreateTask(name);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Task already exists");
                return null;
            }

            Console.Write("Appication Name: ");
            t.ApplicationName = Console.ReadLine();
            Console.Write("Parameters: ");
            t.Parameters = Console.ReadLine();
            Console.Write("Comment: ");
            t.Comment = Console.ReadLine();
            Console.Write("Creator: ");
            t.Creator = Console.ReadLine();
            Console.Write("Working Directory: ");
            t.WorkingDirectory = Console.ReadLine();
            Console.Write("Account: ");
            string acct = Console.ReadLine();
            if (acct == "")
            {
                t.SetAccountInformation(acct, (string) null);
            }
            else if (acct == Environment.UserName)
            {
                t.Flags = TaskFlags.RunOnlyIfLoggedOn;
                t.SetAccountInformation(acct, (string) null);
                Console.WriteLine("cur user is " + Environment.UserName + "; No password needed.");
            }
            else
            {
                Console.Write("Password: ");
                t.SetAccountInformation(acct, Console.ReadLine());
            }

            //t.Hidden = true;
            t.IdleWaitDeadlineMinutes = 20;
            t.IdleWaitMinutes = 10;
            t.MaxRunTime = new TimeSpan(1, 0, 0);
            t.Priority = System.Diagnostics.ProcessPriorityClass.High;
            t.Triggers.Add(new RunOnceTrigger(DateTime.Now + TimeSpan.FromMinutes(3.0)));
            t.Triggers.Add(new DailyTrigger(8, 30, 1));
            t.Triggers.Add(new WeeklyTrigger(6, 0, DaysOfTheWeek.Sunday));
            t.Triggers.Add(new MonthlyDOWTrigger(8, 0, DaysOfTheWeek.Monday | DaysOfTheWeek.Thursday,
                WhichWeek.FirstWeek | WhichWeek.ThirdWeek));
            int[] days = {1, 8, 15, 22, 29};
            t.Triggers.Add(new MonthlyTrigger(9, 0, days, MonthsOfTheYear.July));
            t.Triggers.Add(new OnIdleTrigger());
            t.Triggers.Add(new OnLogonTrigger());
            t.Triggers.Add(new OnSystemStartTrigger());
            return t;
        }


        // Return value: 0 means success, non-zero means failure/anomaly.
        static int Main(string[] args)
        {
            Task t = null;
            ;
            //string p_OutputDataReceived;
            st = new ScheduledTasks();
            ListTasks(st);
            Console.WriteLine();

            string sErrors = "";
            if (args.Length < 3) // We are expecting:  RunAndWait.exe  <MinsToWait>  <SuccessExitCode>  <ProgToRun>  ...
            {
                Usage();
                return 1;
            }

            try
            {
                Runner runner = new Runner();
                // runner.AppendToLogFile("\n\n--- " + GetProgramNameAndVersion() + " is starting. ---", true);
                runner.GetConfigInfo(ref sErrors);
                if (sErrors.Length > 0)
                {
                    Console.WriteLine(
                        "\nERROR:\nThe program could not read the required information from its configuration file.");
                    Console.WriteLine("Please consult the log file (" + runner.LogFilePathPlusName +
                                      ") for more information.");
                    return 3;
                }

                if (!runner.ProcessCmdLineArgs(args))
                {
                    // Something is wrong with the command-line arguments
                    Usage();
                    //SendEmail(0);
                    return 1;
                }

                StringBuilder sbResults = new StringBuilder();
                bool boOk = runner.RunTheProgram(ref sbResults);
                if (!boOk)
                {
                    Console.WriteLine("\nThe process did not run and complete within the specified time, results:\n" +
                                      sbResults);
                    return 2;
                }

                return 0; // success
            }
            catch (System.Exception e4)
            {
                Console.WriteLine("\nA fatal problem occurred:\n" + e4.ToString());
                return 5;
            }
        }

        private bool RunTheProgram(ref StringBuilder sbResults)
        {
            throw new NotImplementedException();
        }

        private bool ProcessCmdLineArgs(string[] args)
        {
            throw new NotImplementedException();
        }

        public object LogFilePathPlusName { get; set; }

        private void GetConfigInfo(ref string sErrors)
        {
            throw new NotImplementedException();
        }

        static private void Usage()
        {

            ProcessStartInfo psi = new ProcessStartInfo("SCHTASKS", "/QUERY /fo table");
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            Process p = Process.Start(psi);

            p.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);
            p.BeginOutputReadLine();



        }

        static void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            string strOutput = "";

            strOutput +=
                e.Data; /// Here i need to split the string into each indiviaul index and assign the task status to email object and need to send the email.???????



        }
    }
}
  