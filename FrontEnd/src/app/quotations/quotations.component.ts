import { Component, HostBinding } from '@angular/core';
import { QuotationsService } from './quotations.service';

@Component({
  selector: 'app-quotations',
  templateUrl: './quotations.component.html',
  styleUrl: './quotations.component.scss',
})
export class QuotationsComponent {
  @HostBinding('class') public classes: string =
    'd-flex flex-column h-100 overflow-hidden';
  constructor(private quotationsService: QuotationsService) {}

  ngOnInit(): void {}
  add() {
    this.quotationsService.addQuotations().subscribe((ele) => {
      ;
    });
  }
}
