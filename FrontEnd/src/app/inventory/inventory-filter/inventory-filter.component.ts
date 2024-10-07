import { Component } from '@angular/core';
import { OverlayInputConfig } from '../../shared/components/overlay/overlay.model';

@Component({
  selector: 'app-inventory-filter',
  templateUrl: './inventory-filter.component.html',
  styleUrl: './inventory-filter.component.scss',
})
export class InventoryFilterComponent {
  public yourItems = [1, 2, 3];
  public inputConfig!: OverlayInputConfig;
  constructor(){
    this.inputConfig = {
      title: 'Filter',
      isSubmitEnable: true,
      isFormTouched: false,
    };
  }
  
  onCancel() {}
  onSubmit() {}
}
