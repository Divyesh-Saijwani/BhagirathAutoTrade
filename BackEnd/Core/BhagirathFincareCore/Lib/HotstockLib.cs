using System;
using System.Collections.Generic;
using System.Linq;
using BhagirathFincareClassLibrary.Models;
using System.Data.Entity;
using BhagirathFincareUtil;

namespace BhagirathFincareCore.Lib
{
    public partial class HotstockLib
    {
        public List<HotStock> GetHotStocks()
        {
            try
            {
                BhagirathFinCareEntities context = new BhagirathFinCareEntities();
                List<HotStock> list = context.HotStock.OrderByDescending(x => x.Id).ToList();
                return list;
            }
            catch (Exception ex)
            {
                ex.LogError(this);
                return null;
            }

        }

        public object AutoCompleteCompanyForHotstock(string companyName)
        {
            try
            {
                BhagirathFinCareEntities db = new BhagirathFinCareEntities();
                List<string> companyNames = new List<string>();

                companyNames =
                   db.MarketPrice.Where(x => x.Symbol.Contains(companyName) && x.FileType == 4)
                       .OrderBy(x => x.Symbol).Select(x => x.Symbol).Distinct()
                       .Take(50)
                       .ToList();
                return companyNames;
            }
            catch (Exception ex)
            {
                ex.LogError(this);
                return null;
            }
        }

        public bool SavehotStock(HotStock hotStock, out string message)
        {
            message = "";
            try
            {
                using (var context = new BhagirathFinCareEntities())
                {
                    if (context.HotStock.Where(x => x.CompanyName.Equals(hotStock.CompanyName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault() != null)
                    {
                        message = "AlreadyExists";
                        return false;
                    }
                    else
                    {
                        HotStock hotStockObj = context.HotStock.Add(hotStock);
                        context.SaveChanges();

                        if (hotStockObj.Id != 0 && hotStockObj.CompanyName.Equals(hotStock.CompanyName))
                        {
                            message = "Successfully Inserted";
                            return true;
                        }
                        else
                        {
                            message = "Fail to Insert";
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogError(this);
                return false;
            }
        }

        public bool DeletehotStock(int id)
        {
            try
            {
                using (var context = new BhagirathFinCareEntities())
                {
                    HotStock hotStockObj = context.HotStock.Where(x => x.Id == id).FirstOrDefault();
                    if (hotStockObj.Id != 0)
                    {
                        context.HotStock.Remove(hotStockObj);
                        context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogError(this);
                return false;
            }
        }

        public bool EdithotStock(HotStock hotstock)
        {
            try
            {
                using (var context = new BhagirathFinCareEntities())
                {
                    HotStock hotStock = context.HotStock.Find(hotstock.Id);
                    hotStock.CompanyName = hotstock.CompanyName;
                    context.Entry(hotStock).State = EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.LogError(this);
                return false;
            }
        }
    }
}
