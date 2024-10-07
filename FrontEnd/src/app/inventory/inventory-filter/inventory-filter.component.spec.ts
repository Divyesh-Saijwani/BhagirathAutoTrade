import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InventoryFilterComponent } from './inventory-filter.component';

describe('InventoryFilterComponent', () => {
  let component: InventoryFilterComponent;
  let fixture: ComponentFixture<InventoryFilterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [InventoryFilterComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(InventoryFilterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
