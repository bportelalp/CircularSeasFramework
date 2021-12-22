using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinCAT.Ads;

namespace Infrastructure.TcADS
{
    public class ClientADS : IClientADS
    {
        private AdsClient _adsClient = new AdsClient();

        public ClientADS()
        {
        }
        public bool Connected => _adsClient.IsConnected;

        #region Methods

        public bool Connect()
        {
            bool result = false;
            if (!_adsClient.IsConnected)
            {
                try
                {
                    _adsClient.Connect(AmsNetId.Local, 851);
                    if (_adsClient.IsConnected)
                        result = true;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            else
                result = true;
            return result;
        }

        public object Read(string address, Type type)
        {
            object value;
            try
            {
                value = _adsClient.ReadValue(address, type);
            }
            catch (Exception ex)
            {

                throw;
            }
            return value;

        }

        public void Write<T>(string address, T value)
        {
            try
            {
                if (_adsClient.IsConnected)
                {
                    _adsClient.WriteValue(address, value);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region Accessors
        #endregion
    }
}
