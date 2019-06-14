import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';

export interface PromptDialogOptions {
  title?: string;
  type?: string;
  content?: string;
  placeholder?: string;
  submitText?: string;
  cancelText?: string;
  submitColor?: string;
  cancelColor?: string;
  regExp?: RegExp;
}

@Component({
  selector: 'app-prompt-dialog',
  templateUrl: './prompt-dialog.component.html',
  styleUrls: ['./prompt-dialog.component.scss']
})
export class PromptDialogComponent implements OnInit {
  value = "";

  title = "Prompt";
  type = "text";
  content = "Enter text";
  placeholder = "Text";
  submitText = "SUBMIT";
  cancelText = "CANCEL";
  submitColor = "primary";
  cancelColor = "basic";
  regExp = /^(?=\s*\S).*$/;

  constructor(@Inject(MAT_DIALOG_DATA) inputData: PromptDialogOptions,
    public dialogRef: MatDialogRef<PromptDialogOptions>) {
    if (inputData)
      this.copyData(inputData);
  }

  ngOnInit() {
  }

  copyData(data: PromptDialogOptions) {
    for (let key in data)
      this[key] = data[key];
  }

  submitData() {
    if (this.value)
      this.dialogRef.close(this.value);
  }
}
