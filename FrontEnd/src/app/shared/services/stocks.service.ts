import { HttpClient } from '@angular/common/http';
import { EventEmitter, Injectable, Output } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { EquityData } from '../../dashboard/models/equity-data'
import { UserAlgoData } from '../models/UserAlgoData';

@Injectable({
  providedIn: 'root'
})
export class StocksService {

  private baseUrl = `${environment.serverUrl}/MarketData`;
  private tradeBaseUrl = `${environment.serverUrl}/MarketData`;

  @Output()
  public refreshEvent: EventEmitter<boolean> = new EventEmitter<boolean>();

  constructor(private http: HttpClient) { }

  autoCompleteCompanyForEquity(exchange: string, type: string, instrument: string, optionType: string): string[] {
    const names: string[] = ["NIFTY", "BANKNIFTY", "FINNIFTY"];
    return names; 
  }

  getCalculateDataForEQ(data: EquityData): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/getCalculateDataForEQ`, data);
  }

  downloadExcel(data: EquityData) {
    // Convert the requestData object to a query string
    const queryString = this.serialize(data);

    // Construct the API endpoint URL with the query string
    const downloadUrl = `${this.baseUrl}/Calculate?${queryString}`;

    // Make a GET request to the API endpoint
    return this.http.get(downloadUrl, { responseType: 'blob' })
  }

  // Helper function to convert an object to a query string
  private serialize(obj: any): string {
    return Object.keys(obj).map(key => `${encodeURIComponent(key)}=${encodeURIComponent(obj[key])}`).join('&');
  }

  getStrikePrice(symbol: string): Observable<any> {
    const url = `${this.baseUrl}/GetOptionsStrikePrices?symbol=${symbol}`;
    return this.http.get<string[]>(url);
  }

  getexpiredate(companyName: string, optiontype: string = 'XX'): Observable<string[]> {
    if (optiontype == '') {
      optiontype = 'XX';
    }

    const url = `${this.baseUrl}/GetExpiryDates?symbol=${companyName}&optionType=${optiontype}`;
    return this.http.get<string[]>(url);
  }

  getCloseData(data: EquityData): Observable<string> {
    return this.http.post<string>(`${this.baseUrl}/GetCloseData`, data);
  }

  getOpenData(data: EquityData): Observable<string> {
    return this.http.post<string>(`${this.baseUrl}/GetOpenData`, data);
  }

  getAlgoData(): Observable<UserAlgoData[]>{
    const url = `${this.baseUrl}/GetAlgoConfigurations`;
    return this.http.get<UserAlgoData[]>(url);
  }

  addUserAlgoData(data:UserAlgoData):Observable<boolean>{
    return this.http.post<boolean>(`${this.baseUrl}/InsertAlgoConfigurations`, data);
  }
}
