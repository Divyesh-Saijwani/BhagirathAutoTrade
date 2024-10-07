import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { QuotationsComponent } from './quotations.component';
import { QuotationListComponent } from './quotation-list/quotation-list.component';
import { LoadUserComponent } from './load-user/load-user.component';
import { StepThreeComponent } from './step-three/step-three.component';
import { QuotationsViewComponent } from './quotations-view/quotations-view.component';
import { QuotationHeadersComponent } from './quotation-headers/quotation-headers.component';
import { ChooseCustomerComponent } from './choose-customer/choose-customer.component';

const routes: Routes = [
  {
    path: '',
    component: QuotationsComponent,
    children: [
      {
        path:'',
        pathMatch:'full',
        redirectTo:'list'
      },
      {
        path:'list',
        component:QuotationListComponent
      },
      {
        path: 'load-profile',
        component: LoadUserComponent
      },
      {
        path: 'headers',
        component: QuotationHeadersComponent
      },
      {
        path: 'select-product/:id',
        component: StepThreeComponent
      },
      {
        path: 'add',
        component: ChooseCustomerComponent
      },
      {
        path: 'view-quotation',
        component: QuotationsViewComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class QuotationsRoutingModule { }
