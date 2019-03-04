import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfirmForgotPasswordComponent } from './confirm-forgot-password.component';

describe('ConfirmForgotPasswordComponent', () => {
  let component: ConfirmForgotPasswordComponent;
  let fixture: ComponentFixture<ConfirmForgotPasswordComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConfirmForgotPasswordComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfirmForgotPasswordComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
