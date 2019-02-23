import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { extract } from '@app/core';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { Shell } from '@app/shell/shell.service';

const routes: Routes = [
  Shell.childRoutes([
    { path: '', redirectTo: '/change-password', pathMatch: 'full' },
    { path: 'change-password', component: ChangePasswordComponent, data: { title: extract('change-password') } }
  ])
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers: []
})
export class AccountRoutingModule { }
