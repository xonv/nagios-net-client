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
using System.Text;
using WUApiLib;

namespace NscaWinUpdateModule
{
    internal class WindowsUpdate
    {
        internal static List<UpdateInfo> CheckUpdatesAvailable()
        {
            List<UpdateInfo> rslt = new List<UpdateInfo>();
            try
            {
                UpdateSession uSess = new UpdateSession();
                IUpdateSearcher uSearcher = uSess.CreateUpdateSearcher();
                ISearchResult searchResult = uSearcher.Search("IsInstalled=0 and Type='Software'");

                if (searchResult.Updates.Count > 0)
                {
                    foreach (IUpdate x in searchResult.Updates)
                    {
                        if (x.IsHidden == true)
                            continue;
                        UpdateInfo ui = new UpdateInfo();
                        ui.Description = x.Title;
                        foreach (ICategory cat in x.Categories)
                        {
                            switch(cat.Type)
                            {
                                case "UpdateClassification":
                                    ui.UpdateType = cat.Name;
                                    if (CheckingConstants.Critical == cat.Name)
                                    ui.Priority = 0;
                                    else if (CheckingConstants.Security == cat.Name)
                                        ui.Priority = 1;
                                    else if (CheckingConstants.Definition == cat.Name)
                                        ui.Priority = 2;
                                    else if (CheckingConstants.Updates == cat.Name)
                                        ui.Priority = 3;
                                    else if (CheckingConstants.Feature == cat.Name)
                                        ui.Priority = 4;
                                    else
                                        ui.Priority = 255;
                                    break;
                                case "Product":
                                    ui.Product = cat.Name;
                                    break;
                                case "ProductFamily" :
                                    ui.ProductFamily = cat.Name;
                                    break;
                                case "Company":
                                    ui.Company = cat.Name;
                                    break;
                            }
                        }

                        rslt.Add(ui);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("NscaWinUpdateModule: " + ex.Message);
            }

            return rslt;
        }
    }
}
