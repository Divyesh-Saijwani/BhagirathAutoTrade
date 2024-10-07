import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpCommonService } from '../../core/services/http-common.service';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { Broker } from '../../broker/model/broker';
import { HttpParams } from '@angular/common/http';
import { Calculation } from '../../dashboard/models/calculation';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  basedUrl: string = environment.serverUrl;
  public bSubject: BehaviorSubject<any> = new BehaviorSubject(null);
  public showBrokers$ = this.bSubject.asObservable();
  constructor(private http: HttpCommonService) {}

  GetUserBrokerConfigurationStatus(id: any): Observable<any> {
    return this.http.httpGetRequest(`${this.basedUrl}/dashboard/GetUserBrokerConfigurationStatus/${id}`);
  }

  GetTodayCalculations(): Observable<Calculation[]> {
    return this.http.httpGetRequest(`${this.basedUrl}/MarketData/GetTodayCalculations`);
  }
}
