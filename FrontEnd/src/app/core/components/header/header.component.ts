import { Component, OnInit } from '@angular/core';
import { CommonService } from '../../services/common.service';
import { Menus } from '../../model/menu.model';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { Users } from '../../model/common.model';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
})
export class HeaderComponent implements OnInit {
  sidebarList: Menus[] = [];
  currentRoute!: string;
  public userDetails: any;
  constructor(
    private commonService: CommonService,
    private route: ActivatedRoute,
    private authService: AuthService
  ) {
    console.log(this.route);
    this.userDetails = this.authService.getLoggedinUserDetails();
    // this.currentRoute = this.route
  }

  ngOnInit(): void {
    this.commonService.getMenuList().subscribe((el: Menus[]) => {
      this.sidebarList = el;
    });
  }
  
  logout(){
    this.authService.logout()
  }
}
