import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpCommonService } from '../../core/services/http-common.service';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { Broker } from '../../broker/model/broker';
import { HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class BrokerService {

  basedUrl: string = environment.serverUrl;
  public bSubject: BehaviorSubject<any> = new BehaviorSubject(null);
  public showBrokers$ = this.bSubject.asObservable();
  constructor(private http: HttpCommonService) {}

  getAllBrokers(): Observable<Broker[]> {
    return this.http.httpGetRequest<Broker[]>(`${this.basedUrl}/Broker`);
  }

  getBrokerById(id: any): Observable<any> {
    return this.http.httpGetRequest(`${this.basedUrl}/Broker/${id}`);
  }

  deleteBroker(id: any): Observable<any> {
    return this.http.httpDeleteRequest(`${this.basedUrl}/Broker/${id}`);
  }
}
