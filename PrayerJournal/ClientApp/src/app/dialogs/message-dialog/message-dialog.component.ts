import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';

export interface MessageDialogOptions {
  title?: string;
  content?: string;
  buttonText?: string;
  buttonColor?: string;
}

@Component({
  selector: 'app-message-dialog',
  templateUrl: './message-dialog.component.html',
  styleUrls: ['./message-dialog.component.scss']
})
export class MessageDialogComponent implements OnInit {
  value = "";

  title = "MESSAGE";
  content = "Read this";
  buttonText = "OKAY";
  buttonColor = "primary";

  constructor(@Inject(MAT_DIALOG_DATA) inputData: MessageDialogOptions) {
    if (inputData)
      this.copyData(inputData);
  }

  ngOnInit() {
  }

  copyData(data: MessageDialogOptions) {
    for (let key in data)
      this[key] = data[key];
  }
}
