import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-master',
  templateUrl: './master.component.html',
  styleUrls: ['./master.component.scss'],
})
export class MasterComponent implements OnInit {
  /**
   * provides the loader status.
   */
  public loaderStatus: boolean = true;
  constructor() {}

  ngOnInit(): void {}
}
