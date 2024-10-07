/**
 * @author Gaurang valia
 * @description the Http Interceptor Handler.
 *              1)  Handles the Error Response and/or Success Response for Middle Ware Tasks like start and complete the loader.
 *              2)  provides the toaster message on Error.
 */

import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpHeaderResponse,
  HttpInterceptor,
  HttpProgressEvent,
  HttpRequest,
  HttpResponse,
  HttpSentEvent,
  HttpUserEvent,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, map, retry, tap, throwError } from 'rxjs';
import { AuthService } from './auth.service';
import { ToastrService } from 'ngx-toastr';
import { MessageTitles, Messages } from '../model/http-interceptor.model';
import { LoaderService } from '../components/loader/loader.service';

/**
 * \@description provides the global error handling using HttpInterceptor.
 */
@Injectable()
export class HttpInterceptorHandler implements HttpInterceptor {
  /** the message for toaster. */
  private message!: string;
  /** the message Title for toaster. */
  private messageTitle!: string;
  /*** Token*/
  private token!: string;

  /**
   * @constructor
   * @param toasterService        the toaster service to give error in toaster.
   * @param loaderService         the loader service to start or complete the loader.
   * @param _authService          the authentication service to get authentication.
   */
  constructor(
    private toasterService: ToastrService,
    private loaderService: LoaderService,
    private _authService: AuthService
  ) {}

  /**
   * @description intercept the Http Request to handel the Error for user understanding.
   * @param request the Http Request to handle the error
   * @param next will handle the Error.
   */
  public intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<
    | HttpSentEvent
    | HttpHeaderResponse
    | HttpProgressEvent
    | HttpResponse<object>
    | HttpUserEvent<object>
    | HttpEvent<any>
  > {
    this.loaderService.displayLoader(true);

    this.token = this._authService.getAuthorizationHeaderValue();
    const cloneRequest: HttpRequest<any> = request.clone({
      setHeaders: {
        // 'lang': this.preferedLang,
        // 'Authorization': '',
        Authorization: 'Bearer ' + this.token,
      },
    });
    return next.handle(cloneRequest).pipe(
      retry(1),
      tap((response: any) => {
        // Handle success response here
        // For example, you can log the response or process it further
        // console.log('Success:', response);
        this.loaderService.displayLoader(false);
      }),
      catchError((errorResponse: HttpErrorResponse) => {
        // Handle error response here
        let message: string = '';
        this.message = Messages.MessageForCommanError;
        this.messageTitle = MessageTitles.Error;
        if (errorResponse.error instanceof ErrorEvent) {
          message = `Error: ${errorResponse.error}`;
        } else {
          message = `Error Code: ${errorResponse.status}\nMessage: ${errorResponse.message}`;
          if (errorResponse.status === 401) {
            this.message = 'You are unauthorized';
            this.messageTitle =
              errorResponse.error &&
              errorResponse.status + ' ' + 'unauthorized';
            this._authService.logout();
          } else if (errorResponse.status === 403) {
            this.message =
              "You don't have permission to access the data for this page";
            this.messageTitle =
              errorResponse.error &&
              errorResponse.status + ' ' + 'unauthorized';
          }
          if (errorResponse.status === 409) {
            this.message = 'Application with same name already exists';
          }
        }
        this.toasterService.error(this.message, this.messageTitle);
        this.loaderService.displayLoader(false);
        return throwError(message);
      })
    );
  }
}
