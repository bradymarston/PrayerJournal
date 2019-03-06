import { Component, Input } from '@angular/core';
import { AbstractControl } from '@angular/forms';

@Component({
    selector: 'validation-warning',
    templateUrl: './validation-warning.component.html'
})
export class ValidationWarningComponent {
    @Input() control: AbstractControl;
    @Input() controlTitle: string;
    @Input() patternText: string = "";
}
