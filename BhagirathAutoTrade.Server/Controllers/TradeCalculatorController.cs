﻿using BhagirathAutoTrade.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;

namespace BhagirathAutoTrade.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TradeCalculatorController : ControllerBase
    {
        private readonly ILogger<TradeCalculatorController> _logger;
        private readonly string _excelFilePath = @"\CALCULATIONS.xlsx";
        private readonly string _fetchEQDataFromAPI = "http://api.bhagirathfincare.in/api/Equity/getCalculateDataForEQ?exchange={0}&type={1}&symbol={2}&workingdate={3}&expirydate={4}&close={5}&instrument={6}&optionType={7}";
        private readonly string _fetchStrikePriceDataFromAPI = "http://api.bhagirathfincare.in/api/Equity/GetStrikePrice?exchange=NSE&type=DERIVATIVE&symbol=TATASTEEL&expireDate=04/25/2024";
        private readonly string _calculateEquityApiUrl = "http://api.bhagirathfincare.in/api/Equity/CalculateEquity";

        public TradeCalculatorController(ILogger<TradeCalculatorController> logger)
        {
            _logger = logger;
        }

        [HttpGet("DownloadExcel")]
        public async Task<IActionResult> DownloadExcel(string type, string exchange, string symbol, DateTime workingDate, DateTime expiryDate, decimal close, decimal ss, string sst, decimal rs, string rst, decimal hs, decimal hr, string? instrument, string? optionType)
        {
            try
            {
                var url = string.Empty;
                var apiResponse = new EquityApiResponse<EquityData>();

                switch (type.ToUpper())
                {
                    case "EQ":
                        url = string.Format(_fetchEQDataFromAPI, exchange, type, symbol, workingDate.ToString("MM-dd-yyyy"), expiryDate.ToString("MM-dd-yyyy"), close, instrument, optionType);
                        break;
                    case "DERIVATIVE":
                        url = string.Format(_fetchEQDataFromAPI, exchange, type, symbol, workingDate.ToString("MM-dd-yyyy"), expiryDate.ToString("MM-dd-yyyy"), close, instrument, optionType);
                        break;
                }
                // Fetch data from API
                apiResponse = await FetchDataFromAPI(url);

                var requestModel = new DtoRequestModelForCalculate
                {
                    Type = type,
                    Exchange = exchange,
                    Close = close,
                    ExpiryDate = expiryDate.ToString("MM/dd/yyyy"),
                    WorkingDate = workingDate.ToString("MM/dd/yyyy"),
                    Symbole = symbol,
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
                return File(_excelFilePath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "UpdatedFile.xlsx");
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
                var ssrs = Math.Round((data.SS + data.RS) * 0.89m, 2);
                var hsrs = Math.Round((data.HS + data.HR) * 0.89m, 2);
                var srbap = Math.Round((Sbap + Rbap) * 0.89m, 2);
                var s3r2 = Math.Round((S3 + R2) * 0.89m, 2);

                sheet2.Cells[4, 5] = $"{(data.RS + ssrs)},{(data.HR + hsrs)},{(Rbap + srbap)},{(R2 + s3r2)}";

                // Update Sheet3
                Worksheet sheet3 = (Worksheet)workbook.Sheets[3];
                // Update specific cells in sheet2 as needed
                sheet3.Cells[4, 1] = data.SelectSymbol;
                sheet3.Cells[4, 2] = data.expirydate;
                sheet3.Cells[4, 4] = data.strikeprice;

                sheet3.Cells[4, 5] = $"{(data.SS + ssrs)},{(data.HS + hsrs)},{(Sbap + srbap)},{(S3 + s3r2)}";

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
