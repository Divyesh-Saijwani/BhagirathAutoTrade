import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { UsersComponent } from './users.component';
import { UserListComponent } from './user-list/user-list.component';
import { UserAddComponent } from './user-add/user-add.component';
import { UserEditComponent } from './user-edit/user-edit.component';

const routes: Routes = [
  {
    path:'',
    component:UsersComponent,
    children:[
      // {
      //   path:'',
      //   pathMatch:'full',
      //   redirectTo:'list'
      // },
      // {
      //   path:'list',
      //   component:UserListComponent
      // },
      {
        path:'add',
        component:UserAddComponent
      },
      {
        path:'edit/:id',
        component:UserEditComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UsersRoutingModule { }
