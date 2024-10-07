import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { ConfigurationListComponent } from './configuration-list/configuration-list.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { NgSelectModule } from '@ng-select/ng-select';
import { SharedModule } from '../shared/shared.module';
import { ConfigureComponent } from './configure.component';
import { ConfigureRoutingModule } from './configure-routing.module';

const routes: Routes = [
  { path: '', component: ConfigurationListComponent }
];

@NgModule({
  declarations: [
    ConfigureComponent,
    ConfigurationListComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    NgSelectModule,
    FormsModule,
    SharedModule,
    ConfigureRoutingModule
  ]
})
export class ConfigureModule { }
