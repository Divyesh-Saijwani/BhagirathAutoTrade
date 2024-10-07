import { Component, HostBinding } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UsersService } from '../../shared/services/users.service';
import { QuotationsService } from '../quotations.service';

@Component({
  selector: 'app-quotation-list',
  templateUrl: './quotation-list.component.html',
  styleUrl: './quotation-list.component.scss'
})
export class QuotationListComponent {
  @HostBinding('class') public classes: string = 'd-flex flex-column h-100 overflow-hidden';

  searchText: string = '';
  public quotationList:any;
  constructor(
    private route: ActivatedRoute,
    private routing: Router,
    private quotationsService: QuotationsService
  ) {
    this.getAll();
  }
  getAll() {
    this.quotationsService.getAllQuotations().subscribe((res: any) => {
      debugger
      this.quotationList = res?.body;
    });
  }
  viewQuotation(value:any){
    this.routing.navigate(['/quotation/view-quotation'])
  }
  headerQuotation(value:any){
    this.routing.navigate(['/quotation/headers'])
  }
  downlodQuotation(value:any){

  }
}
