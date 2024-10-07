import { Component } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import {
  AdminGroupList,
  roleList,
  userGroupList,
} from '../../core/model/common.model';
import { OverlayInputConfig } from '../../shared/components/overlay/overlay.model';
import { UsersService } from '../../shared/services/users.service';
import { AuthService } from '../../core/services/auth.service';
import { identity } from 'rxjs';

@Component({
  selector: 'app-customer-add',
  templateUrl: './customer-add.component.html',
  styleUrl: './customer-add.component.scss',
})
export class CustomerAddComponent {
  /** provides the configuration for All the inputs. */
  public inputConfig!: OverlayInputConfig;
  public userGroupsData: any = userGroupList;
  public roleDetails: any = roleList;
  public roleLists: any = [];
  public isBuilder: boolean = false;
  public customerFormGroup: FormGroup;
  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private routing: Router,
    private usersService: UsersService,
    private authService: AuthService
  ) {
    let userDetails = this.authService.getLoggedinUserDetails();
    this.customerFormGroup = this.fb.group({
      identity: this.fb.group({
        id: [],
        email: [],
        password: [],
        FullName: [],
        Address: [],
        City: [],
        State: [],
        Country: [],
        postalCode: [],
        leadSource: [],
        stopNotification: [],
        branch: [],
        contact: [],
        phone1: [],
        phone2: [],
        phone3: [],
        fax: [],
        suppliedQuoteTaxGroupsProduct: [],
        suppliedQuoteTaxGroupsService: [],
        installedQuoteTaxGroupsProduct: [],
        installedQuoteTaxGroupsService: [],
        userGroup: [],
        role: [],
        markUp: [],
      }),
      quoting: this.fb.group({
        quoteType: [''],
        signatureLine: [''],
        priceMultiplire: [],
        defaultFactor1Installed: [],
        defaultReason1Installed: [''],
        defaultFactor1SupplyOnly: [],
        defaultReason1SupplyOnly: [''],
        discountAtItemLevel: [''],
        creditStatus: [''],
        creditLimit: [],
        depositeRequired: [],
        invoiceTerms: [''],
        quoteTerms: [''],
        adagioCode: [''],
        inActive: [''],
        representative: [''],
        note: [''],
      }),
    });
    this.inputConfig = {
      title: 'Add Customer Details',
      isSubmitEnable: true,
      isFormTouched: false,
    };
  }

  changeUserGroup(selectedValue: any) {
    this.roleLists = [];
    this.isBuilder = selectedValue.value === 2 ? true : false;
    this.customerFormGroup.get('role')?.setValue('');
    this.roleLists = this.roleDetails.find(
      (el: any) => el.userGroupType === selectedValue.value
    ).roles;
  }

  onCancel() {
    this.usersService.setUserDetails(true);
    this.routing.navigate(['../customer']);
  }

  onSubmit() {
    this.usersService.addUsers(this.customerFormGroup.value).subscribe((res) => {
      this.routing.navigate(['../customer']);
      this.usersService.setUserDetails(true);
    });
  }
}
