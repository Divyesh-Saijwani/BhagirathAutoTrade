import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { QuotationsRoutingModule } from './quotations-routing.module';
import { QuotationsComponent } from './quotations.component';
import { RouterModule } from '@angular/router';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgSelectModule } from '@ng-select/ng-select';
import { QuotationListComponent } from './quotation-list/quotation-list.component';
import { StepTwoComponent } from './step-two/step-two.component';
import { StepThreeComponent } from './step-three/step-three.component';

import { NgbAccordionModule } from '@ng-bootstrap/ng-bootstrap';
import { LoadUserComponent } from './load-user/load-user.component';
import { QuotationsService } from './quotations.service';
import { QuotationHeadersComponent } from './quotation-headers/quotation-headers.component';
import { ChooseCustomerComponent } from './choose-customer/choose-customer.component';


@NgModule({
  declarations: [
    QuotationsComponent,
    QuotationListComponent,
    StepTwoComponent,
    StepThreeComponent,
    LoadUserComponent,
    QuotationHeadersComponent,
    ChooseCustomerComponent
  ],
  imports: [
    RouterModule,
    CommonModule,
    SharedModule,
    FormsModule,
    NgbAccordionModule,
    ReactiveFormsModule,
    NgSelectModule,
    QuotationsRoutingModule
  ],
  providers:[
    QuotationsService
  ]
})
export class QuotationsModule { }
