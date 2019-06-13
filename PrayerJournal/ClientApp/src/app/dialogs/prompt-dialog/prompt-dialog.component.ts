import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';

export class PromptDialogOptions {
  title = "Prompt";
  type = "text";
  content = "Enter text";
  placeholder = "Text";
  submitText = "SUBMIT";
  cancelText = "CANCEL";
  submitColor = "primary";
  cancelColor = "basic";
}

@Component({
  selector: 'app-prompt-dialog',
  templateUrl: './prompt-dialog.component.html',
  styleUrls: ['./prompt-dialog.component.scss']
})
export class PromptDialogComponent implements OnInit {
  value = "";

  constructor(@Inject(MAT_DIALOG_DATA) public data: PromptDialogOptions,
    public dialogRef: MatDialogRef<PromptDialogOptions>) {
    if (!this.data) this.data = new PromptDialogOptions();
  }

  ngOnInit() {
  }

  submitData() {
    if (this.value)
      this.dialogRef.close(this.value);
  }
}
