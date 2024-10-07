import { Directive, ElementRef, Input } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';

@Directive({
  selector: '[appAccessControl]',
})
export class AccessControlDirective {
  @Input('moduleType') moduleType: string | any;
  @Input('accessType') accessType: string | any;
  constructor(private elementRef: ElementRef, private auth: AuthService) {}

  ngOnInit() {
    this.elementRef.nativeElement.style.display = 'none';
    this.checkAccess();
  }
  checkAccess() {
    const accessControls: any = this.auth.getAccessControls();
    const module: any = accessControls.find(
      (access: any) => access.module_name === this.moduleType
    );
    this.elementRef.nativeElement.style.display = module[this.accessType]
      ? 'block'
      : 'none';
  }
}
