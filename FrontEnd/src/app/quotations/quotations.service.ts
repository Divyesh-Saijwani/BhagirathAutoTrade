import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpCommonService } from '../core/services/http-common.service';
import { Observable } from 'rxjs';

@Injectable()
export class QuotationsService {
  baseUrl: string = environment.serverUrl;
  constructor(private httpCommonService: HttpCommonService) {}
  // https://8zjz3yot3k.execute-api.us-west-1.amazonaws.com/develop/quotation/list
  getAllQuotations(): Observable<any> {
    return this.httpCommonService.httpGetRequest(
      `${this.baseUrl}/quotation/list`
    );
  }
  // https://8zjz3yot3k.execute-api.us-west-1.amazonaws.com/develop/quotation/create
  addQuotations() {
    return this.httpCommonService.httpPostRequest(
      `${this.baseUrl}/quotation/create`, this.formatData()
    );
  }

  formatData() {
    let data = {
      customerId: '5bbab9bc-78f8-46ec-bf1c-f26b9688d35b',
      productTypeId: 'd7335897-39c2-4cee-98da-80ebe13c93b8',
      productId: '58a6a2ce-8f22-4115-9269-f2a40e91f9e7',
      categoryId: 'f915dd81-4fda-47ab-82af-c2f7a5b1d9f6',
      categoryItemId: '5ca7781a-bcc1-4e3e-948f-1ac810ea0795',
      qty: 1,
      height: 10,
      width: 11,
      NoOfLite: 1,
      ColorCode: 'NJJN',
      totalAmount: 1000,
      tax: 100,
      discount: 0,
      payableAmount: 1100,
      lites: [
        {
          height: 10,
          width: 11,
          liteType: 1,
        },
      ],
    };
  }
}
