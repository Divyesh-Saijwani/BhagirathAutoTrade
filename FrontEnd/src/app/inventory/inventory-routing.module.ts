import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { InventoryComponent } from './inventory.component';
import { InventoryFilterComponent } from './inventory-filter/inventory-filter.component';
import { InventoryAddComponent } from './inventory-add/inventory-add.component';

const routes: Routes = [
  {
    path:'',
    component:InventoryComponent,
    children:[
      {
        path:'filer',
        component:InventoryFilterComponent
      },
      {
        path:'add',
        component:InventoryAddComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class InventoryRoutingModule { }
