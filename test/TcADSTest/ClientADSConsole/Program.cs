using System;
using TwinCAT.Ads;

namespace ClientADSConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            AdsClient client = new AdsClient();
            try
            {
                client.Connect(AmsNetId.Local, 851);
            }
            catch (Exception ex)
            {
                var a = ex;
                throw;
            }

            if (client.IsConnected)
            {
                try
                {
                    bool result = (bool)client.ReadValue("MAIN.Variable", typeof(bool));

                    client.WriteValue("MAIN.Variable", true);
                }
                catch (Exception ex)
                {
                    var a = ex;
                }
            }
        }
    }
}
