import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, inject } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { PrimengModule } from '../../../../shared/primeng.module';
import { MessageService } from 'primeng/api';
import { firstValueFrom } from 'rxjs';
import { CreateEmployeeDto, EmployeeViewModel, UpdateEmployeeDto } from '../../../../core/model/employee-view.model';
import { EmployeeService } from '../../../../core/services/employee.service';
import { UserService } from '../../../../core/services/user.service';
import { CreateUserDTO, UpdateUserDTO, UserViewModel } from '../../../../core/model/user-view.model';
import { Password } from 'primeng/password';
import { ClientViewModel } from '../../../../core/model/client-vew.model';
import { ClientService } from '../../../../core/services/client.service';

@Component({
  selector: 'app-user-modal',
  standalone: true,
  templateUrl: './user-modal.component.html',
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    PrimengModule
  ]
})
export class UserModalComponent implements OnChanges, OnInit {
  @Input() entity!: UserViewModel;
  @Input() showDialog = false;
  @Output() onSave = new EventEmitter<UserViewModel>();
  @Output() onClose = new EventEmitter<boolean>();

  employees: EmployeeViewModel[] = [];
  clients: ClientViewModel[] = [];
  filteredClients: ClientViewModel[] = [];

  private fb = inject(FormBuilder);
  private service = inject(UserService);
  private employeeService = inject(EmployeeService);
  private clientService = inject(ClientService);
  private messageService = inject(MessageService);

  roles = [{
    id: '1', name: 'ADMIN'
  }, {
    id: '2', name: 'LABORATORIO'
  }, {
    id: '3', name: 'CLIENTE',
  }]

  form: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    roleId: ['1', [Validators.required]],
    changePassword: [true],
    employeeId: [{ value: null, disabled: false }],
    clientId: [{ value: null, disabled: true }],
    active: [true]
  });
  submitted = false;


  ngOnChanges(changes: SimpleChanges): void {
    if (changes['entity']) {
      this.entity?.id ? this.populateFormForEdit() : this.resetForm();
      // ✅ Asegura que se aplique lógica tras cargar valores en edición
      this.toggleClientEmployeeFields(this.form.get('roleId')?.value);
    }
  }

  private populateFormForEdit(): void {
    this.form.patchValue(this.entity);

    const roleName = this.entity.roleName;
    const roleId = this.roles.find(r => r.name === roleName)?.id;

    if (roleId) {
      this.role?.setValue(roleId);
    }
 
    this.active?.enable();
  }

  private resetForm(): void {
    this.form.reset({
      email: '',
      roleId: '1',
      active: true,
      changePassword: true,
      employeeId: null,
      clientId: null
    });
 
    this.active?.disable();
    this.client?.disable();
  }



  async saveClick(): Promise<void> {
    this.submitted = true;
    if (this.form.invalid) return;

    const data = { ...this.form.value };
    const isUpdate = !!this.entity?.id;

    const dto: CreateUserDTO | UpdateUserDTO = isUpdate
      ? this.mapToUpdateProductDTO(data)
      : this.mapToCreateProductDTO(data);

    try {
      const res: any = await firstValueFrom(
        isUpdate ? this.service.put(dto as UpdateUserDTO) : this.service.post(dto as CreateUserDTO)
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

  private mapToCreateProductDTO(entity: any): CreateUserDTO {
    return {
      email: entity.email,
      role: +entity.roleId,
      employeeId: entity.employeeId,
      changePassword: entity.changePassword,
      clientId: entity.clientId,
      active: true
    };
  }

  private mapToUpdateProductDTO(entity: any): UpdateUserDTO {
    return {
      id: this.entity.id,
      email: this.entity.email,
      role: +entity.roleId,
      employeeId: entity.employeeId,
      clientId: entity.clientId,
      changePassword: entity.changePassword,
      active: entity.active
    };
  }

  ngOnInit() {
    this.form.get('employeeId')?.disable();
    this.employeeService.getActive().subscribe({
      next: (res: any) => {
        if (res.success && res.payload?.length > 0) {
          this.employees = res.payload;
        } else {
          if (res.errors?.length > 0) {
            this.messageService.add({
              severity: 'error',
              summary: 'Error',
              detail: res?.error?.errors?.Error?.[0] ?? 'No se pudieron cargar los empleados.',
              life: 3000
            });

          }
        }
      }
    });


    this.clientService.getActive().subscribe({
      next: (res: any) => {
        if (res.success && res.payload?.length > 0) {
          this.clients = res.payload;
        } else {
          if (res.errors?.length > 0) {
            this.messageService.add({
              severity: 'error',
              summary: 'Error',
              detail: res?.error?.errors?.Error?.[0] ?? 'No se pudieron cargar los clientes.',
              life: 3000
            });

          }
        }
      }
    });

    this.role?.valueChanges.subscribe(roleId => this.toggleClientEmployeeFields(roleId));

    // 👉 Aplica la lógica de activación/desactivación desde el inicio
    this.toggleClientEmployeeFields(this.role?.value);

    // Actualizar el email automáticamente
    this.form.get('employeeId')?.valueChanges.subscribe(employeeId => {
      const selectedEmployee = this.employees.find(e => e.id === employeeId);
      this.form.get('email')?.setValue(selectedEmployee?.email ?? '');
    });
 
  }

  private toggleClientEmployeeFields(roleId: string | null) {
    const selectedRole = this.roles.find(r => r.id === roleId)?.name;

    const employeeControl = this.form.get('employeeId');
    const clientControl = this.form.get('clientId');

    if (selectedRole === 'CLIENTE') {
      employeeControl?.setValue(null);
      employeeControl?.clearValidators();
      employeeControl?.disable();

      clientControl?.enable();
      clientControl?.setValidators([Validators.required]);
    } else {
      clientControl?.setValue(null);
      clientControl?.clearValidators();
      clientControl?.disable();

      employeeControl?.enable();
      employeeControl?.setValidators([Validators.required]);
    }

    employeeControl?.updateValueAndValidity();
    clientControl?.updateValueAndValidity();

    if (this.entity) {
      this.email?.setValue(this.entity.email);
    }
  }

  filterClients(event: any) {
    const query = event.query.toLowerCase();
    this.filteredClients = this.clients.filter(c => c.name?.toLowerCase().includes(query));
  }

  onClientSelected(event: any) { 
    this.form.get('email')?.setValue(event?.value.generalEmail ?? '');
  }


  get email() {
    return this.form.get('email');
  }

  get active() {
    return this.form.get('active');
  }

  get role() {
    return this.form.get('roleId');
  }

  get client() {
    return this.form.get('clientId');
  }

}
