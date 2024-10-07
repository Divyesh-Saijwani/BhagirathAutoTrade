import { Component, OnInit } from '@angular/core';
import { CommonService } from '../../services/common.service';
import { Menus } from '../../model/menu.model';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent implements OnInit {

  sidebarList:Menus[]=[];
  constructor(private commonService:CommonService, public authService:AuthService) { }

  ngOnInit(): void {
    this.commonService.getMenuList().subscribe((el:Menus[])=>{
      this.sidebarList = el
    });
  }

  

}
