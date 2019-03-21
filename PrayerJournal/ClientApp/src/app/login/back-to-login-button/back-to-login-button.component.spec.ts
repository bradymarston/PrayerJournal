import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BackToLoginButtonComponent } from './back-to-login-button.component';

describe('BackToLoginButtonComponent', () => {
  let component: BackToLoginButtonComponent;
  let fixture: ComponentFixture<BackToLoginButtonComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BackToLoginButtonComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BackToLoginButtonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
