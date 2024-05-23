using BhagirathAutoTrade.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;

namespace BhagirathAutoTrade.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TradeCalculatorController : ControllerBase
    {
        private readonly ILogger<TradeCalculatorController> _logger;
        private readonly string _excelFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "wwwroot\\CALCULATIONS.xlsx");
        private readonly string _fetchEQDataFromAPI = "http://api.bhagirathfincare.in/api/Equity/getCalculateDataForEQ?exchange={0}&type={1}&symbol={2}&workingdate={3}&expirydate={4}&close={5}&instrument={6}&optionType={7}";
        private readonly string _fetchStrikePriceDataFromAPI = "http://api.bhagirathfincare.in/api/Equity/GetStrikePrice?exchange=NSE&type=DERIVATIVE&symbol=TATASTEEL&expireDate=04/25/2024";
        private readonly string _calculateEquityApiUrl = "http://api.bhagirathfincare.in/api/Equity/CalculateEquity";

        public TradeCalculatorController(ILogger<TradeCalculatorController> logger)
        {
            _logger = logger;
        }

        [HttpGet("DownloadExcel")]
        public async Task<IActionResult> DownloadExcel(string type, string exchange, string symbole, DateTime workingDate, DateTime expiryDate, decimal close, decimal ss, string sst, decimal rs, string rst, decimal hs, decimal hr, string? instrument, string? optionType)
        {
            try
            {
                var url = string.Empty;
                var apiResponse = new EquityApiResponse<EquityData>();

                switch (type.ToUpper())
                {
                    case "EQ":
                        url = string.Format(_fetchEQDataFromAPI, exchange, type, symbole, workingDate.ToString("MM-dd-yyyy"), expiryDate.ToString("MM-dd-yyyy"), close, instrument, optionType);
                        break;
                    case "DERIVATIVE":
                        url = string.Format(_fetchEQDataFromAPI, exchange, type, symbole, workingDate.ToString("MM-dd-yyyy"), expiryDate.ToString("MM-dd-yyyy"), close, instrument, optionType);
                        break;
                }
                // Fetch data from API
                apiResponse = await FetchDataFromAPI(url);

                var requestModel = new DtoRequestModelForCalculate
                {
                    Type = type,
                    Exchange = exchange,
                    Close = close,
                    ExpiryDate = type.ToUpper()=="EQ"?"":expiryDate.ToString("MM/dd/yyyy"),
                    WorkingDate = workingDate.ToString("MM/dd/yyyy"),
                    Symbole = symbole,
                    Instrument = instrument,
                    OptionType = optionType,
                    CMP = Convert.ToDecimal(apiResponse.Data.Cmp),
                    Average = Convert.ToDecimal(apiResponse.Data.Average),
                    IDH = Convert.ToDecimal(apiResponse.Data.High),
                    IDL = Convert.ToDecimal(apiResponse.Data.Low),
                    Open = Convert.ToDecimal(apiResponse.Data.Open),
                };

                var result = await GetCalculatedDataFromAPI(_calculateEquityApiUrl, requestModel);
                result.Data.SS = ss;
                result.Data.SST = sst;
                result.Data.RS = rs;
                result.Data.RST = rst;
                result.Data.HS = hs;
                result.Data.HR = hr;
                // Update Excel file
                UpdateExcel(result.Data);

                // Provide download link for the updated file
                return PhysicalFile(_excelFilePath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "UpdatedFile.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        private async Task<EquityApiResponse<EquityData>> FetchDataFromAPI(string apiUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                // Sample data for POST request
                var myContent = JsonConvert.SerializeObject("");
                var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                var byteContent = new ByteArrayContent(buffer);

                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await client.PostAsync(apiUrl, byteContent);
                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();

                    // Create a StringReader to read the JSON string
                    using (StringReader stringReader = new StringReader(responseData))
                    {
                        // Create a JsonTextReader to read from the StringReader
                        using (JsonTextReader jsonReader = new JsonTextReader(stringReader))
                        {
                            // Use JsonSerializer to deserialize the JSON
                            JsonSerializer serializer = new JsonSerializer();
                            var apiResponse = serializer.Deserialize<EquityApiResponse<EquityData>>(jsonReader);
                            return apiResponse;
                        }
                    }
                }
                else
                {
                    throw new Exception($"Failed to fetch data from API. Status code: {response.StatusCode}");
                }
            }
        }

        private async Task<EquityApiResponse<EquityDetailedData>> GetCalculatedDataFromAPI(string getCalculatedDataUrl, DtoRequestModelForCalculate data)
        {
            using (HttpClient client = new HttpClient())
            {
                // Sample data for POST request
                var myContent = JsonConvert.SerializeObject(data);
                var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                var byteContent = new ByteArrayContent(buffer);

                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                // Make POST request to API
                HttpResponseMessage response = await client.PostAsync(getCalculatedDataUrl, byteContent);

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<EquityApiResponse<EquityDetailedData>>(responseData);
                    result.Data.CMP = data.CMP;
                    return result;
                }
                else
                {
                    throw new Exception($"Failed to fetch data from API. Status code: {response.StatusCode}");
                }
            }
        }

        private void UpdateExcel(EquityDetailedData data)
        {
            var trend = data.txt_K13_not.ToLower();
            var S3Heading = "S3";
            var R2Heading = "R2";
            decimal S3 = 0;
            decimal R2 = 0;
            var Sbap = Math.Round(Convert.ToDecimal(data.txt_J13), 2);
            var Rbap = Math.Round(Convert.ToDecimal(data.txt_N13), 2);
            decimal sellSL = 0;
            decimal buySL = 0;
            decimal entryReferencePoint = Math.Round(data.CMP * 0.0021m, 2) ;

            switch (trend)
            {
                case "buy":
                    S3 = Math.Round(Convert.ToDecimal(data.txt_J6), 2);
                    R2 = Math.Round(Convert.ToDecimal(data.txt_P6), 2);
                    S3Heading = "S2";
                    R2Heading = "R3";
                    sellSL = Math.Round(Convert.ToDecimal(data.txt_t11) + .05m, 2);
                    buySL = Math.Round(Convert.ToDecimal(data.txt_d11) - .05m, 2);
                    break;

                case "sell":
                    S3 = Math.Round(Convert.ToDecimal(data.txt_I6), 2);
                    R2 = Math.Round(Convert.ToDecimal(data.txt_O6), 2);
                    S3Heading = "S3";
                    R2Heading = "R2";
                    sellSL = Math.Round(Convert.ToDecimal(data.txt_t11) + .05m, 2);
                    buySL = Math.Round(Convert.ToDecimal(data.txt_d11) - .05m, 2);
                    break;

                default:
                    S3 = Math.Round(Convert.ToDecimal(data.txt_I6), 2);
                    R2 = Math.Round(Convert.ToDecimal(data.txt_P6), 2);
                    S3Heading = "S3";
                    R2Heading = "R3";
                    Sbap = Math.Round(Convert.ToDecimal(data.txt_I6), 2);
                    Rbap = Math.Round(Convert.ToDecimal(data.txt_P6), 2);
                    sellSL = Math.Round(Convert.ToDecimal(data.txt_t8) + .10m, 2);
                    buySL = Math.Round(Convert.ToDecimal(data.txt_d8) - .10m, 2);
                    break;
            }

            Sbap=Sbap<1?S3 : Sbap;
            Rbap=Rbap<1?R2 : Rbap;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo(_excelFilePath)))
            {
                try
                {
                    ExcelWorksheet sheet1 = excelPackage.Workbook.Worksheets[0];
                    sheet1.Cells[2, 8].Value = data.CMP; // CMP
                    sheet1.Cells[2, 5].Value = data.SS; // SS
                    sheet1.Cells[2, 6].Value = data.RS; // RS
                    sheet1.Cells[4, 5].Value = data.HS; // HS
                    sheet1.Cells[4, 6].Value = data.HR; // HR
                    sheet1.Cells[6, 5].Value = Sbap; // S-BAP
                    sheet1.Cells[6, 6].Value = Rbap; // R-BAP

                    sheet1.Cells[7, 5].Value = S3Heading; // S3 Heading
                    sheet1.Cells[7, 6].Value = R2Heading; // R2 Heading
                    sheet1.Cells[8, 5].Value = S3; // S3
                    sheet1.Cells[8, 6].Value = R2; // R2

                    // Buy points calculations
                    ExcelWorksheet sheet2 = excelPackage.Workbook.Worksheets[1];
                    sheet2.Cells[4, 1].Value = data.SelectSymbol;
                    sheet2.Cells[4, 2].Value = data.expirydate;
                    sheet2.Cells[4, 4].Value = data.strikeprice;
                    sheet2.Cells[2, 8].Value = "Entry Time";
                    sheet2.Cells[4, 8].Value = data.SST;

                    var ssrs = Math.Round((data.SS - data.RS) * 0.89m, 2);
                    var hsrs = Math.Round((data.HS - data.HR) * 0.89m, 2);
                    var srbap = Math.Round((Sbap - Rbap) * 0.89m, 2);
                    var s3r2 = Math.Round((S3 - R2) * 0.89m, 2);

                    var buyPoints = new List<decimal> { (data.RS + ssrs), (data.HR + hsrs), (Rbap + srbap), (R2 + s3r2) };
                    var buyPointMax = buyPoints.Max();
                    var buyPointMin = buyPoints.Min();
                    var buyPointAverage = (buyPointMax + buyPointMin) / 2;

                    
                    


                    // Sell points calculation
                    ExcelWorksheet sheet3 = excelPackage.Workbook.Worksheets[2];
                    sheet3.Cells[4, 1].Value = data.SelectSymbol;
                    sheet3.Cells[4, 2].Value = data.expirydate;
                    sheet3.Cells[4, 4].Value = data.strikeprice;
                    sheet3.Cells[2, 8].Value = "Exit Time";
                    sheet3.Cells[4, 8].Value = data.RST;

                    var sellPoints = new List<decimal> { (data.SS - ssrs), (data.HS - hsrs), (Sbap - srbap), (S3 - s3r2) };
                    var sellPointMax = sellPoints.Max();
                    var sellPointMin = sellPoints.Min();
                    var sellPointAverage = (sellPointMax + sellPointMin) / 2;

                    // Buy Entry Points
                    var buyEntryPoints = "";
                    if (entryReferencePoint <= (sellPointMin - buyPointMax))
                    {
                        buyEntryPoints = buyEntryPoints + buyPointMax;
                    }

                    if (entryReferencePoint <= (sellPointMin - buyPointAverage))
                    {
                        var saperator = string.IsNullOrEmpty(buyEntryPoints) ? "" : ", ";
                        buyEntryPoints = buyEntryPoints + saperator + buyPointAverage;
                    }

                    if (entryReferencePoint <= (sellPointMin - buyPointMin))
                    {
                        var saperator = string.IsNullOrEmpty(buyEntryPoints) ? "" : ", ";
                        buyEntryPoints = buyEntryPoints + saperator + buyPointMin;
                    }

                    sheet2.Cells[4, 5].Value = buyEntryPoints; // entry points
                    sheet2.Cells[4, 6].Value = buySL; // Buy Stop Loss

                    // Sell Entry Points
                    var sellEntryPoints = "";
                    if (entryReferencePoint <= (sellPointMin - buyPointMax))
                    {
                        sellEntryPoints = sellEntryPoints + sellPointMin;
                    }

                    if (entryReferencePoint <= (sellPointMin - buyPointAverage))
                    {
                        var saperator = string.IsNullOrEmpty(buyEntryPoints) ? "" : ", ";
                        sellEntryPoints = sellEntryPoints + saperator + sellPointAverage;
                    }

                    if (entryReferencePoint <= (sellPointMin - buyPointMin))
                    {
                        var saperator = string.IsNullOrEmpty(buyEntryPoints) ? "" : ", ";
                        sellEntryPoints = sellEntryPoints + saperator + sellPointMax;
                    }

                    sheet3.Cells[4, 5].Value = $"{sellPointMin}, {sellPointAverage}, {sellPointMax}"; // exit points
                    sheet3.Cells[4, 6].Value = sellSL; // Sell Stop Loss

                    // Target for both Buy and Sell

                    sheet2.Cells[4, 7].Value = sellPoints.Min(); // Buy target
                    sheet3.Cells[4, 7].Value = buyPoints.Max(); // Sell target

                    excelPackage.Save();
                }
                catch (Exception ex)
                {
                    throw new Exception($"An error occurred while updating Excel file: {ex.Message}");
                }
            }
        }

        private void UpdateExcelUsingCom(EquityDetailedData data)
        {
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            try
            {
                Workbook workbook = excelApp.Workbooks.Open(_excelFilePath);

                // Update Sheet1
                Worksheet sheet1 = (Worksheet)workbook.Sheets[1];
                sheet1.Cells[2, 8] = data.CMP; // CMP
                sheet1.Cells[2, 5] = data.SS; // SS
                sheet1.Cells[2, 6] = data.RS; // RS
                sheet1.Cells[4, 5] = data.HS; // HS
                sheet1.Cells[4, 6] = data.HR; // HR
                var Sbap = Math.Round(Convert.ToDecimal(data.txt_J13), 2);
                var Rbap = Math.Round(Convert.ToDecimal(data.txt_N13), 2);
                sheet1.Cells[6, 5] = Sbap; // S-BAP
                sheet1.Cells[6, 6] = Rbap; // R-BAP
                var S3 = Math.Round(Convert.ToDecimal(data.txt_I6), 2);
                var R2 = Math.Round(Convert.ToDecimal(data.txt_O6), 2);
                sheet1.Cells[8, 5] = data.txt_I6; // S3
                sheet1.Cells[8, 6] = data.txt_O6; // R2



                // Update Sheet2
                Worksheet sheet2 = (Worksheet)workbook.Sheets[2];
                // Update specific cells in sheet2 as needed
                sheet2.Cells[4, 1] = data.SelectSymbol;
                sheet2.Cells[4, 2] = data.expirydate;
                sheet2.Cells[4, 4] = data.strikeprice;
                var ssrs = Math.Round((data.SS - data.RS) * 0.89m, 2);
                var hsrs = Math.Round((data.HS - data.HR) * 0.89m, 2);
                var srbap = Math.Round((Sbap - Rbap) * 0.89m, 2);
                var s3r2 = Math.Round((S3 - R2) * 0.89m, 2);

                var buyPoints = new List<decimal> { (data.RS + ssrs), (data.HR + hsrs), (Rbap + srbap), (R2 + s3r2) };
                var buyPointMax = buyPoints.Max();
                var buyPointMin = buyPoints.Min();
                var buyPointAverage = (buyPointMax + buyPointMin) / 2;

                sheet2.Cells[4, 5] = $"{buyPointMax},{buyPointAverage},{buyPointMin}";
                sheet2.Cells[2, 8] = "Entry Time";
                sheet2.Cells[4, 8] = data.SST;


                // Update Sheet3
                Worksheet sheet3 = (Worksheet)workbook.Sheets[3];
                // Update specific cells in sheet2 as needed
                sheet3.Cells[4, 1] = data.SelectSymbol;
                sheet3.Cells[4, 2] = data.expirydate;
                sheet3.Cells[4, 4] = data.strikeprice;

                var sellPoints = new List<decimal> { (data.SS + ssrs), (data.HS + hsrs), (Sbap + srbap), (S3 + s3r2) };
                var sellPointMax = sellPoints.Max();
                var sellPointMin= sellPoints.Min();
                var sellPointAverage = (sellPointMax + sellPointMin) / 2;

                sheet3.Cells[4, 5] = $"{sellPointMin},{sellPointAverage},{sellPointMax}";
                sheet3.Cells[2, 8] = "Exit Time";
                sheet3.Cells[4, 8] = data.RST;

                // Save and close the workbook
                workbook.Save();
                workbook.Close();

                // Release COM objects to avoid memory leaks
                ReleaseObject(sheet1);
                ReleaseObject(sheet2);
                ReleaseObject(sheet3);
                ReleaseObject(workbook);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while updating Excel file: {ex.Message}");
            }
            finally
            {
                // Quit Excel application
                if (excelApp != null)
                {
                    excelApp.Quit();
                    ReleaseObject(excelApp);
                }
            }
        }

        // Helper method to release COM objects
        private void ReleaseObject(object obj)
        {
            try
            {
                Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                throw ex;
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
