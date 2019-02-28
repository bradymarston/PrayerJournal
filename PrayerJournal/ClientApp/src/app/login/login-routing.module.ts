import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { extract, AuthenticationGuard } from '@app/core';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { ConfirmEmailComponent } from './confirm-email/confirm-email.component';
import { EmailNotConfirmedComponent } from './email-not-confirmed/email-not-confirmed.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent, data: { title: extract('Login') } },
  { path: 'register', component: RegisterComponent, data: { title: extract('Register') } },
  { path: 'confirm-email', component: ConfirmEmailComponent, data: { title: extract('Confirm Email') } },
  { path: 'email-not-confirmed', component: EmailNotConfirmedComponent, data: { title: extract('Confirm Your Email'), caveat: 'ConfirmEmail' }, canActivate: [AuthenticationGuard] }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers: []
})
export class LoginRoutingModule { }
