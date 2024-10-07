/**
 * @author Gaurang Valia
 * @description the common Http service for communication with the server with the help of REST API.
 */

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
/** -------------------------------------------------------------- */

@Injectable({
    /** to provide an object of this service in the root injector. */
    providedIn: 'root',
})
export class HttpCommonService {

    /**
     * @constructor
     * @param httpClient the Http Client Injection
     */
    constructor(
        private httpClient: HttpClient,
    ) {

    }

    /**
     * @author Gaurang Valia
     * @description provides the Common Http Get Request
     * @param url               The Common URL for Http Request
     * @param options           The Optional Parameters
     */
    public httpGetRequest<T>(url: string, options?: any): Observable<any> {
        return this.httpClient.get<T>(url, options);
    }

    /**
     * @author Gaurang Valia
     * @description Provides the common Http Post Request
     * @param url               the Request URL,
     * @param requestBody       The Request Body
     * @param options           The Optional Parameters
     */
    public httpPostRequest<T>(url: string, requestBody: any, options?: any): Observable<any> {
        return this.httpClient.post<T>(url, requestBody, options);
    }

    /**
     * @author Gaurang Valia
     * @description Provides the common Http Put Request
     * @param url               the Request URL
     * @param requestBody       The Request Body
     * @param options           The Optional Parameters
     */
    public httpPutRequest<T>(url: string, requestBody: any, options?: any): Observable<any> {
        return this.httpClient.put<T>(url, requestBody, options);
    }

    /**
     * @author Gaurang Valia
     * @description provides the Common Http Delete Request
     * @param url               The Common URL for Http Request
     * @param options           The Optional Parameters
     */
    public httpDeleteRequest<T>(url: string, options?: any): Observable<any> {
        return this.httpClient.delete(url, options);
    }
}
