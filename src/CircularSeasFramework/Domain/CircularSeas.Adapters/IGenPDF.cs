using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircularSeas.Adapters
{
    public interface IGenPDF
    {
        void CreatePdf(string dest);
        byte[] CreateSpoolPDF(Models.Order order);
    }
}
