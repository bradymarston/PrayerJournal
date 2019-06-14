import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material';

export interface ConfirmDialogOptions {
  title?: string;
  content?: string;
  yesText?: string;
  noText?: string;
  yesColor?: string;
  noColor?: string;
}

@Component({
  selector: 'app-confirm-dialog',
  templateUrl: './confirm-dialog.component.html',
  styleUrls: ['./confirm-dialog.component.scss']
})
export class ConfirmDialogComponent implements OnInit {
  title = "Confirmation";
  content = "Which one?";
  yesText = "YES";
  noText = "NO";
  yesColor = "primary";
  noColor = "basic";

  constructor(@Inject(MAT_DIALOG_DATA) inputData: ConfirmDialogOptions) {
    if (inputData)
      this.copyData(inputData);
  }

  ngOnInit() {
  }

  copyData(data: ConfirmDialogOptions) {
    for (let key in data)
      this[key] = data[key];
  }
}
