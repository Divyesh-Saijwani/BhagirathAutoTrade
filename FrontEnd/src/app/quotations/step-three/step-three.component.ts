import { Component, HostBinding } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { OverlayInputConfig } from '../../shared/components/overlay/overlay.model';

@Component({
  selector: 'app-step-three',
  templateUrl: './step-three.component.html',
  styleUrl: './step-three.component.scss'
})
export class StepThreeComponent {
  items = ['First', 'Second', 'Third'];
  @HostBinding('class') public classes: string = 'd-flex flex-column h-100 overflow-hidden';
  /** provides the configuration for All the inputs. */
  public inputConfig!: OverlayInputConfig;
  public categoryList: any = ["Window", "Entry Doors", "Glass"];
  public yourItems: any = [1, 2, 3]
  public QuotationFormGroup: FormGroup;
  constructor(private fb: FormBuilder) {
    this.QuotationFormGroup = this.fb.group({
      email: ['', [Validators.email]],
      password: [''],
      fullName: [''],
      contactNo: [''],
      QuotationGroup: [''],
      role: ['']
    });
    this.inputConfig = {
      title: 'Add Quotation Details',
      isSubmitEnable: true,
      isFormTouched: false,
    };
  }
  public demoItems: any[] = [
    { id: 1, name: 'Glass Sealed Unit',  imgPath: 'https://sv10.fenestratio.com/fenestratio/images/5/v_5_11.jpg' },
    { id: 2, name: 'Miscellaneous Miscellaneous', imgPath:'https://sv10.fenestratio.com/fenestratio/images/5/v_5_520.jpg' },
    { id: 3, name: 'Windows Fixed Csmnt Fixed Csmt', imgpath: 'https://sv10.fenestratio.com/fenestratio/images/5/v_5_592.jpg' },
    { id: 4, name: 'Windows Picture SLDR Picture', imgpath: 'https://sv10.fenestratio.com/fenestratio/images/5/v_5_479.jpg' },
    { id: 5, name: 'Entry Doors Entry Doors' }
  ];
  onCancel() { }
  onSubmit() { }
}
