import { Component, HostBinding } from '@angular/core';

@Component({
  selector: 'app-inventory',
  templateUrl: './inventory.component.html',
  styleUrl: './inventory.component.scss'
})
export class InventoryComponent {
  @HostBinding('class') public classes: string = 'd-flex flex-column h-100 overflow-hidden';

  public isOpenFilter:boolean = true

}
