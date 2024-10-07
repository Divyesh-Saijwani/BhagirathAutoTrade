import { Component, HostBinding, OnInit } from '@angular/core';

@Component({
  selector: 'app-broker',
  templateUrl: './broker.component.html',
  styleUrls: ['./broker.component.scss']
})
export class BrokerComponent implements OnInit {
  @HostBinding('class') public classes: string = 'd-flex flex-column h-100 overflow-hidden';
  constructor() { }

  ngOnInit(): void {
  }

}
