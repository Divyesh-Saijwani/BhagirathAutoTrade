import { Component, HostBinding } from '@angular/core';

@Component({
  selector: 'app-quotations-view',
  standalone: true,
  imports: [],
  templateUrl: './quotations-view.component.html',
  styleUrl: './quotations-view.component.scss'
})
export class QuotationsViewComponent {
  @HostBinding('class') public classes: string = 'd-flex flex-column h-100 overflow-hidden';

}
