// Copyright (c) 2012, XBRL Cloud Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:
//
// Redistributions of source code must retain the above copyright notice,
// this list of conditions and the following disclaimer. Redistributions in
// binary form must reproduce the above copyright notice, this list of
// conditions and the following disclaimer in the documentation and/or
// other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS
// IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
// TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

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
