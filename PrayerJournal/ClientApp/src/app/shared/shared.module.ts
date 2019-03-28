import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FlexLayoutModule } from '@angular/flex-layout';

import { MaterialModule } from '@app/material.module';
import { LoaderComponent } from './loader/loader.component';
import { ValidationWarningComponent } from './validation-warning/validation-warning.component';
import { ServerErrorsComponent } from './server-errors/server-errors.component';
import { HighlightSubstringComponent } from './highlight-substring/highlight-substring.component';

@NgModule({
  imports: [
    FlexLayoutModule,
    MaterialModule,
    CommonModule
  ],
  declarations: [
    LoaderComponent,
    ValidationWarningComponent,
    ServerErrorsComponent,
    HighlightSubstringComponent
  ],
  exports: [
    LoaderComponent,
    ValidationWarningComponent,
    ServerErrorsComponent,
    HighlightSubstringComponent
  ]
})
export class SharedModule { }
