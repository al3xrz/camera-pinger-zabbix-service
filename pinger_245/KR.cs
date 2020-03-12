using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace pinger_245
{
    [Serializable]
    public class KR
    {
        public string vsc_name = "";
        public string kr_name = "";
        public string kr_ip = "";
        public string zabbix_ip = "192.168.2.15";
        public int timeout = 10;
        public bool log = false;


        public event Action<string> OnError;
        private string setting_file = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\settings.xml";

        public KR GetKR()
        {
            KR kr = new KR();
            if (OnError != null) { OnError($"{Environment.CurrentDirectory}"); }
            XmlSerializer formatter = new XmlSerializer(typeof(KR));
            using (FileStream fs = new FileStream(setting_file, FileMode.OpenOrCreate))
            {
                try
                {
                    kr = (KR)formatter.Deserialize(fs);
                }
                catch (Exception ex)
                {
                    if (OnError != null) { OnError(ex.Message); }
                }

            }
            return kr;
        }

        public void SetKR(KR kr)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(KR));
            using (FileStream fs = new FileStream(setting_file, FileMode.Create))
            {
                try
                {
                    formatter.Serialize(fs, kr);
                }
                catch (Exception ex)
                {
                    if (OnError != null)
                    {
                        OnError(ex.Message);
                    }
                }
            }
        }
    }


}
