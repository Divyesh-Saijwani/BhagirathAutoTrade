import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OverlayComponent } from './components/overlay/overlay.component';
import { RouterModule } from '@angular/router';
import { SerchPipe } from './pipes/serch.pipe';
import { UsersService } from './services/users.service';
import { PhoneNumberMaskDirective } from './directives/phone-number-mask.directive';
import { AccessControlDirective } from './directives/access-control.directive';

@NgModule({
  declarations: [OverlayComponent, SerchPipe, PhoneNumberMaskDirective,AccessControlDirective],
  imports: [CommonModule, RouterModule],
  exports: [OverlayComponent, SerchPipe, PhoneNumberMaskDirective,AccessControlDirective],
  providers: [UsersService],
})
export class SharedModule {}
