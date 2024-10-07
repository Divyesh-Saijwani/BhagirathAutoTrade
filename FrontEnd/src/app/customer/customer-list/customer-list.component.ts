import { Component, HostBinding } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Users, userGroupList, roleList } from '../../core/model/common.model';
import { UsersService } from '../../shared/services/users.service';

@Component({
  selector: 'app-customer-list',
  templateUrl: './customer-list.component.html',
  styleUrl: './customer-list.component.scss',
})
export class CustomerListComponent {
  @HostBinding('class') public classes: string =
    'd-flex flex-column h-100 overflow-hidden';
  public userList: Users[] | any = [];
  public userGroupsData: any = userGroupList;
  public roleDetails: any = roleList;
  searchText: string = '';
  constructor(
    private route: ActivatedRoute,
    private routing: Router,
    private usersService: UsersService
  ) {
    this.usersService.getUserDetails().subscribe((el) => {
      this.getAll();
    });
    this.getAll();
  }
  getAll() {
    this.usersService.getAllUsers(this.roleDetails[1].userGroupType).subscribe((res: any) => {
      this.userList = [];
      let data = res?.map((el: any) => {
        let findUser = this.userGroupsData.find(
          (item: any) => item.value === el.userGroup
        );
        el.userGroup = findUser?.label;
        let findRole = this.roleDetails.find(
          (item: any) => item.userGroupType === findUser?.value
        );
        let findRoleItems = findRole?.roles.find(
          (itemOfRole: any) => itemOfRole.value === el.role
        );
        el.role = findRoleItems?.label;
        this.userList.push(el);
      });
    });
  }

  edit(value: any) {
    this.routing.navigate(['customer/edit', value]);
  }

  delete(value: any) {
    this.usersService.deleteUser(value).subscribe((res: Users) => {
      this.getAll();
    });
  }
}
