using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarmingBlast.Repository;

namespace WarmingBlast
{
    class Program
    {
        static void Main(string[] args)
        {

            EmailBlastBLL objemail = new EmailBlastBLL();
            objemail.SendWarmingEmails();
        }
    }
}
