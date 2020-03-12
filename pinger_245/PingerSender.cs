using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zabbix_Sender;

namespace pinger_245
{
    class PingerSender
    {
        public KR kr { get; set; }
        public event Action<string> OnError;
        public event Action<string> OnNoPing;
        public event Action<string> OnPing;
        public event Action<string> OnSend;
        //R-20-VSC440119076-KR-246

        public void PingSend()
        {

            Ping ping = new System.Net.NetworkInformation.Ping();

            PingReply pingReply = null;
            while (true)
            {
                string result_text = "";
                string zabbix_response = "";
                try
                {
                    pingReply = ping.Send(kr.kr_ip);
                    if (pingReply.Status != IPStatus.Success)
                    {
                        result_text = $"{kr.kr_name} {kr.kr_ip} {pingReply.Status.ToString()}";
                        if (OnNoPing != null) { OnNoPing(result_text); }
                        try
                        {
                            ZS_Request r = new ZS_Request($"{kr.kr_name}", "ping", "0");
                            zabbix_response = r.Send(kr.zabbix_ip).info;
                            if (OnSend != null) { OnSend(zabbix_response); }
                        }
                        catch (Exception ex)
                        {
                            if (OnError != null) OnError(ex.Message);
                        }
                    }
                    else
                    {
                        result_text = $"{kr.kr_name} {kr.kr_ip} {pingReply.Address.ToString()} {pingReply.Status} {(pingReply.RoundtripTime + 1).ToString()}ms";
                        if (OnPing != null) { OnPing(result_text); }
                        try
                        {
                            ZS_Request r = new ZS_Request($"{kr.kr_name}", "ping", (pingReply.RoundtripTime + 1).ToString());
                            zabbix_response = r.Send(kr.zabbix_ip).info;
                            if (OnSend != null) { OnSend(zabbix_response); }
                        }
                        catch (Exception ex)
                        {
                            if (OnError != null) OnError(ex.Message);
                        }

                    }
                }
                catch (PingException)
                {
                    if (OnNoPing != null) { OnNoPing(result_text); }
                    result_text = $"{kr.kr_name} {kr.kr_ip} {pingReply.Status.ToString()}";
                    try
                    {
                        ZS_Request r = new ZS_Request($"{kr.kr_name}", "ping", "0");
                        zabbix_response = r.Send(kr.zabbix_ip).info;
                        if (OnSend != null) { OnSend(result_text); }
                    }
                    catch (Exception ex)
                    {
                        if (OnError != null) OnError(ex.Message);
                    }
                }
                Thread.Sleep(kr.timeout * 1000);
            }
        }
    }
}
