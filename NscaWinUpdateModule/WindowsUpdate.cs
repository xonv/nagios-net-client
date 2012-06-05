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
