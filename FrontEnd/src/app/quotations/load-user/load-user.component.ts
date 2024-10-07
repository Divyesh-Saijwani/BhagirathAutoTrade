import { Component, HostBinding } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Users, roleList } from '../../core/model/common.model';
import { UsersService } from '../../shared/services/users.service';

@Component({
  selector: 'app-load-user',
  templateUrl: './load-user.component.html',
  styleUrl: './load-user.component.scss',
})
export class LoadUserComponent {
  @HostBinding('class') public classes: string =
    'd-flex flex-column h-100 overflow-hidden';
  public userList: Users | any;
  searchText: string = '';
  public roleDetails: any = roleList;
  constructor(
    private route: ActivatedRoute,
    private routing: Router,
    private usersService: UsersService
  ) {
    this.getAll();
  }
  getAll() {
    this.usersService.getAllUsers(this.roleDetails[1].userGroupType).subscribe((res: any) => {
      this.userList = res?.body;
    });
  }
  addQuotation(value:any){
    this.routing.navigate(['/quotation/select-product',value])
  }
}
