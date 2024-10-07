import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UsersRoutingModule } from './users-routing.module';
import { UsersComponent } from './users.component';
import { UserListComponent } from './user-list/user-list.component';
import { UserAddComponent } from './user-add/user-add.component';
import { SharedModule } from '../shared/shared.module';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgSelectModule } from '@ng-select/ng-select';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { UsersService } from '../shared/services/users.service';
import { UserEditComponent } from './user-edit/user-edit.component';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@NgModule({
  declarations: [UsersComponent, UserListComponent, UserAddComponent,UserEditComponent],
  imports: [
    CommonModule,
    RouterModule,
    UsersRoutingModule,
    SharedModule,
    ReactiveFormsModule,
    NgSelectModule,
    FormsModule,
    MatAutocompleteModule,
    MatInputModule,
    MatFormFieldModule,
    // BrowserAnimationsModule,
    BsDatepickerModule.forRoot(),
  ],
 
})
export class UsersModule {}
