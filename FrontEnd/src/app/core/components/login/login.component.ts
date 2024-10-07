import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router } from '@angular/router';
import { Roles } from '../../model/common.model';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
  public loginForm: FormGroup;
  userDetails: any;
  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private toasterService: ToastrService,
    private route: ActivatedRoute,
    private routing: Router,
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required]],
      password: ['', [Validators.required]],
    });
  }

  ngOnInit(): void {}

  login() {
    if (this.loginForm.valid) {        
      this.authService.loginUsers(this.loginForm.value).subscribe((ex) => {
        this.authService.setTokenData(ex);
        this.authService.getUserDetails(ex.accessToken).subscribe((userDetails: any) => {
          this.authService.setUserDetails(userDetails);
          this.routing.navigate(['dashboard'])  
        })
      });
    }
  }
}
