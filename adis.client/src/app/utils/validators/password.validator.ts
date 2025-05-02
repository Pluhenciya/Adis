import { AbstractControl, ValidationErrors } from "@angular/forms";

export function passwordComplexityValidator(control: AbstractControl): ValidationErrors | null {
    const value = control.value;
    if (!value) return null;
    
    const errors: ValidationErrors = {};
    
    if (!/\d/.test(value)) errors['missingNumber'] = true;
    if (!/[A-Z]/.test(value)) errors['missingUpper'] = true;
    if (!/[`~!@#$%^&*()\-+={}[\]\\|:;"'<>,.?/]/.test(value)) errors['missingSpecial'] = true;

    return Object.keys(errors).length ? errors : null;
}