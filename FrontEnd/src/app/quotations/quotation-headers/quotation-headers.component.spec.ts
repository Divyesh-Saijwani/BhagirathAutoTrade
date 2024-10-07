import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QuotationHeadersComponent } from './quotation-headers.component';

describe('QuotationHeadersComponent', () => {
  let component: QuotationHeadersComponent;
  let fixture: ComponentFixture<QuotationHeadersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [QuotationHeadersComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(QuotationHeadersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
