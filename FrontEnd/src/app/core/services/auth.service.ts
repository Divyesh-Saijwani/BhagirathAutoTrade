import { Injectable } from '@angular/core';
import { RequestLoginDetails } from '../model/common.model';
import { environment } from '../../../environments/environment';
import { HttpCommonService } from './http-common.service';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';
import { HttpHeaders } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  currentUserRole: any = 3;
  userDetails: any;
  basedUrl: string = environment.serverUrl;
  constructor(private http: HttpCommonService, private routing: Router) {}

  loginUsers(loginDetails: RequestLoginDetails): Observable<any> {
    return this.http.httpPostRequest(
      // `${this.basedUrl}/account/login`,
      `${this.basedUrl}/login`,
      loginDetails
    );
  }

  public setTokenData(value: any) {
    localStorage.setItem('tokens', JSON.stringify(value))
  }
  
  logout(): void {
    localStorage.setItem('loggedIn', 'false');
    localStorage.removeItem('userDetails');
    this.routing.navigate(['login']);
  }

  setUserDetails(userDetails: any) {
    this.userDetails = userDetails;
    localStorage.setItem('loggedIn', 'true');
    this.setUserRole(this.userDetails.role)
    localStorage.setItem('userDetails', JSON.stringify(userDetails));
  }
  getAuthorizationHeaderValue() {
    try{
      if(localStorage){
        let token = JSON.parse(localStorage?.getItem('tokens') || '');
        return token?.accessToken;  
      }  
    }catch{
      return '';
    }
  }

  getLoggedinUserDetails(){
    return JSON.parse(localStorage.getItem('userDetails') || '');
  }

  getUserDetails(accessToken:string) {
    var headers_object = new HttpHeaders().set("Authorization", "Bearer " + accessToken);
    const httpOptions = {
      headers: headers_object
    };
    return this.http.httpGetRequest(`${this.basedUrl}/account/GetUserDetails`, httpOptions);
  }

  isLoggedIn(): boolean {
    return localStorage.getItem('loggedIn') === 'true';
  }
  setUserRole(role: string) {
    // let userDetails=JSON.parse(localStorage.getItem('userDetails') || '');
    this.currentUserRole = 1;
  }
  
  hasAnyRequiredRole(roles: string[]): boolean {
    console.log('role',this.currentUserRole);
    console.log('con',roles.includes(this.currentUserRole));
    
    return roles.includes(this.currentUserRole);
  }

  getAccessControls() {
    return [
      {
        module_name: 'users',
        create_action: false,
        read_action: true,
        update_action: true,
        delete_action: false,
      },
      {
        module_name: 'customer',
        create_action: true,
        read_action: true,
        update_action: false,
        delete_action: false,
      },
    ];
  }
}
