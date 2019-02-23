import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { FlexLayoutModule } from '@angular/flex-layout';

import { CoreModule } from '@app/core';
import { SharedModule } from '@app/shared';
import { MaterialModule } from '@app/material.module';
import { AccountRoutingModule } from './account-routing.module';
import { ChangePasswordComponent } from './change-password/change-password.component';

@NgModule({
  imports: [
    CommonModule,
    TranslateModule,
    CoreModule,
    SharedModule,
    FlexLayoutModule,
    MaterialModule,
    AccountRoutingModule
  ],
  declarations: [
    ChangePasswordComponent
  ]
})
export class AccountModule { }
