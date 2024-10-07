import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { BrokerRoutingModule } from './broker-routing.module';
import { BrokerComponent } from './broker.component';
import { BrokerListComponent } from './broker-list/broker-list.component';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgSelectModule } from '@ng-select/ng-select';
import { SharedModule } from '../shared/shared.module';


@NgModule({
  declarations: [
    BrokerComponent,
    BrokerListComponent],
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    NgSelectModule,
    FormsModule,
    SharedModule,
    BrokerRoutingModule
  ]
})
export class BrokerModule { }
