import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MasterComponent } from './core/components/master/master.component';
import { LoginComponent } from './core/components/login/login.component';
import { RegisterComponent } from './core/components/register/register.component';
import { NotFoundComponent } from './core/components/not-found/not-found.component';
import { AuthGuard } from './core/guards/auth.guard';

const routes: Routes = [
  {
    path: '',
    component: MasterComponent,
    canActivate:[AuthGuard],
    children: [
      {
        path:'',
        pathMatch:'full',
        redirectTo:'dashboard'
      },
      {
        path: 'dashboard',
        loadChildren: () =>
          import('./dashboard/dashboard.module').then((m) => m.DashboardModule),
      },
      {
        path: 'broker',
        loadChildren: () =>
          import('./broker/broker.module').then((m) => m.BrokerModule),
      },
      {
        path: 'apis',
        loadChildren: () =>
          import('./users/users.module').then((m) => m.UsersModule),
      },
      {
        path: 'quotation',
        loadChildren: () =>
          import('./quotations/quotations.module').then(
            (m) => m.QuotationsModule
          ),
      },
      {
        path: 'customer',
        loadChildren: () =>
          import('./customer/customer.module').then(
            (m) => m.CustomerModule
          ),
      },
      {
        path: 'inventory',
        loadChildren: () =>
          import('./inventory/inventory.module').then((m) => m.InventoryModule),
      },
      { path: 'configure', 
        loadChildren: () => 
          import('./configure/configure.module').then(m => m.ConfigureModule) },
    ],
  },
  {
    path: 'login',
    component: LoginComponent,
  },
  {
    path: 'register',
    component: RegisterComponent,
  },
  { path: '**', redirectTo: '404' },
  {
    path: '404',
    component: NotFoundComponent,
  },
  { path: 'configure', loadChildren: () => import('./configure/configure.module').then(m => m.ConfigureModule) },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
