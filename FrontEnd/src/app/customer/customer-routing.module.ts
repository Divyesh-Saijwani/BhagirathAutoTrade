import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CustomerComponent } from './customer.component';
import { CustomerAddComponent } from './customer-add/customer-add.component';
import { CustomerEditComponent } from './customer-edit/customer-edit.component';

const routes: Routes = [
  {
    path:'',
    component:CustomerComponent,
    children:[
      {
        path:'add',
        component:CustomerAddComponent
      },
      {
        path:'edit/:id',
        component:CustomerEditComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CustomerRoutingModule { }
