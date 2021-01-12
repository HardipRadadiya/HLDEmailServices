using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnverifiedBlast.Repository;

namespace UnverifiedBlast
{
    class Program
    {
        static void Main(string[] args)
        {

            EmailBlastBLL objemail = new EmailBlastBLL();
            objemail.SendWarmingEmails();

            //Methods.sendPhysicianAlert_NetBlast();        //HOLD FOR AFTER NURSES ARE SENT
            //Methods.sendPhysicianAlert_OrgBlast();        //HOLD FOR AFTER NURSES ARE SENT
            //Methods.sendPhysicianAlert_NetNurseBlast(); // This is Current
           // Methods.sendPhysicianAlert_OrgNurseBlast();
            //trishul did comment on 1oct2020 as we need to send only .org as we need to wait for .net blacklist if any comes in 5 days

        }
    }
}
