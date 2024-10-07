import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MasterComponent } from './components/master/master.component';
import { HeaderComponent } from './components/header/header.component';
import { SidebarComponent } from './components/sidebar/sidebar.component';
import { RouterModule } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { CommonService } from './services/common.service';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrModule } from 'ngx-toastr';
import { HttpInterceptorHandler } from './services/http.interceptor';
import { LoaderComponent } from './components/loader/loader.component';

@NgModule({
  declarations: [
    MasterComponent,
    HeaderComponent,
    SidebarComponent,
    LoginComponent,
    RegisterComponent,
    NotFoundComponent,
    LoaderComponent
  ],
  imports: [CommonModule, RouterModule,HttpClientModule,ReactiveFormsModule,ToastrModule.forRoot(),BrowserAnimationsModule],
  providers:[CommonService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpInterceptorHandler,
      multi: true,
    },]
})
export class CoreModule {}
