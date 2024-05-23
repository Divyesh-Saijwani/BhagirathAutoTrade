import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { APIUrlConstants } from '../models/apiurl-constants';
import { EquityData } from '../models/equity-data';

@Injectable({
  providedIn: 'root'
})
export class StocksService {

  private baseUrl = `${APIUrlConstants.BaseUrl}/Equity`;
  private tradeBaseUrl = `${APIUrlConstants.BaseUrl}/TradeCalculator`;

  constructor(private http: HttpClient) { }

  autoCompleteCompanyForEquity(exchange: string, type: string, instrument: string, optionType: string): Observable<string[]> {
    const body = { exchange, type, instrument, optionType };
    return this.http.post<string[]>(`${this.baseUrl}/AutoCompleteCompanyForEquity`, body);
  }

  getCalculateDataForEQ(data: EquityData): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/getCalculateDataForEQ`, data);
  }

  downloadExcel(data: EquityData) {
    // Convert the requestData object to a query string
    const queryString = this.serialize(data);

    // Construct the API endpoint URL with the query string
    const downloadUrl = `${this.tradeBaseUrl}/DownloadExcel?${queryString}`;

    // Make a GET request to the API endpoint
    return this.http.get(downloadUrl, { responseType: 'blob' })
  }

  // Helper function to convert an object to a query string
  private serialize(obj: any): string {
    return Object.keys(obj).map(key => `${encodeURIComponent(key)}=${encodeURIComponent(obj[key])}`).join('&');
  }

  getStrikePrice(exchange: string, type: string, symbole: string, expiryDate: string): Observable<any> {
    const url = `${APIUrlConstants.BaseUrl}/Equity/GetStrikePrice`;
    const params = {
      exchange,
      type,
      symbole,
      expiryDate
    };

    return this.http.post<any>(url, params).pipe(map(response => {
      if (response) {
        return response;
      }
    }));
  }

  getexpiredate(companyName: string, optiontype: string = 'XX'): Observable<string[]> {
    if (optiontype == '') {
      optiontype = 'XX';
    }

    const url = `${this.baseUrl}/GetExpiryDate?symbol=${companyName}&optionType=${optiontype}`;
    return this.http.get<string[]>(url);
  }

  getCloseData(data: EquityData): Observable<string> {
    return this.http.post<string>(`${this.baseUrl}/GetCloseData`, data);
  }

  getOpenData(data: EquityData): Observable<string> {
    return this.http.post<string>(`${this.baseUrl}/GetOpenData`, data);
  }
}
