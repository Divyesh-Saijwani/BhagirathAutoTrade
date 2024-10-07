import { Component } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { userGroupList, roleList } from '../../core/model/common.model';
import { OverlayInputConfig } from '../../shared/components/overlay/overlay.model';
import { UsersService } from '../../shared/services/users.service';

@Component({
  selector: 'app-inventory-add',
  templateUrl: './inventory-add.component.html',
  styleUrl: './inventory-add.component.scss',
})
export class InventoryAddComponent {
  /** provides the configuration for All the inputs. */
  public inputConfig!: OverlayInputConfig;
  public userGroupsData: any = userGroupList;
  public supplierDetails: any = roleList;
  public wareHouseDetails: any = [];
  public isBuilder: boolean = false;
  public inventoryFormGroup: FormGroup;
  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private routing: Router,
    private usersService: UsersService
  ) {
    this.inventoryFormGroup = this.fb.group({
      code: [''],
      description: [''],
      supplier: [''],
      warehouse: [''],
      uoM: [''],
      avgCost: [''],
      availableQty: [''],
      allocatedQty: [''],
      requiredQty: [''],
      createdBy: [''],
    });
    this.inputConfig = {
      title: 'Add Inventory Details',
      isSubmitEnable: true,
      isFormTouched: false,
    };
  }

  changeUserGroup(selectedValue: any) {
   
  }

  onCancel() {}

  onSubmit() {
    this.usersService.addUsers(this.inventoryFormGroup.value).subscribe((res) => {
      this.routing.navigate(['../users']);
    });
  }
}
