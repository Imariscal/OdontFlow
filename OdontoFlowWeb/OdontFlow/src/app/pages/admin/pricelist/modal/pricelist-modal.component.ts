import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges, inject } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { PrimengModule } from '../../../../shared/primeng.module';
import { MessageService } from 'primeng/api';
import { firstValueFrom } from 'rxjs'; 
import { CreatePriceListDTO, PriceListViewModel, UpdatePriceListDTO } from '../../../../core/model/pricelist-view.model';
import { PriceListService } from '../../../../core/services/pricelist.service';

@Component({
  selector: 'app-pricelist-modal',
  standalone: true,
  templateUrl: './pricelist-modal.component.html',
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    PrimengModule
  ]
})
export class PriceListModalComponent implements OnChanges {
  @Input() entity!: PriceListViewModel;
  @Input() showDialog = false;
  @Output() onSave = new EventEmitter<PriceListViewModel>();
  @Output() onClose = new EventEmitter<boolean>();

  private fb = inject(FormBuilder);
  private productService = inject(PriceListService);
  private messageService = inject(MessageService);

  categories = [
    { label: 'GAMMA', value: '1' },
    { label: 'ZIRCONIA', value: '2' }
  ];

  form: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(10)]],
    categoryId: [1],
    discount: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
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
      categoryId: this.entity.categoryId ?? this.entity.category,
      discount: this.entity.discount,
      active: this.entity.active
    });

    this.name?.disable();
    this.active?.enable();
  }

  private resetForm(): void {
    this.form.reset({
      name: '',
      categoryId: '1',
      discount: 0,
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

    const dto: CreatePriceListDTO | UpdatePriceListDTO = isUpdate
      ? this.mapToUpdateProductDTO(data)
      : this.mapToCreateProductDTO(data);

    try {
      const res: any = await firstValueFrom(
        isUpdate ? this.productService.put(dto as UpdatePriceListDTO) : this.productService.post(dto as CreatePriceListDTO)
      );
      this.handleResponse(res, data);
    } catch (err: any) {
      this.messageService.add({
        severity: 'error',
        summary: 'Error del servidor',
        detail: err?.error?.[0] ?? 'Error inesperado',
        life: 3000
      });
    }
  }

  private handleResponse(res: any, data: PriceListViewModel): void {
    if (res.success) {
      this.messageService.add({
        severity: 'success',
        summary: 'Éxito',
        detail: 'Lista guardado correctamente',
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
        detail: res.errores?.[0] ?? 'Error desconocido',
        life: 3000
      });
    }
  }

  closeClick(): void {
    this.resetForm();
    this.submitted = false;
    this.onClose.emit(false);
  }

  private mapToCreateProductDTO(entity: any): CreatePriceListDTO {
    return {
      name: entity.name,
      category: +entity.categoryId,
      discount: entity.discount,
      active: true
    };
  }

  private mapToUpdateProductDTO(entity: any): UpdatePriceListDTO {
    return {
      id: this.entity.id,
      name: this.entity.name,
      category: +entity.categoryId,
      discount: entity.discount,
      active: true
    };
  }

  get name() {
    return this.form.get('name');
  }

  get discount() {
    return this.form.get('discount');
  }

  
  get active() {
    return this.form.get('active');
  }

}
