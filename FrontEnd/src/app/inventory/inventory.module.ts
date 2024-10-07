import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { InventoryRoutingModule } from './inventory-routing.module';
import { InventoryComponent } from './inventory.component';
import { InventoryListComponent } from './inventory-list/inventory-list.component';
import { InventoryFilterComponent } from './inventory-filter/inventory-filter.component';
import { SharedModule } from '../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgSelectModule } from '@ng-select/ng-select';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { InventoryAddComponent } from './inventory-add/inventory-add.component';
import { InventoryService } from './inventory.service';


@NgModule({
  declarations: [
    InventoryComponent,
    InventoryListComponent,
    InventoryFilterComponent,
    InventoryAddComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    InventoryRoutingModule,
    ReactiveFormsModule,
    NgSelectModule,
    FormsModule,
    BsDatepickerModule.forRoot(),
  ],
  providers:[
    InventoryService
  ]
})
export class InventoryModule { }
