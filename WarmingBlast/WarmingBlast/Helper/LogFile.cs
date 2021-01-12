using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WarmingBlast.Helper
{
    public class LogFile
    {
        public static void AddMessage(string ExceptionMessage, string StackTrace)
        {
            try
            {
                string filename = "LogFile.txt";
                string path = System.Web.Hosting.HostingEnvironment.MapPath("~/") + filename;
                if (!File.Exists(path))
                {
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(DateTime.Now + ": ExceptionMessage: " + ExceptionMessage + Environment.NewLine);
                        sw.WriteLine(DateTime.Now + ": StackTrace: " + StackTrace + Environment.NewLine);
                    }
                }
                else
                {
                    File.AppendAllText(path, DateTime.Now + ": ExceptionMessage: " + ExceptionMessage + Environment.NewLine + DateTime.Now + ": StackTrace: " + StackTrace + Environment.NewLine);
                }

            }
            catch (Exception ex)
            {
            }
        }

        public static void AddMessage(string Message)
        {
            try
            {
                string filename = "LogFile.txt";
                string path = System.Web.Hosting.HostingEnvironment.MapPath("~/") + filename;
                if (!File.Exists(path))
                {
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(DateTime.Now + ": " + Message + Environment.NewLine);
                    }
                }
                else
                {
                    File.AppendAllText(path, DateTime.Now + ": " + Message + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
