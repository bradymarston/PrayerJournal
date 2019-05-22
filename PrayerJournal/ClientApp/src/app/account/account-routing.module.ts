import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { extract } from '@app/core';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { ProfileComponent } from './profile/profile.component';

const routes: Routes = [
  // Module is lazy loaded, see app-routing.module.ts
  { path: 'change-password', component: ChangePasswordComponent, data: { title: extract('Change Password') } },
  { path: 'profile', component: ProfileComponent, data: { title: extract('Edit Profile') } }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers: []
})
export class AccountRoutingModule { }
