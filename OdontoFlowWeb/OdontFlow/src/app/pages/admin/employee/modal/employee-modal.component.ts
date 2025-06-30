import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges, inject } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { PrimengModule } from '../../../../shared/primeng.module';
import { MessageService } from 'primeng/api';
import { firstValueFrom } from 'rxjs'; 
import { CreateEmployeeDto, EmployeeViewModel, UpdateEmployeeDto } from '../../../../core/model/employee-view.model';
import { EmployeeService } from '../../../../core/services/employee.service';
import { ClientService } from '../../../../core/services/client.service';

@Component({
  selector: 'app-employee-modal',
  standalone: true,
  templateUrl: './employee-modal.component.html',
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    PrimengModule
  ]
})
export class EmployeeModalComponent implements OnChanges {
  @Input() entity!: EmployeeViewModel;
  @Input() showDialog = false;
  @Output() onSave = new EventEmitter<EmployeeViewModel>();
  @Output() onClose = new EventEmitter<boolean>();

  private fb = inject(FormBuilder);
  private service = inject(EmployeeService);
  private clientService = inject(ClientService);
  private messageService = inject(MessageService);

  form: FormGroup = this.fb.group({
    name: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    applyCommission: [false],
    isSalesRepresentative: [false],
    commissionPercentage: [{ value: 0, disabled: true }],
    active: [true] 
  });

  submitted = false; 

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['entity']) {
      this.entity?.id ? this.populateFormForEdit() : this.resetForm();
    }
  }

  private populateFormForEdit(): void {
    this.form.patchValue(this.entity);
    this.name?.disable();
    this.active?.enable(); 
  }

  private resetForm(): void {
    this.form.reset({
      name: '',
      email: '',
      applyCommission: false,
      isSalesRepresentative: false,
      commissionPercentage: 0,
      active: true
    });
    this.name?.enable();
    this.active?.disable(); 
  }

  async saveClick(): Promise<void> {
    this.submitted = true;
    if (this.form.invalid) return;

    const data = { ...this.form.getRawValue() };
    const isUpdate = !!this.entity?.id;

    const dto: CreateEmployeeDto | UpdateEmployeeDto = isUpdate
      ? this.mapToUpdateProductDTO(data)
      : this.mapToCreateProductDTO(data);

    try {
      const res: any = await firstValueFrom(
        isUpdate ? this.service.put(dto as UpdateEmployeeDto) : this.service.post(dto as CreateEmployeeDto)
      );
      this.handleResponse(res, data);
    } catch (err: any) {
      this.messageService.add({
        severity: 'error',
        summary: 'Error del servidor',
        detail: err?.error?.errors?.Error?.[0] ?? 'Error inesperado',
        life: 3000
      });
    }
  }

  private handleResponse(res: any, data: EmployeeViewModel): void {
    if (res.success) {
      this.messageService.add({
        severity: 'success',
        summary: 'Éxito',
        detail: 'Empleado guardado correctamente',
        life: 3000
      });

      this.onSave.emit(res.payload);
      this.onClose.emit(false);
      this.resetForm();
      this.submitted = false;
    } else {
      this.messageService.add({
        severity: 'error',
        summary: 'Error',
        detail: res?.error?.errors?.Error?.[0] ?? 'Error desconocido',
        life: 3000
      });
    }
  }

  closeClick(): void {
    this.resetForm();
    this.submitted = false;
    this.onClose.emit(false);
  }

  private mapToCreateProductDTO(entity: any): CreateEmployeeDto {
    return {
      name: entity.name,
      email: entity.email,
      applyCommission: entity.applyCommission,
      isSalesRepresentative: entity.isSalesRepresentative,
      commissionPercentage: entity.commissionPercentage,
      active: true 
    };
  }
  
  private mapToUpdateProductDTO(entity: any): UpdateEmployeeDto {
    return {
      id: this.entity.id,
      name: entity.name,
      email: entity.email,
      applyCommission: entity.applyCommission,
      isSalesRepresentative: entity.isSalesRepresentative,
      commissionPercentage: entity.commissionPercentage,
      active: true 
    };
  }
  

  async ngOnInit() {
    // Escuchar cambios en el checkbox "Aplica Comisión"
    this.form.get('applyCommission')?.valueChanges.subscribe((value) => {
      const control = this.commissionPercentage;
      if (value) {
        control?.setValidators([Validators.required, Validators.min(0), Validators.max(100)]);
        control?.enable();
      } else {
        control?.clearValidators();
        control?.disable();
      }
      control?.updateValueAndValidity({ onlySelf: true });
      this.form.updateValueAndValidity();
    });
   
  }
  
  get name() {
    return this.form.get('name');
  }

  get commissionPercentage() {
    return this.form.get('commissionPercentage');
  }

  get active() {
    return this.form.get('active');
  }

  get email() {
    return this.form.get('email');
  }

  asFormGroup(control: AbstractControl): FormGroup {
    return control as FormGroup;
  }

 
}