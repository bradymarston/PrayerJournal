import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FlexLayoutModule } from '@angular/flex-layout';

import { MaterialModule } from '@app/material.module';
import { LoaderComponent } from './loader/loader.component';
import { ValidationWarningComponent } from './validation-warning/validation-warning.component';

@NgModule({
  imports: [
    FlexLayoutModule,
    MaterialModule,
    CommonModule
  ],
  declarations: [
    LoaderComponent,
    ValidationWarningComponent
  ],
  exports: [
    LoaderComponent,
    ValidationWarningComponent
  ]
})
export class SharedModule { }
