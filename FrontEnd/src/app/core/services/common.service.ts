import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { Menus } from '../model/menu.model';
import { HttpCommonService } from './http-common.service';
import { environment } from '../../../environments/environment';
import { Roles } from '../model/common.model';

@Injectable()
export class CommonService {
  baseUrl: string = environment.serverUrl;
  constructor(private httpCommonService: HttpCommonService) {}

  getMenuList(): Observable<Menus[]> {
    return of(this.getMenuDetails());
    // return this.httpCommonService.httpGetRequest(`${this.baseUrl}/menus`);
  }

  getMenuDetails() {
    return [
      {
        id: 6,
        menuTitle: 'Dashboard',
        menuLink: '/dashboard',
        roles: [
          Roles.Admin,
          Roles.Associate,
          Roles.Trader
        ],
        active: true,
        subMenu: [],
        iconClass: 'fi fi-rs-table-rows',
        headerTitle: 'Dashboard',
      },
      {
        id: 1,
        menuTitle: 'Customer Algo',
        menuLink: '/apis',
        roles: [Roles.Admin],
        active: true,
        subMenu: [],
        iconClass: 'fi fi-tr-inventory-alt',
        headerTitle: 'APIs Details',
      },
      {
        id: 2,
        menuTitle: 'Configuration',
        menuLink: '/configure',
        roles: [
          Roles.Admin
        ],
        active: true,
        subMenu: [],
        iconClass: 'fi fi-rr-block-quote',
        headerTitle: 'Configuration Details',
      },
      {
        id: 3,
        menuTitle: 'Users',
        menuLink: '/customer',
        roles: [
          Roles.Admin,
          Roles.Associate
        ],
        active: true,
        subMenu: [],
        iconClass: 'fi fi-tr-circle-user',
        headerTitle: 'Customer Details',
      },
      // {
      //   id: 4,
      //   menuTitle: 'Inventory',
      //   menuLink: '/inventory',
      //   roles: [
      //       Roles.Admin,
      //     ],
      //   active: true,
      //   subMenu: [],
      //   iconClass: 'fi fi-tr-inventory-alt',
      //   headerTitle: 'Inventory Details',
      // },
      {
        id: 5,
        menuTitle: 'Brokers',
        menuLink: '/broker',
        roles: [
            Roles.Admin,
          ],
        active: true,
        subMenu: [],
        iconClass: 'fi fi-tr-box-open',
        headerTitle: 'Broker Details',
      },
    ];
  }
}
