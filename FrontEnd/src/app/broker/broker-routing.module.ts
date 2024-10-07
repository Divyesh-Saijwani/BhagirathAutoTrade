import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { BrokerComponent } from './broker.component';

const routes: Routes = [
  {
    path:'',
    component:BrokerComponent,
    children:[
      // {
      //   path:'add',
      //   component:BrokerAddComponent
      // },
      // {
      //   path:'edit/:id',
      //   component:BrokerEditComponent
      // }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class BrokerRoutingModule { }
