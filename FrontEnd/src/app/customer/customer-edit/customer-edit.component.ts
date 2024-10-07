import { Component } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminGroupList, roleList, userGroupList } from '../../core/model/common.model';
import { OverlayInputConfig } from '../../shared/components/overlay/overlay.model';
import { UsersService } from '../../shared/services/users.service';

@Component({
  selector: 'app-customer-edit',
  templateUrl: './customer-edit.component.html',
  styleUrl: './customer-edit.component.scss'
})
export class CustomerEditComponent {
 /** provides the configuration for All the inputs. */
 public inputConfig!: OverlayInputConfig;
 public userGroupsData: any = userGroupList;
 public roleDetails: any = roleList;
 public roleLists: any = [];
 public isBuilder: boolean = false;
 public userFormGroup: FormGroup;
 public editUserId: string | any;
 constructor(
   private fb: FormBuilder,
   private route: ActivatedRoute,
   private routing: Router,
   private usersService: UsersService
 ) {
   this.route.params.subscribe((params) => {
     this.editUserId = params['id'];
   });
   this.userFormGroup = this.fb.group({
     email: [''],
     password: [''],
     fullName: [''],
     contactNo: [''],
     userGroup: [''],
     role: [''],
     markup: [''],
     address: [''],
     updatedBy:['']
   });
   this.inputConfig = {
     title: 'Edit Customer Details',
     isSubmitEnable: true,
     isFormTouched: false,
   };
   this.getById();
 }

 getById() {
   this.usersService.getUserById(this.editUserId).subscribe((res: any) => {
     ;
     this.userFormGroup.patchValue(res.body);
   });
 }
 changeUserGroup(selectedValue: any) {
   this.roleLists = [];
   this.isBuilder = selectedValue.value === 2 ? true : false;
   this.userFormGroup.get('role')?.setValue('');
   this.roleLists = this.roleDetails.find(
     (el: any) => el.userGroupType === selectedValue.value
   ).roles;
 }

 onCancel() {
   this.usersService.setUserDetails(true);
   this.routing.navigate(['../customer']);
 }

 onSubmit() {
   this.usersService.editUsers(this.userFormGroup.value).subscribe((res) => {
     this.routing.navigate(['../customer']);
     this.usersService.setUserDetails(true);
   });
 }
}
