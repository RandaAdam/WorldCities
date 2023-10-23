import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormGroup } from '@angular/forms';

@Component({
  template: ``
})
export abstract class BaseFormComponent{

  form!: FormGroup;

  getErrors(
    control: AbstractControl,
    displayName: string,
    customMessage: { [key:string]:string } | null = null
  ): string[] {
    var errors: string[] = [];
    Object.keys(control.errors || {}).forEach((key) => {
      switch (key) {
        case 'required':
          errors.push(`${displayName} ${customMessage?.[key] ?? "is required."}`);
          break;
        case 'pattern':
          errors.push(`${displayName} ${customMessage?.[key] ?? "contains invalid characters."}`);
          break;
        case 'isDupeField':
          errors.push(`${displayName} ${customMessage?.[key] ?? "already exists: please choose another."}`);
          break;
        default:
          errors.push(`${displayName} is invalid.`);
          break;
      }
    });
    return errors;
  }
  constructor() { }

  
}
