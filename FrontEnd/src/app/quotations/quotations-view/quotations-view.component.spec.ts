import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QuotationsViewComponent } from './quotations-view.component';

describe('QuotationsViewComponent', () => {
  let component: QuotationsViewComponent;
  let fixture: ComponentFixture<QuotationsViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [QuotationsViewComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(QuotationsViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
