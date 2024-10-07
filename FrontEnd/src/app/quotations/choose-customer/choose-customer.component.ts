import { Component, HostBinding } from '@angular/core';

@Component({
  selector: 'app-choose-customer',
  templateUrl: './choose-customer.component.html',
  styleUrl: './choose-customer.component.scss'
})
export class ChooseCustomerComponent {
  @HostBinding('class') public classes: string = 'd-flex flex-column h-100 overflow-hidden';
}
