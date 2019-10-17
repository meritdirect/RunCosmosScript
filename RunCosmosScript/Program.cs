using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RunCosmosScript
{
    using System.Data;
    using System.Data.SqlClient;
    using System.Text.RegularExpressions;
    using VcClient;

    class Program
    {
        public static string vc = string.Empty;
        public static string script_filename = string.Empty;
        public static string startdate = string.Empty;
        public static string enddate = string.Empty;

        //Pass nType as parameter to determin script lookup
        static void Main(string[] args)
        {

            FileElement fe = null;
            bool bHeader = false;


            LogError("Starting " + args[0]);
           
            if (args.Length == 0) {
                LogError("No parameter passed");
                System.Environment.Exit(1); // no parameters                
            }
            else
            {
                try { 
                fe = cosmosSetup.GetFiles(args[0]);                   
                vc = fe.VC;
                script_filename = fe.script;
                if (fe.excludeHeader == "1")
                {
                    bHeader = true;
                }
                    /* 
                    string baseStreamPath = fe.streamPath + DateTime.Now.ToString("yyyy-MM-dd") + '/';

                    var stream = VC.ReadStream(fe.streamPath + fe.filePrefix + "LastUpdate.txt", false);
                    byte[] buffer = new byte[2048];
                    stream.Read(buffer, 0, 2048);
                    stream.Close();
                    string LastDate = Encoding.ASCII.GetString(buffer, 0, buffer.Length).TrimEnd('\r', '\n', '\0');
                    DateTime dtmLastDate;
                    if (DateTime.TryParse(LastDate, out dtmLastDate) == true)
                        {                     
                            if (dtmLastDate.Date == DateTime.Now.AddDays(-1).Date)
                            {
                                LogError("Script " + fe.filePrefix + " already ran for " + dtmLastDate.ToShortDateString());
                                System.Environment.Exit(1); // already ran for date
                            }
                            startdate = dtmLastDate.AddDays(1).ToString("yyyy-MM-dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                        }
                    else
                        {
                            startdate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                        }
                        //getParameters(args[0]); // pass type as parameter to program

                    enddate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                    updateScript(); //udpates start and end dates, creates temp script file and sets script_filename

                    if (! File.Exists(script_filename))
                    {
                        LogError("Script file not found: " + script_filename);
                        System.Environment.Exit(1); // script does not exit
                    }
                    */

                }
                    catch(Exception ex)
                    {
                        LogError(ex.ToString());
                        System.Environment.Exit(1); // error processing parameters
                    }
                    } //end else
                try
                {
                    VcClient.VC.Setup(vc, VcClient.VC.NoProxy, null);
                }
                catch (Exception ex)
                {
                    LogError("Error connecting to VC:");
                    LogError(ex.ToString());
                    System.Environment.Exit(1); // Could not connect to VC
                }

                try
                {
                /*
                    var subParams = new ScopeClient.SubmitParameters(script_filename);
                    ScopeClient.ScopeEnvironment.Instance.WorkingRoot = System.IO.Path.GetTempPath();
                    var jobinfo = ScopeClient.Scope.Submit(subParams);

                    // Wait
                    WaitUntilJobFinished(jobinfo);
                    switch (VcClient.VC.GetJobInfo(jobinfo.ID, true).State)
                    {
                        case JobInfo.JobState.CompletedFailure:
                            LogError("Job Failed " + fe.filePrefix);
                            System.Environment.Exit(1); // Job Failed
                            break;
                        case JobInfo.JobState.Cancelled:
                            LogError("Job Cancelled " + fe.filePrefix);
                            System.Environment.Exit(1); // Job Cancelled
                            break;
                        case JobInfo.JobState.CompletedSuccess:
                        case JobInfo.JobState.Completed:
                            LogError("Job Completed " + fe.filePrefix);
                            break;
                        default:                
                            LogError("Job Completed Other " + fe.filePrefix);
                            System.Environment.Exit(1); // Job Completed
                            break;
                    }
                    
                    // At this point the job is finished
                    */
                    string baseStreamPath = fe.streamPath + DateTime.Now.ToString("yyyy-MM-dd") + '/';

                //var stream = VC.ReadStream(fe.streamPath + fe.filePrefix + "LastUpdate.txt", false);
                var stream = VC.ReadStream(fe.streamPath + fe.updateFile, false);
                byte[] buffer = new byte[2048];
                stream.Read(buffer, 0, 2048);
                stream.Close();
                string newLastDate = Encoding.ASCII.GetString(buffer, 0, buffer.Length).TrimEnd('\r', '\n', '\0');
                DateTime dtmNewLastDate;
                if(fe.renameFile == "1") { 
                if (DateTime.TryParse(newLastDate, out dtmNewLastDate) == true)
                {
                    newLastDate = dtmNewLastDate.ToString("yyyyMMdd");
                    if (newLastDate != enddate.Replace("-", ""))
                    {
                        VC.Rename(baseStreamPath + fe.filePrefix + startdate.Replace("-", "") + '_' + enddate.Replace("-", "") + ".ss", baseStreamPath + fe.filePrefix + startdate.Replace("-", "") + '_' + newLastDate + ".ss");
                    }
                }
                }

                // Add actual last update date to output filename
                foreach (var streamPath in GetStreamsRecurse(baseStreamPath, new Regex(fe.filePrefix + @".*\.ss$")))
                {
                    var uri = new Uri(streamPath);
                    var relativeStreamPath = uri.Segments[uri.Segments.Length - 1];
                    var fullCosmosPath = Path.Combine(baseStreamPath, relativeStreamPath);
                    
                    DownloadCosmos.DownloadFile(baseStreamPath, relativeStreamPath.Replace(".ss",""), fe.downloadDirectory, bHeader);

                    //VC.Download(fullCosmosPath, fullDiskPath, true, DownloadMode.OverWrite);
                }
                //DownloadCosmos.DownloadFile(fe.streamPath + DateTime.Now.ToString("yyyyMMdd") + '/', fe.filePrefix + DateTime.Now.ToString("yyyyMMdd"), "C:\\temp\\");
            }
            catch (VcClientExceptions.VcClientException ex)
            {
                LogError(ex.ToString());
            }
            catch (Exception ex)
            {
                LogError(ex.ToString());
            }
            
            File.Delete(script_filename); //delete temporary file
          //Call proc for success or failure
        } //end Main

        private static IEnumerable<string> GetStreamsRecurse(string baseDirectory, Regex regex)
        {
            foreach (var streamInfo in VC.GetDirectoryInfo(baseDirectory, true))
            {
                if (streamInfo.IsDirectory)
                {
                    foreach (var subStream in GetStreamsRecurse(streamInfo.StreamName, regex))
                    {
                        yield return subStream;
                    }
                }
                else if (regex.IsMatch(streamInfo.StreamName))
                {
                    yield return streamInfo.StreamName;
                }
            }
        }

        private static void WaitUntilJobFinished(JobInfo jobinfo)
        {
            // The submission is done. Now we wait until the job is done
            bool use_compression = true;
            int seconds_to_sleep = 5;
            var wait_time = new System.TimeSpan(0, 0, 0, seconds_to_sleep);
            while (true)
            {
                jobinfo = VcClient.VC.GetJobInfo(jobinfo.ID, use_compression);
                Console.WriteLine("Job State = {0}", jobinfo.State);
                if (jobinfo.State == VcClient.JobInfo.JobState.Cancelled || jobinfo.State == VcClient.JobInfo.JobState.Completed
                    || jobinfo.State == VcClient.JobInfo.JobState.CompletedFailure
                    || jobinfo.State == VcClient.JobInfo.JobState.CompletedSuccess)
                {
                    LogError("Job Stopped Running");
                    break;
                }

                System.Threading.Thread.Sleep(wait_time);
            }
        }

        private static bool UploadInputStream(string data_filename, string remote_path)
        {
            bool use_compression = true;

            string data = @"FOO,1
BAR,3
FOO,7
BAR,10
FOO,1
BAR,9";

            data = data.Trim().Replace(",", "\t");
            System.IO.File.WriteAllText(data_filename, data);

            if (VcClient.VC.StreamExists(remote_path))
            {
                VcClient.VC.Delete(remote_path);
            }
            VcClient.VC.Upload(data_filename, remote_path, use_compression);
            return use_compression;
        }
        private static void LogError(string sText)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss") + ' ' + sText);
            File.AppendAllText(Properties.Settings.Default.logPath + "RunCosmosScript.txt", sb.ToString() + "\r\n");
            sb.Clear();
        }
        private static void getParameters(string sType)
        {
            using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.ConnectionString))
            {

                conn.Open();
                //Get parameters from the database
                SqlCommand Command = new SqlCommand("p_MeritCosmos_GetParameters", conn);
                Command.CommandType = System.Data.CommandType.StoredProcedure;
                Command.Parameters.AddWithValue("@sType", sType);                
                Command.Parameters.Add("@StartDate", SqlDbType.VarChar,250).Direction = ParameterDirection.Output;
                Command.Parameters.Add("@EndDate", SqlDbType.VarChar, 250).Direction = ParameterDirection.Output;
                Command.ExecuteNonQuery();
                
                if (Command.Parameters["@StartDate"].Value != null)
                {
                    startdate = Command.Parameters["@StartDate"].Value.ToString();
                }
                if (Command.Parameters["@EndDate"].Value != null)
                {
                    enddate = Command.Parameters["@EndDate"].Value.ToString();
                }
                //Read script file and replace any date parameters
                string fileName = Path.GetDirectoryName(script_filename) + '\\' + Guid.NewGuid().ToString() + ".script";
                string text = File.ReadAllText(script_filename);
                if (startdate.Length > 0)
                {                    
                 
                    text = text.Replace("#StartDate", startdate);
                    text = text.Replace("#EndDate", enddate);
                    
                }
                File.WriteAllText(fileName, text);
                script_filename = fileName;
            }
        }
        private static void updateScript()
        {
            string fileName = Path.GetDirectoryName(script_filename) + '\\' + Guid.NewGuid().ToString() + ".script";
            string text = File.ReadAllText(script_filename);
            if (startdate.Length > 0)
            {

                text = text.Replace("#StartDate", startdate);
                text = text.Replace("#EndDate", enddate);

            }
            File.WriteAllText(fileName, text);
            script_filename = fileName;
        }
        
    }
    
}
