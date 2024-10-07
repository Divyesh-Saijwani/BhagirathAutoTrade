import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LoadUserComponent } from './load-user.component';

describe('LoadUserComponent', () => {
  let component: LoadUserComponent;
  let fixture: ComponentFixture<LoadUserComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LoadUserComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(LoadUserComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
