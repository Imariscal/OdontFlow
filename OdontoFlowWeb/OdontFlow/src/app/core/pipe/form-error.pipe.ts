import { Pipe, PipeTransform } from '@angular/core';
import { AbstractControl } from '@angular/forms';

@Pipe({
    standalone: true,
    name: 'formError'
})
export class FormErrorPipe implements PipeTransform {
    transform(control: AbstractControl | null | undefined, forceShow: boolean = false): string | null {
        if (!control) return null;
        const showError = forceShow || control.touched || control.dirty;

        if (!showError) return null;

        if (control.errors?.['required']) return 'Este campo es obligatorio.';
        if (control.errors?.['minlength']) {
            const { requiredLength, actualLength } = control.errors['minlength'];
            return `Mínimo ${requiredLength} caracteres (actual: ${actualLength}).`;
        }
        return 'Campo inválido.';
    }
}
