import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges, inject } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { PrimengModule } from '../../../../shared/primeng.module'; 
import { MessageService } from 'primeng/api';
import { firstValueFrom } from 'rxjs';
import { CreateSupplierDTO, SupplierViewModel, UpdateSupplierDTO } from '../../../../core/model/supplier-view.model';
import { SupplierService } from '../../../../core/services/supplier.service';

@Component({
  selector: 'app-supplier-modal',
  standalone: true,
  templateUrl: './supplier-modal.component.html',
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    PrimengModule 
  ]
})
export class SupplierModalComponent implements OnChanges {
  @Input() entity!: SupplierViewModel;
  @Input() showDialog = false;
  @Output() onSave = new EventEmitter<SupplierViewModel>();
  @Output() onClose = new EventEmitter<boolean>();

  private fb = inject(FormBuilder);
  private productService = inject(SupplierService);
  private messageService = inject(MessageService);


  form: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(10)]],
    contact: ['',[Validators.required,  Validators.minLength(10)]],
    email: ['', [Validators.required, Validators.email]],
    phone1: ['', Validators.required],
    phone2: [''],
    bankDetails: [''],
    invoiceContact: [''],
    invoiceEmail: [''],
    invoicePhone: [''],
    account: [''],
    credit: [null],
    comments: [''],
    product: [''],
    address: [''],
    active: [true]
  });
  
  submitted = false;
 
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['entity']) {
      this.entity?.id ? this.populateFormForEdit() : this.resetForm();
    }
  }
  private populateFormForEdit(): void {
    this.form.patchValue({
      name: this.entity.name,
      contact: this.entity.contact,
      email: this.entity.email,
      phone1: this.entity.phone1,
      phone2: this.entity.phone2,
      bankDetails: this.entity.bankDetails,
      invoiceContact: this.entity.invoiceContact,
      invoiceEmail: this.entity.invoiceEmail,
      invoicePhone: this.entity.invoicePhone,
      account: this.entity.account,
      credit: this.entity.credit,
      comments: this.entity.comments,
      product: this.entity.product,
      address: this.entity.address,
      active: this.entity.active
    });
  
    this.name?.disable();  
    this.active?.enable(); 
  }

  private resetForm(): void {
    this.form.reset({
      name: '',
      contact: '',
      email: '',
      phone1: '',
      phone2: '',
      bankDetails: '',
      invoiceContact: '',
      invoiceEmail: '',
      invoicePhone: '',
      account: '',
      credit: null,
      comments: '',
      product: '',
      address: '',
      active: true
    });
  
    this.name?.enable(); 
    this.active?.disable(); 
  }

 
  async saveClick(): Promise<void> {
    this.submitted = true;
    if (this.form.invalid) return;

    const data = { ...this.form.value };
    const isUpdate = !!this.entity?.id;

    const dto: CreateSupplierDTO | UpdateSupplierDTO = isUpdate
      ? this.mapToUpdateProductDTO(data)
      : this.mapToCreateProductDTO(data);

    try {
      const res: any = await firstValueFrom(
        isUpdate ? this.productService.put(dto as UpdateSupplierDTO) : this.productService.post(dto as CreateSupplierDTO)
      );
      this.handleResponse(res, data);
    } catch (err: any) {
      this.messageService.add({
        severity: 'error',
        summary: 'Error',
        detail: err.error?.errors?.Error[0] ?? 'Error desconocido',
        life: 3000
      });
    }
  }

  private handleResponse(res: any, data: SupplierViewModel): void {
    if (res.success) {
      this.messageService.add({
        severity: 'success',
        summary: 'Éxito',
        detail: 'Proveedor guardado correctamente',
        life: 3000
      });

      this.onSave.emit(data);
      this.onClose.emit(false);
      this.resetForm();
      this.submitted = false;
    } else {
      this.messageService.add({
        severity: 'error',
        summary: 'Error',
        detail: res.error?.errors?.Error[0] ?? 'Error desconocido',
        life: 3000
      });
    }
  }

  closeClick(): void {
    this.resetForm();
    this.submitted = false;
    this.onClose.emit(false);
  }

  private mapToCreateProductDTO(entity: any): CreateSupplierDTO {
    return {
      name: entity.name,
      contact: entity.contact,
      email: entity.email,
      phone1: entity.phone1,
      phone2: entity.phone2,
      bankDetails: entity.bankDetails,
      invoiceContact: entity.invoiceContact,
      invoiceEmail: entity.invoiceEmail,
      invoicePhone: entity.invoicePhone,
      account: entity.account,
      credit: entity.credit,
      comments: entity.comments,
      product: entity.product,
      address: entity.address,
      active: entity.active
    };
  }

  private mapToUpdateProductDTO(entity: any): UpdateSupplierDTO {
    return {
      id: this.entity.id,
      name:this.entity.name,
      contact: entity.contact,
      email: entity.email,
      phone1: entity.phone1,
      phone2: entity.phone2,
      bankDetails: entity.bankDetails,
      invoiceContact: entity.invoiceContact,
      invoiceEmail: entity.invoiceEmail,
      invoicePhone: entity.invoicePhone,
      account: entity.account,
      credit: entity.credit,
      comments: entity.comments,
      product: entity.product,
      address: entity.address,
      active: entity.active
    };
  }

  get name() {
    return this.form.get('name');
  }

  get contact() {
    return this.form.get('contact');
  }

  get email() {
    return this.form.get('email');
  }

  get phone() {
    return this.form.get('phone1');
  }

  get active() {
    return this.form.get('active');
  }
}
