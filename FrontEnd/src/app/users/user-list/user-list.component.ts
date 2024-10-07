import { Component, HostBinding } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UsersService } from '../../shared/services/users.service';
import { AdminGroupList, Users, roleList, userGroupList } from '../../core/model/common.model';
import { UserAlgoData } from '../../shared/models/UserAlgoData';
import { StocksService } from '../../shared/services/stocks.service';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.scss',
})
export class UserListComponent {
  @HostBinding('class') public classes: string =
    'd-flex flex-column h-100 overflow-hidden';
  public userAlgoData !: UserAlgoData[];
  searchText: string = '';
  constructor(
    private route: ActivatedRoute,
    private routing: Router,
    private usersService: UsersService,
    private stockService: StocksService
  ) {
    this.stockService.refreshEvent.subscribe((data:any)=>{
      this.getAll();
    })
    this.getAll();
  }
  
  getAll() {
    this.stockService.getAlgoData().subscribe((res:UserAlgoData[])=>{
      if(res){
        this.userAlgoData=res;
      }else{
        this.userAlgoData=[];
      }
    })
  }

  edit(value: any) {
    //this.routing.navigate(['users/edit', value]);
  }

  delete(value: any) {
    // this.usersService.deleteUser(value).subscribe((res: Users) => {
    //   this.getAll();
    // });
  }
}
