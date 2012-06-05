using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NscaWinUpdateModule
{
    public class UpdateInfo
    {
        public byte Priority { get; set; }
        public string UpdateType { get; set; }
        public string Product { get; set; }
        public string ProductFamily { get; set; }
        public string Company { get; set; }
        public string Description { get; set; }

        public string GetMessage()
        {
            string msg = string.Format("{0}", Description);
            if (false == string.IsNullOrEmpty(Product))
                msg = string.Format("{0}, {1}", msg, this.Product);
            if (false == string.IsNullOrEmpty(ProductFamily))
                msg = string.Format("{0}, {1}", msg, this.ProductFamily);
            if (false == string.IsNullOrEmpty(Company))
                msg = string.Format("{0}, {1}", msg, this.Company);

            return msg;
        }
    }
}
