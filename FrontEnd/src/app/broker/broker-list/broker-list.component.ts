import { Component, HostBinding } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Users, userGroupList, roleList } from '../../core/model/common.model';
import { UsersService } from '../../shared/services/users.service';
import { BrokerService } from '../../shared/services/broker.service';
import { Broker } from '../model/broker';

@Component({
  selector: 'app-broker-list',
  templateUrl: './broker-list.component.html',
  styleUrl: './broker-list.component.scss',
})
export class BrokerListComponent {
  @HostBinding('class') public classes: string =
    'd-flex flex-column h-100 overflow-hidden';
  public brokerList: Broker[] | any = [];
  public userGroupsData: any = userGroupList;
  public roleDetails: any = roleList;
  searchText: string = '';
  constructor(
    private route: ActivatedRoute,
    private routing: Router,
    private usersService: UsersService,
    private brokerService: BrokerService
  ) {
    this.getAll();
  }
  getAll() {
    this.brokerService.getAllBrokers().subscribe((res: Broker[]) => {
      this.brokerList = res;
    });
  }

  edit(value: string) {
    this.routing.navigate(['broker/edit', value]);
  }

  delete(value: string) {
    this.brokerService.deleteBroker(value).subscribe((res: any) => {
      this.getAll();
    });
  }
}
