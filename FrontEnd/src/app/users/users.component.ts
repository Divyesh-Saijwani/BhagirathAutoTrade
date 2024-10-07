import { Component, HostBinding, OnInit } from '@angular/core';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit {
  @HostBinding('class') public classes: string = 'd-flex flex-column h-100 overflow-hidden';
  constructor() { }

  ngOnInit(): void {
  }

}
