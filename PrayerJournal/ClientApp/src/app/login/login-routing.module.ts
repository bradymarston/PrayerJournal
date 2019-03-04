import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { extract, AuthenticationGuard } from '@app/core';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { ConfirmEmailComponent } from './confirm-email/confirm-email.component';
import { EmailNotConfirmedComponent } from './email-not-confirmed/email-not-confirmed.component';
import { ForgotPasswordComponent } from './forgot-password/forgot-password.component';
import { ConfirmForgotPasswordComponent } from './confirm-forgot-password/confirm-forgot-password.component';
import { ResetPasswordComponent } from './reset-password/reset-password.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent, data: { title: extract('Login') } },
  { path: 'register', component: RegisterComponent, data: { title: extract('Register') } },
  { path: 'confirm-email', component: ConfirmEmailComponent, data: { title: extract('Confirm Email') } },
  { path: 'email-not-confirmed', component: EmailNotConfirmedComponent, data: { title: extract('Confirm Your Email') }, canActivate: [AuthenticationGuard] },
  { path: 'forgot-password', component: ForgotPasswordComponent, data: { title: extract('Forgot Password') } },
  { path: 'confirm-forgot-password', component: ConfirmForgotPasswordComponent, data: { title: extract('Password Reset Request Reseived') } },
  { path: 'reset-password', component: ResetPasswordComponent, data: { title: extract('Reset Password') } }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers: []
})
export class LoginRoutingModule { }
