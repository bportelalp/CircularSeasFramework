using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinCAT.Ads;

namespace Infrastructure.TcADS
{
    public interface IClientADS
    {
        bool Connect();
        object Read(string address, Type type);
        void Write<T>(string address, T value);
    }
}
