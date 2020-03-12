using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;


namespace pinger_245
{
    public partial class Pinger_245 : ServiceBase
    {

        
        private KR kr;
        private PingerSender ps;
        private Thread PingerThread;

        public Pinger_245(string[] args)
        {
            InitializeComponent();
            eventLog1 = new EventLog();
            if (!EventLog.SourceExists("Pinger_245"))
            {
                EventLog.CreateEventSource(
                    "Pinger_245", "Application");
            }
            eventLog1.Source = "Pinger_245";
            eventLog1.Log = "Application";

            kr = new KR();
            kr = kr.GetKR();
            
            ps = new PingerSender();
            ps.kr = kr;

            if (kr.log)
            {
                kr.OnError += eventLog1.WriteEntry;
                ps.OnError += eventLog1.WriteEntry;
                ps.OnNoPing += eventLog1.WriteEntry;
                ps.OnPing += eventLog1.WriteEntry;
                ps.OnSend += eventLog1.WriteEntry;
            }
            


        }

        protected override void OnStart(string[] args)
        {
            PingerThread = new Thread(new ThreadStart(ps.PingSend));
            PingerThread.Start();
        }

        protected override void OnStop()
        {
            //eventLog1.WriteEntry("Stoped");
            PingerThread.Abort();
        }

        protected override void OnContinue()
        {
            //eventLog1.WriteEntry("Continued");
        }

        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }
    }
}
