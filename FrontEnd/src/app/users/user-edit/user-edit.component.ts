import { Component } from '@angular/core';
import { OverlayInputConfig } from '../../shared/components/overlay/overlay.model';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AdminGroupList, roleList, userGroupList } from '../../core/model/common.model';
import { UsersService } from '../../shared/services/users.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-user-edit',
  templateUrl: './user-edit.component.html',
  styleUrl: './user-edit.component.scss',
})
export class UserEditComponent {
  /** provides the configuration for All the inputs. */
  public inputConfig!: OverlayInputConfig;
  public userGroupsData: any = AdminGroupList;
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
    });
    this.inputConfig = {
      title: 'Edit User Details',
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
    this.routing.navigate(['../apis']);
  }

  onSubmit() {
    this.usersService.editUsers(this.userFormGroup.value).subscribe((res) => {
      this.routing.navigate(['../apis']);
      this.usersService.setUserDetails(true);
    });
  }
}
