import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { FlexLayoutModule } from '@angular/flex-layout';

import { SharedModule } from '@app/shared';
import { MaterialModule } from '@app/material.module';
import { LoginRoutingModule } from './login-routing.module';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { ConfirmEmailComponent } from './confirm-email/confirm-email.component';
import { EmailNotConfirmedComponent } from './email-not-confirmed/email-not-confirmed.component';
import { ForgotPasswordComponent } from './forgot-password/forgot-password.component';
import { ConfirmForgotPasswordComponent } from './confirm-forgot-password/confirm-forgot-password.component';
import { ResetPasswordComponent } from './reset-password/reset-password.component';
import { BackToLoginButtonComponent } from './back-to-login-button/back-to-login-button.component';
import { ExternalLoginCallbackComponent } from './external-login-callback/external-login-callback.component';
import { ExternalLoginButtonComponent } from './external-login-button/external-login-button.component';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    TranslateModule,
    SharedModule,
    FlexLayoutModule,
    MaterialModule,
    LoginRoutingModule
  ],
  declarations: [
    LoginComponent,
    RegisterComponent,
    ConfirmEmailComponent,
    EmailNotConfirmedComponent,
    ForgotPasswordComponent,
    ConfirmForgotPasswordComponent,
    ResetPasswordComponent,
    BackToLoginButtonComponent,
    ExternalLoginCallbackComponent,
    ExternalLoginButtonComponent
  ]
})
export class LoginModule { }
