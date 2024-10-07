import { Component, HostBinding } from '@angular/core';

@Component({
  selector: 'app-step-two',
  templateUrl: './step-two.component.html',
  styleUrl: './step-two.component.scss'
})
export class StepTwoComponent {
  @HostBinding('class') public classes: string = 'd-flex flex-column h-100 overflow-hidden';
}
