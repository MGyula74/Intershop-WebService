// Decompiled with JetBrains decompiler
// Type: IntershopWebService
// Assembly: IntershopWebService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6A055958-F609-4E74-9C8B-6034BADC5D94
// Assembly location: C:\public\Projects\DLLs\Intershop\IntershopWebService.dll

using Microsoft.Dynamics.BusinessConnectorNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Web.Script.Services;
using System.Web.Services;
using System.Xml;
using System.Xml.Serialization;

namespace IntershopWebService
{
    [WebService(Namespace = "http://bunzl.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]
    public class IntershopWebService : WebService
    {
        public List<IntershopWebService.SizeStock> getItemDims(string itemId, string company)
        {
            List<IntershopWebService.SizeStock> sizeStockList = new List<IntershopWebService.SizeStock>();
            Axapta axapta = (Axapta)null;
            try
            {
                axapta = this.axLogon(company);
                string str = "select * from %1 where %1.ItemId == '" + itemId + "'";
                AxaptaRecord axaptaRecord = axapta.CreateAxaptaRecord("InventSize");
                axaptaRecord.ExecuteStmt(str);
                while (axaptaRecord.Found)
                {
                    IntershopWebService.SizeStock sizeStock = new IntershopWebService.SizeStock();
                    if (Convert.ToBoolean(axaptaRecord.get_Field("Color")))
                        sizeStock.color = Convert.ToString(axaptaRecord.get_Field("InventSizeId"));
                    else
                        sizeStock.size = Convert.ToString(axaptaRecord.get_Field("InventSizeId"));
                    sizeStockList.Add(sizeStock);
                    axaptaRecord.Next();
                }
                return sizeStockList;
            }
            catch (Exception ex)
            {
                return (List<IntershopWebService.SizeStock>)null;
            }
            finally
            {
                axapta.Logoff();
            }
        }

        [WebMethod]
        public string GetItemStock(string itemId, string company)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Axapta axapta = (Axapta)null;
            try
            {
                axapta = this.axLogon(company);
                object obj = axapta.CallStaticClassMethod("BTX_ISHItemDescriptionProxy", "GetOnHandUrl", (object)itemId, (object)string.Empty);
                stopwatch.Stop();
                MessageLogger.info(string.Format("GetItemStock({0}, {1}) -> {2} , exec time -> {3} ms", (object)itemId, (object)company, (object)Convert.ToString(obj), (object)stopwatch.ElapsedMilliseconds));
                return Convert.ToString(obj);
            }
            catch (Exception ex)
            {
                return "An error occured: " + ex.Message;
            }
            finally
            {
                axapta.Logoff();
            }
        }

        [WebMethod]
        public string GetItemStockSize(string itemId, string size, string company)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Axapta axapta = (Axapta)null;
            try
            {
                axapta = this.axLogon(company);
                object obj = axapta.CallStaticClassMethod("BTX_ISHItemDescriptionProxy", "GetOnHandUrl", (object)itemId, (object)size);
                stopwatch.Stop();
                MessageLogger.info(string.Format("GetItemStock({0}, {1}, {2}) -> {3} , exec time -> {5} ms", (object)itemId, (object)size, (object)company, (object)Convert.ToString(obj), (object)stopwatch.ElapsedMilliseconds));
                return Convert.ToString(obj);
            }
            catch (Exception ex)
            {
                return "An error occured: " + ex.Message;
            }
            finally
            {
                axapta.Logoff();
            }
        }

        [WebMethod]
        [XmlInclude(typeof(IntershopWebService.ProductStock))]
        public List<IntershopWebService.ProductStock> GetItemsStock(List<IntershopWebService.ProductStockIn> products, string company)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            MessageLogger.info(string.Format("GetItemsStock(count={0}, company={1})", (object)products.Count, (object)company));
            Axapta axapta = (Axapta)null;
            List<IntershopWebService.ProductStock> productStockList = new List<IntershopWebService.ProductStock>();
            try
            {
                axapta = this.axLogon(company);
                foreach (IntershopWebService.ProductStockIn product in products)
                {
                    string str = Convert.ToString(axapta.CallStaticClassMethod("BTX_ISHItemDescriptionProxy", "GetOnHandUrl", (object)product.itemId, (object)string.Empty));
                    IntershopWebService.ProductStock productStock = new IntershopWebService.ProductStock();
                    productStock.itemId = product.itemId;
                    productStock.stockLevel = str;
                    productStockList.Add(productStock);
                    stopwatch.Stop();
                    MessageLogger.info(string.Format("GetItemsStock({0}, {1}, {2}, {3}) -> {4} , exec time -> {5} ms", (object)product.itemId, (object)product.size, (object)product.color, (object)company, (object)productStock.stockLevel, (object)stopwatch.ElapsedMilliseconds));
                }
                return productStockList;
            }
            catch (Exception ex)
            {
                return (List<IntershopWebService.ProductStock>)null;
            }
            finally
            {
                axapta.Logoff();
            }
        }

        protected string normalizeXmlString(string strIn)
        {
            return strIn.Replace("\n", string.Empty).Replace("\r", string.Empty).Replace(" ", string.Empty);
        }

        [WebMethod]
        [XmlInclude(typeof(IntershopWebService.ProductStock))]
        public List<IntershopWebService.ProductStock> GetItemsStockSize(List<IntershopWebService.ProductStockIn> products, string company)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            MessageLogger.info(string.Format("GetItemsStockSize(count={0}, company={1})", (object)products.Count, (object)company));
            Axapta axapta = (Axapta)null;
            List<IntershopWebService.ProductStock> productStockList = new List<IntershopWebService.ProductStock>();
            try
            {
                axapta = this.axLogon(company);
                foreach (IntershopWebService.ProductStockIn product in products)
                {
                    IntershopWebService.ProductStockIn productIn = product;
                    IntershopWebService.ProductStock productStock = new IntershopWebService.ProductStock();
                    productStock.itemId = productIn.itemId;
                    string str1 = string.Empty;
                    productIn.size = this.normalizeXmlString(productIn.size);
                    productIn.color = this.normalizeXmlString(productIn.color);
                    List<IntershopWebService.SizeStock> itemDims = this.getItemDims(productIn.itemId, company);
                    if (itemDims.Count > 0 && (!string.IsNullOrEmpty(productIn.size) || !string.IsNullOrEmpty(productIn.color)))
                    {
                        if (itemDims.FindIndex((Predicate<IntershopWebService.SizeStock>)(f => f.size == productIn.size)) >= 0)
                            str1 = productIn.size;
                        if (itemDims.FindIndex((Predicate<IntershopWebService.SizeStock>)(f => f.color == productIn.color)) >= 0)
                            str1 = productIn.color;
                    }
                    string str2 = Convert.ToString(axapta.CallStaticClassMethod("BTX_ISHItemDescriptionProxy", "GetOnHandUrl", (object)productIn.itemId, (object)str1));
                    productStock.stockLevel = str2;
                    productStockList.Add(productStock);
                    stopwatch.Stop();
                    MessageLogger.info(string.Format("GetItemsStockSize({0}, {1}, {2}, {3}) -> {4}, exec time: {5} ms", (object)productIn.itemId, (object)productIn.size, (object)productIn.color, (object)company, (object)productStock.stockLevel, (object)stopwatch.ElapsedMilliseconds));
                }
                return productStockList;
            }
            catch (Exception ex)
            {
                return (List<IntershopWebService.ProductStock>)null;
            }
            finally
            {
                axapta.Logoff();
            }
        }

        [WebMethod]
        public IntershopWebService.ProductPrice GetItemPrice(string itemId, string customerID, string Unit, string company)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Axapta ax = this.axLogon(company);
            IntershopWebService.ProductPrice productPrice = new IntershopWebService.ProductPrice();
            try
            {
                double num = Convert.ToDouble(ax.CallStaticClassMethod("BTX_ISHItemDescriptionProxy", "GetSalesPrice", new object[4]
                {
          (object) customerID,
          (object) itemId,
          (object) Unit,
          (object) ""
                }));
                productPrice.itemId = itemId;
                productPrice.price = num;
                productPrice.miscCharges = this.ExtractMiscCharges(ax, productPrice.itemId, customerID, productPrice.price, 1, Unit);
                stopwatch.Stop();
                MessageLogger.info(string.Format("GetItemPrice({0}, {1}, {2}, {3}) -> {4} , exec time -> {5} ms", (object)itemId, (object)customerID, (object)Unit, (object)company, (object)productPrice.price, (object)stopwatch.ElapsedMilliseconds));
                return productPrice;
            }
            catch (Exception ex)
            {
                return (IntershopWebService.ProductPrice)null;
            }
            finally
            {
                ax.Logoff();
            }
        }

        [WebMethod]
        [XmlInclude(typeof(IntershopWebService.ProductPrice))]
        [XmlInclude(typeof(IntershopWebService.ProductPriceIn))]
        public List<IntershopWebService.ProductPrice> GetItemsPrice(List<IntershopWebService.ProductPriceIn> products, string company, string customerID)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            MessageLogger.info(string.Format("GetItemsPrice(count={0}, custaccount={1}, company={2})", (object)products.Count, (object)customerID, (object)company));
            Axapta ax = this.axLogon(company);
            List<IntershopWebService.ProductPrice> productPriceList = new List<IntershopWebService.ProductPrice>();
            try
            {
                foreach (IntershopWebService.ProductPriceIn product in products)
                {
                    object obj = ax.CallStaticClassMethod("BTX_ISHItemDescriptionProxy", "GetSalesPrice", (object)customerID, (object)product.itemId, (object)product.unitId);
                    IntershopWebService.ProductPrice productPrice = new IntershopWebService.ProductPrice()
                    {
                        itemId = product.itemId,
                        price = Math.Round(Convert.ToDouble(obj), 2)
                    };
                    productPrice.miscCharges = this.ExtractMiscCharges(ax, productPrice.itemId, customerID, productPrice.price, (int)product.quantity, product.unitId);
                    stopwatch.Stop();
                    MessageLogger.info(string.Format("GetItemsPrice({0}, {1}, {2}) -> {3} , exec time -> {4} ms", (object)productPrice.itemId, (object)customerID, (object)company, (object)productPrice.price, (object)stopwatch.ElapsedMilliseconds));
                    productPriceList.Add(productPrice);
                }
                return productPriceList;
            }
            catch (Exception ex)
            {
                return (List<IntershopWebService.ProductPrice>)null;
            }
            finally
            {
                ax.Logoff();
            }
        }

        [WebMethod]
        public IntershopWebService.ProductPrice GetItemPriceSize(string itemId, string customerID, string Unit, string size, string company)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Axapta ax = this.axLogon(company);
            IntershopWebService.ProductPrice productPrice = new IntershopWebService.ProductPrice();
            try
            {
                double salesPrice;
                string priceTypeStr;
                this.ExtractPriceValues((AxaptaContainer)ax.CallStaticClassMethod("BTX_ISHItemDescriptionProxy", "GetPriceType", new object[4]
                {
          (object) customerID,
          (object) itemId,
          (object) Unit,
          (object) size
                }), out salesPrice, out priceTypeStr);
                productPrice.itemId = itemId;
                productPrice.price = salesPrice;
                productPrice.priceType = priceTypeStr;
                productPrice.miscCharges = this.ExtractMiscCharges(ax, productPrice.itemId, customerID, productPrice.price, 1, size, Unit);
                stopwatch.Stop();
                MessageLogger.info(string.Format("GetItemPrice({0}, {1}, {2}, {3}) -> {4} , exec time -> {5} ms", (object)itemId, (object)customerID, (object)Unit, (object)company, (object)productPrice.price, (object)stopwatch.ElapsedMilliseconds));
                return productPrice;
            }
            catch (Exception ex)
            {
                return (IntershopWebService.ProductPrice)null;
            }
            finally
            {
                ax.Logoff();
            }
        }

        [WebMethod]
        [XmlInclude(typeof(IntershopWebService.ProductPrice))]
        [XmlInclude(typeof(IntershopWebService.ProductPriceIn))]
        public List<IntershopWebService.ProductPrice> GetItemsPriceSize(List<IntershopWebService.ProductPriceIn> products, string company, string customerID)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            MessageLogger.info(string.Format("GetItemsPriceSize(count={0}, custaccount={1}, company={2})", (object)products.Count, (object)customerID, (object)company));
            Axapta ax = this.axLogon(company);
            List<IntershopWebService.ProductPrice> productPriceList = new List<IntershopWebService.ProductPrice>();
            try
            {
                foreach (IntershopWebService.ProductPriceIn product in products)
                {
                    IntershopWebService.ProductPriceIn productIn = product;
                    IntershopWebService.ProductPrice productPrice = new IntershopWebService.ProductPrice();
                    productPrice.itemId = productIn.itemId;
                    string inventSizeId = string.Empty;
                    productIn.size = this.normalizeXmlString(productIn.size);
                    productIn.color = this.normalizeXmlString(productIn.color);
                    List<IntershopWebService.SizeStock> itemDims = this.getItemDims(productIn.itemId, company);
                    if (itemDims.Count > 0 && (!string.IsNullOrEmpty(productIn.size) || !string.IsNullOrEmpty(productIn.color)))
                    {
                        if (itemDims.FindIndex((Predicate<IntershopWebService.SizeStock>)(f => f.size == productIn.size)) >= 0)
                            inventSizeId = productIn.size;
                        if (itemDims.FindIndex((Predicate<IntershopWebService.SizeStock>)(f => f.color == productIn.color)) >= 0)
                            inventSizeId = productIn.color;
                    }
                    double salesPrice;
                    string priceTypeStr;
                    this.ExtractPriceValues((AxaptaContainer)ax.CallStaticClassMethod("BTX_ISHItemDescriptionProxy", "GetPriceType", new object[4]
                    {
            (object) customerID,
            (object) productIn.itemId,
            (object) productIn.unitId,
            (object) inventSizeId
                    }), out salesPrice, out priceTypeStr);
                    productPrice.price = salesPrice;
                    productPrice.priceType = priceTypeStr;
                    productPrice.miscCharges = this.ExtractMiscCharges(ax, productPrice.itemId, customerID, productPrice.price, 1, inventSizeId, productIn.unitId);
                    stopwatch.Stop();
                    MessageLogger.info(string.Format("GetItemsPriceSize({0}, {1}, {2}, {3}, {4}) -> {5}, exec time -> {6} ms", (object)productIn.itemId, (object)productIn.size, (object)productIn.color, (object)customerID, (object)company, (object)productPrice.price, (object)stopwatch.ElapsedMilliseconds));
                    productPriceList.Add(productPrice);
                }
                return productPriceList;
            }
            catch (Exception ex)
            {
                return (List<IntershopWebService.ProductPrice>)null;
            }
            finally
            {
                ax.Logoff();
            }
        }

        [WebMethod]
        public IntershopWebService.TrackInfo GetTrackInfo(string company, string customerID, string salesID)
        {
            MessageLogger.info(string.Format("GetTrackInfo(company={0}, customerID={1}, salesID={2})", (object)company, (object)customerID, (object)salesID));
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if (string.IsNullOrEmpty(customerID) || string.IsNullOrEmpty(salesID))
                return new IntershopWebService.TrackInfo()
                {
                    ErrorMsg = "Please populate both lookup fields.",
                    ErrorCode = IntershopWebService.TrackInfoError.ERROR_NO_PARAM
                };
            Axapta axapta = this.axLogon(company);
            try
            {
                IntershopWebService.TrackInfo trackInfo = this.extractTrackInfo(Convert.ToString(axapta.CallStaticClassMethod("BTX_SharedFunctionsProxy", "getTrackAndTraceLinks", (object)customerID, (object)salesID)));
                stopwatch.Stop();
                MessageLogger.info(string.Format("GetTrackInfo() -> {0} ms", (object)stopwatch.ElapsedMilliseconds));
                return trackInfo;
            }
            catch (Exception ex)
            {
                MessageLogger.info(string.Format("GetTrackInfo() -> error: {0}", (object)ex.Message));
                return (IntershopWebService.TrackInfo)null;
            }
            finally
            {
                axapta.Logoff();
            }
        }

        [WebMethod]
        public double OrderTotalAmount(string customerID, List<IntershopWebService.IshOrderLine> orderLines, string company)
        {
            MessageLogger.info(string.Format("OrderTotalAmount(company={0}, customerID={1})", (object)company, (object)customerID));
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Axapta axapta = this.axLogon(company);
            try
            {
                AxaptaContainer axaptaContainer1 = axapta.CreateAxaptaContainer();
                foreach (IntershopWebService.IshOrderLine orderLine in orderLines)
                {
                    IntershopWebService.IshOrderLine productIn = orderLine;
                    string str = string.Empty;
                    productIn.size = this.normalizeXmlString(productIn.size);
                    productIn.color = this.normalizeXmlString(productIn.color);
                    List<IntershopWebService.SizeStock> itemDims = this.getItemDims(productIn.ItemID, company);
                    if (itemDims.Count > 0 && (!string.IsNullOrEmpty(productIn.size) || !string.IsNullOrEmpty(productIn.color)))
                    {
                        if (itemDims.FindIndex((Predicate<IntershopWebService.SizeStock>)(f => f.size == productIn.size)) >= 0)
                            str = productIn.size;
                        if (itemDims.FindIndex((Predicate<IntershopWebService.SizeStock>)(f => f.color == productIn.color)) >= 0)
                            str = productIn.color;
                    }
                    AxaptaContainer axaptaContainer2 = axapta.CreateAxaptaContainer();
                    axaptaContainer2.Add((object)productIn.ItemID);
                    axaptaContainer2.Add((object)productIn.Qty);
                    axaptaContainer2.Add((object)productIn.SalesUnit);
                    axaptaContainer2.Add((object)str);
                    axaptaContainer1.Add((object)axaptaContainer2);
                }
                double num = Convert.ToDouble(axapta.CallStaticClassMethod("BTX_ISHItemDescriptionProxy", "CalcTotalAmount", (object)customerID, (object)axaptaContainer1));
                stopwatch.Stop();
                MessageLogger.info(string.Format("OrderTotalAmount() -> {0} ms", (object)stopwatch.ElapsedMilliseconds));
                return num;
            }
            catch (Exception ex)
            {
                MessageLogger.info(string.Format("OrderTotalAmount() -> error: {0}", (object)ex.Message));
                return 0.0;
            }
            finally
            {
                axapta.Logoff();
            }
        }

        [WebMethod]
        public IntershopWebService.TotalDetails OrderTotalAmountDetails(string customerID, List<IntershopWebService.IshOrderLine> orderLines, string IshBasketID, string company)
        {
            MessageLogger.info(string.Format("OrderTotalAmountDetails(company={0}, customerID={1})", (object)company, (object)customerID));
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Axapta ax = this.axLogon(company);
            ax.Refresh();
            try
            {
                AxaptaContainer axaptaContainer1 = ax.CreateAxaptaContainer();
                foreach (IntershopWebService.IshOrderLine orderLine in orderLines)
                {
                    IntershopWebService.IshOrderLine productIn = orderLine;
                    string str = string.Empty;
                    productIn.size = this.normalizeXmlString(productIn.size);
                    productIn.color = this.normalizeXmlString(productIn.color);
                    List<IntershopWebService.SizeStock> itemDims = this.getItemDims(productIn.ItemID, company);
                    if (itemDims.Count > 0 && (!string.IsNullOrEmpty(productIn.size) || !string.IsNullOrEmpty(productIn.color)))
                    {
                        if (itemDims.FindIndex((Predicate<IntershopWebService.SizeStock>)(f => f.size == productIn.size)) >= 0)
                            str = productIn.size;
                        if (itemDims.FindIndex((Predicate<IntershopWebService.SizeStock>)(f => f.color == productIn.color)) >= 0)
                            str = productIn.color;
                    }
                    AxaptaContainer axaptaContainer2 = ax.CreateAxaptaContainer();
                    axaptaContainer2.Add((object)productIn.ItemID);
                    axaptaContainer2.Add((object)productIn.Qty);
                    axaptaContainer2.Add((object)productIn.SalesUnit);
                    axaptaContainer2.Add((object)str);
                    axaptaContainer1.Add((object)axaptaContainer2);
                }
                AxaptaContainer totalContainer = (AxaptaContainer)ax.CallStaticClassMethod("BTX_ISHItemDescriptionProxy", "CalcTotalAmountDetails", (object)customerID, (object)axaptaContainer1, (object)IshBasketID);
                stopwatch.Stop();
                MessageLogger.info(string.Format("OrderTotalAmount() -> {0} ms", (object)stopwatch.ElapsedMilliseconds));
                return this.extractTotalDetails(ax, totalContainer);
            }
            catch (Exception ex)
            {
                MessageLogger.info(string.Format("OrderTotalAmount() -> error: {0}", (object)ex.Message));
                return (IntershopWebService.TotalDetails)null;
            }
            finally
            {
                ax.Logoff();
            }
        }

        private IntershopWebService.TotalDetails extractTotalDetails(Axapta ax, AxaptaContainer totalContainer)
        {
            IEnumerator enumerator = totalContainer.GetEnumerator();
            IntershopWebService.TotalDetails totalDetails = new IntershopWebService.TotalDetails();
            int num = 0;
            while (enumerator.MoveNext())
            {
                switch (num++)
                {
                    case 0:
                        totalDetails.IshBasketID = Convert.ToString(enumerator.Current);
                        break;
                    case 1:
                        totalDetails.AxBasketID = Convert.ToString(enumerator.Current);
                        break;
                    case 2:
                        totalDetails.TotalAmount = Math.Round(Convert.ToDouble(enumerator.Current), 2);
                        break;
                    case 3:
                        totalDetails.Currency = Convert.ToString(enumerator.Current);
                        break;
                    case 4:
                        totalDetails.TotalNetAmount = Math.Round(Convert.ToDouble(enumerator.Current), 2);
                        break;
                    case 5:
                        totalDetails.VatTotal = Math.Round(Convert.ToDouble(enumerator.Current), 2);
                        break;
                    case 6:
                        totalDetails.MiscCharges = this.ExtractMiscChargesTotal(ax, (AxaptaContainer)enumerator.Current);
                        break;
                    case 7:
                        totalDetails.Discounts = this.ExtractDiscountsTotal(ax, (AxaptaContainer)enumerator.Current);
                        break;
                    case 8:
                        totalDetails.OrderLines = this.ExtractOrderLinesTotal(ax, (AxaptaContainer)enumerator.Current);
                        break;
                }
            }
            return totalDetails;
        }

        private List<IntershopWebService.TotalDetailsLine> ExtractOrderLinesTotal(Axapta ax, AxaptaContainer orderLineCont)
        {
            List<IntershopWebService.TotalDetailsLine> totalDetailsLineList = new List<IntershopWebService.TotalDetailsLine>();
            IEnumerator enumerator1 = orderLineCont.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                IntershopWebService.TotalDetailsLine totalDetailsLine = new IntershopWebService.TotalDetailsLine();
                IEnumerator enumerator2 = ((AxaptaContainer)enumerator1.Current).GetEnumerator();
                int num = 0;
                while (enumerator2.MoveNext())
                {
                    switch (num++)
                    {
                        case 0:
                            totalDetailsLine.ItemID = Convert.ToString(enumerator2.Current);
                            break;
                        case 1:
                            totalDetailsLine.Qty = Math.Round(Convert.ToDouble(enumerator2.Current), 2);
                            break;
                        case 2:
                            totalDetailsLine.SalesUnit = Convert.ToString(enumerator2.Current);
                            break;
                        case 3:
                            totalDetailsLine.UnitPrice = Math.Round(Convert.ToDouble(enumerator2.Current), 2);
                            break;
                        case 4:
                            totalDetailsLine.LineNetAmount = Math.Round(Convert.ToDouble(enumerator2.Current), 2);
                            break;
                        case 5:
                            totalDetailsLine.Discounts = this.ExtractDiscountsTotalLine(ax, (AxaptaContainer)enumerator2.Current);
                            break;
                        case 6:
                            totalDetailsLine.MiscCharges = this.ExtractMiscChargesTotal(ax, (AxaptaContainer)enumerator2.Current);
                            break;
                    }
                }
                totalDetailsLineList.Add(totalDetailsLine);
            }
            return totalDetailsLineList;
        }

        private IntershopWebService.Discount ExtractDiscountsTotal(Axapta ax, AxaptaContainer discCont)
        {
            IntershopWebService.Discount discount = new IntershopWebService.Discount();
            IEnumerator enumerator = discCont.GetEnumerator();
            int num = 0;
            while (enumerator.MoveNext())
            {
                switch (num++)
                {
                    case 0:
                        discount.EndDiscAmount = Math.Round(Convert.ToDouble(enumerator.Current), 2);
                        break;
                    case 2:
                        discount.CashDisc = Math.Round(Convert.ToDouble(enumerator.Current), 2);
                        break;
                }
            }
            return discount;
        }

        private IntershopWebService.DiscountLine ExtractDiscountsTotalLine(Axapta ax, AxaptaContainer discCont)
        {
            IntershopWebService.DiscountLine discountLine = new IntershopWebService.DiscountLine();
            IEnumerator enumerator = discCont.GetEnumerator();
            int num = 0;
            while (enumerator.MoveNext())
            {
                switch (num++)
                {
                    case 0:
                        discountLine.DiscAmount = Math.Round(Convert.ToDouble(enumerator.Current), 2);
                        break;
                    case 1:
                        discountLine.DiscPercent = Math.Round(Convert.ToDouble(enumerator.Current), 2);
                        break;
                }
            }
            return discountLine;
        }

        private List<IntershopWebService.MiscCharge> ExtractMiscChargesTotal(Axapta ax, AxaptaContainer miscChargeCont)
        {
            List<IntershopWebService.MiscCharge> miscChargeList = new List<IntershopWebService.MiscCharge>();
            IEnumerator enumerator1 = miscChargeCont.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                IntershopWebService.MiscCharge miscCharge = new IntershopWebService.MiscCharge();
                IEnumerator enumerator2 = ((AxaptaContainer)enumerator1.Current).GetEnumerator();
                int num = 0;
                while (enumerator2.MoveNext())
                {
                    switch (num++)
                    {
                        case 0:
                            miscCharge.code = Convert.ToString(enumerator2.Current);
                            break;
                        case 1:
                            miscCharge.description = Convert.ToString(enumerator2.Current);
                            break;
                        case 2:
                            miscCharge.value = Math.Round(Convert.ToDouble(enumerator2.Current), 2);
                            break;
                        case 3:
                            miscCharge.currency = Convert.ToString(enumerator2.Current);
                            break;
                    }
                }
                if (num > 0)
                    miscChargeList.Add(miscCharge);
            }
            return miscChargeList;
        }

        private IntershopWebService.TrackInfo extractTrackInfo(string links)
        {
            List<IntershopWebService.Package> packageList = new List<IntershopWebService.Package>();
            IntershopWebService.TrackInfo trackInfo = new IntershopWebService.TrackInfo();
            if (string.IsNullOrEmpty(links))
            {
                trackInfo.ErrorMsg = "No sales order exists with the specified details.";
                trackInfo.ErrorCode = IntershopWebService.TrackInfoError.ERROR_SO_NOT_FOUND;
                return trackInfo;
            }
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(links);
            XmlNode childNode = xmlDocument.ChildNodes[0];
            string str1 = "<a href=\"%1\" target=\"_blank\">%1</a>";
            string str2 = "";
            IntershopWebService.Package package1 = new IntershopWebService.Package();
            package1.PackingSlipId = package1.Link = "&nbsp;";
            foreach (XmlElement xmlElement in xmlDocument.GetElementsByTagName("Item"))
            {
                if (!xmlElement["PackingSlipId"].InnerText.Equals(str2) && !string.IsNullOrEmpty(str2))
                {
                    packageList.Add(package1);
                    str2 = xmlElement["PackingSlipId"].InnerText;
                }
                else if (string.IsNullOrEmpty(str2))
                    str2 = xmlElement["PackingSlipId"].InnerText;
                IntershopWebService.Package package2 = new IntershopWebService.Package();
                package2.PackingSlipId = xmlElement["PackingSlipId"].InnerText;
                package2.PackageId = xmlElement["PackageId"].InnerText;
                package2.DispatchDate = xmlElement["DispatchDate"].InnerText;
                package2.Link = !string.IsNullOrEmpty(xmlElement["Link"].InnerText) ? str1.Replace("%1", Uri.UnescapeDataString(xmlElement["Link"].InnerText)) : "ERROR_NA";
                if (string.IsNullOrEmpty(package2.PackageId) && string.IsNullOrEmpty(package2.PackingSlipId) && string.IsNullOrEmpty(xmlElement["Link"].InnerText))
                {
                    trackInfo.ErrorMsg = "The sales order is being processed, shipping information is not available yet.";
                    trackInfo.ErrorCode = IntershopWebService.TrackInfoError.ERROR_NO_SHIPINFO;
                }
                if (string.IsNullOrEmpty(package2.PackageId))
                    package2.PackageId = "ERROR_NA";
                if (string.IsNullOrEmpty(package2.PackingSlipId))
                    package2.PackingSlipId = "ERROR_NA";
                if (string.IsNullOrEmpty(package2.DispatchDate))
                {
                    package2.DispatchDate = "ERROR_NA";
                    package2.Link = "ERROR_NA";
                }
                packageList.Add(package2);
            }
            if (links.Trim().Length == 0)
            {
                trackInfo.ErrorMsg = "No sales order exists with the specified details.";
                trackInfo.ErrorCode = IntershopWebService.TrackInfoError.ERROR_SO_NOT_FOUND;
            }
            else
                trackInfo.TrackLinks = packageList;
            return trackInfo;
        }

        private XmlDocument extractTrackRecord(string xmlString)
        {
            XmlDocument xmlDocument = new XmlDocument();
            if (!string.IsNullOrEmpty(xmlString))
                xmlDocument.LoadXml(xmlString.Replace("&", "&amp;"));
            return xmlDocument;
        }

        private void ExtractPriceValues(AxaptaContainer priceData, out double salesPrice, out string priceTypeStr)
        {
            IEnumerator enumerator = priceData.GetEnumerator();
            salesPrice = 0.0;
            priceTypeStr = "";
            int num = 0;
            while (enumerator.MoveNext())
            {
                switch (num++)
                {
                    case 0:
                        salesPrice = Math.Round(Convert.ToDouble(enumerator.Current), 2);
                        break;
                    case 3:
                        priceTypeStr = (int)enumerator.Current == 2 ? "List" : "Custom";
                        break;
                }
            }
        }

        private List<IntershopWebService.MiscCharge> ExtractMiscCharges(Axapta ax, string itemId, string custAccount, double salesPrice, int qty, string unit)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            IEnumerator enumerator1 = ((AxaptaContainer)ax.CallStaticClassMethod("BTX_ISHItemDescriptionProxy", "GetMiscCharges", new object[6]
            {
        (object) custAccount,
        (object) itemId,
        (object) salesPrice,
        (object) qty,
        (object) string.Empty,
        (object) unit
            })).GetEnumerator();
            List<IntershopWebService.MiscCharge> miscChargeList = new List<IntershopWebService.MiscCharge>();
            if (enumerator1.MoveNext())
            {
                double current = (double)enumerator1.Current;
            }
            if (enumerator1.MoveNext())
            {
                IEnumerator enumerator2 = ((AxaptaContainer)enumerator1.Current).GetEnumerator();
                IntershopWebService.MiscCharge miscCharge = new IntershopWebService.MiscCharge();
                while (enumerator2.MoveNext())
                {
                    IEnumerator enumerator3 = ((AxaptaContainer)enumerator2.Current).GetEnumerator();
                    int num = 0;
                    while (enumerator3.MoveNext())
                    {
                        switch (num++)
                        {
                            case 0:
                                miscCharge.code = (string)enumerator3.Current;
                                break;
                            case 1:
                                miscCharge.description = (string)enumerator3.Current;
                                break;
                            case 2:
                                miscCharge.value = Math.Round(Convert.ToDouble(enumerator3.Current), 2);
                                break;
                            case 3:
                                miscCharge.currency = (string)enumerator3.Current;
                                break;
                        }
                    }
                    miscChargeList.Add(miscCharge);
                }
            }
            stopwatch.Stop();
            MessageLogger.info(string.Format("ExtractMiscCharges({0}, {1}, {2}, {3}, {4}) -> {5} ms", (object)itemId, (object)custAccount, (object)salesPrice, (object)qty, (object)unit, (object)stopwatch.ElapsedMilliseconds));
            return miscChargeList;
        }

        private List<IntershopWebService.MiscCharge> ExtractMiscCharges(Axapta ax, string itemId, string custAccount, double salesPrice, int qty, string inventSizeId, string unit)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            IEnumerator enumerator1 = ((AxaptaContainer)ax.CallStaticClassMethod("BTX_ISHItemDescriptionProxy", "GetMiscCharges", new object[6]
            {
        (object) custAccount,
        (object) itemId,
        (object) salesPrice,
        (object) qty,
        (object) inventSizeId,
        (object) unit
            })).GetEnumerator();
            List<IntershopWebService.MiscCharge> miscChargeList = new List<IntershopWebService.MiscCharge>();
            if (enumerator1.MoveNext())
            {
                double current = (double)enumerator1.Current;
            }
            if (enumerator1.MoveNext())
            {
                IEnumerator enumerator2 = ((AxaptaContainer)enumerator1.Current).GetEnumerator();
                IntershopWebService.MiscCharge miscCharge = new IntershopWebService.MiscCharge();
                while (enumerator2.MoveNext())
                {
                    IEnumerator enumerator3 = ((AxaptaContainer)enumerator2.Current).GetEnumerator();
                    int num = 0;
                    while (enumerator3.MoveNext())
                    {
                        switch (num++)
                        {
                            case 0:
                                miscCharge.code = (string)enumerator3.Current;
                                break;
                            case 1:
                                miscCharge.description = (string)enumerator3.Current;
                                break;
                            case 2:
                                miscCharge.value = Math.Round(Convert.ToDouble(enumerator3.Current), 2);
                                break;
                            case 3:
                                miscCharge.currency = (string)enumerator3.Current;
                                break;
                        }
                    }
                    miscChargeList.Add(miscCharge);
                }
            }
            stopwatch.Stop();
            MessageLogger.info(string.Format("ExtractMiscCharges({0}, {1}, {2}, {3}, {4}, {5}) -> {6} ms", (object)itemId, (object)custAccount, (object)salesPrice, (object)qty, (object)inventSizeId, (object)unit, (object)stopwatch.ElapsedMilliseconds));
            return miscChargeList;
        }

        private Axapta axLogon(string company)
        {
            Axapta axapta = new Axapta();
            NetworkCredential networkCredential = new NetworkCredential("xxxxxxxxx", "xxxxxxxxx", "BCE.Local");
            axapta.LogonAs("xxxxxxxxxxx", "xxxxxxxx", networkCredential, company, (string)null, (string)null, (string)null);
            return axapta;
        }

        public class ItemUnit
        {
            public string itemId;
            public string unitId;
        }

        public class ProductPriceIn
        {
            public string itemId;
            public string unitId;
            public short quantity;
            public string size;
            public string color;
        }

        public class ProductStockIn
        {
            public string itemId;
            public string unitId;
            public string size;
            public string color;
        }

        public class ProductPrice
        {
            public string itemId;
            public double price;
            public string priceType;
            public List<IntershopWebService.MiscCharge> miscCharges;
        }

        public class ProductPriceSize
        {
            public string itemId;
            public double price;
            public string priceType;
            public List<IntershopWebService.MiscCharge> miscCharges;
            public List<IntershopWebService.SizePrice> sizes;
        }

        public class SizeStock
        {
            public string size;
            public string color;
            public string stockLevel;
        }

        public class SizePrice
        {
            public string size;
            public string color;
            public double price;
            public string priceType;
            public List<IntershopWebService.MiscCharge> miscCharges;
        }

        public class ProductStock
        {
            public string itemId;
            public string stockLevel;
            public List<IntershopWebService.SizeStock> sizes;
        }

        public class MiscCharge
        {
            public string code;
            public string description;
            public double value;
            public string currency;
        }

        public class Package
        {
            public string PackingSlipId;
            public string PackageId;
            public string Link;
            public string DispatchDate;
        }

        public enum TrackInfoError
        {
            OK,
            ERROR_NO_PARAM,
            ERROR_NA,
            ERROR_NO_SHIPINFO,
            ERROR_SO_NOT_FOUND,
        }

        public class TrackInfo
        {
            public List<IntershopWebService.Package> TrackLinks;
            public string ErrorMsg;
            public IntershopWebService.TrackInfoError ErrorCode;
        }

        public class IshOrderLine
        {
            public string ItemID;
            public double Qty;
            public string SalesUnit;
            public string size;
            public string color;
        }

        public class Discount
        {
            public double EndDiscAmount;
            public double CashDisc;
        }

        public class DiscountLine
        {
            public double DiscAmount;
            public double DiscPercent;
        }

        public class TotalDetailsLine
        {
            public string ItemID;
            public double Qty;
            public string SalesUnit;
            public double UnitPrice;
            public double LineNetAmount;
            public List<IntershopWebService.MiscCharge> MiscCharges;
            public IntershopWebService.DiscountLine Discounts;
        }

        public class TotalDetails
        {
            public string IshBasketID;
            public string AxBasketID;
            public double TotalAmount;
            public string Currency;
            public double TotalNetAmount;
            public double VatTotal;
            public List<IntershopWebService.MiscCharge> MiscCharges;
            public IntershopWebService.Discount Discounts;
            public List<IntershopWebService.TotalDetailsLine> OrderLines;
        }
    }
}
