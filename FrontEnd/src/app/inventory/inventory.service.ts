import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpCommonService } from '../core/services/http-common.service';
import { Observable } from 'rxjs';

@Injectable()
export class InventoryService {
  basedUrl: string = environment.serverUrl;

  constructor(private http: HttpCommonService) {}
  
  getAllInventory(): Observable<any> {
    return this.http.httpGetRequest(`${this.basedUrl}/rawmaterial/list`);
  }
  importFile(data:any): Observable<any> {
    debugger
    return this.http.httpPostRequest(`${this.basedUrl}/rawmaterial/import`,data);
  }
}
