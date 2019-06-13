import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material';
import { ConfirmDialogOptions, ConfirmDialogComponent } from '../dialogs/confirm-dialog/confirm-dialog.component';
import { Observable } from 'rxjs';
import { PromptDialogOptions, PromptDialogComponent } from '../dialogs/prompt-dialog/prompt-dialog.component';
import { MessageDialogOptions, MessageDialogComponent } from '../dialogs/message-dialog/message-dialog.component';

@Injectable()
export class DialogService {
  constructor(private _dialog: MatDialog) { }

  confirm(options: ConfirmDialogOptions): Observable<boolean> {
    return this._dialog.open(ConfirmDialogComponent, {
      autoFocus: false,
      restoreFocus: false,
      data: options
    }).afterClosed();
  }

  prompt(options: PromptDialogOptions): Observable<string> {
    return this._dialog.open(PromptDialogComponent, {
      autoFocus: true,
      restoreFocus: false,
      data: options
    }).afterClosed();
  }

  message(options: MessageDialogOptions): Observable<boolean> {
    return this._dialog.open(MessageDialogComponent, {
      autoFocus: false,
      restoreFocus: false,
      data: options
    }).afterClosed();
  }
}
