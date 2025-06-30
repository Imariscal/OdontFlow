import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges, inject } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms'; 
import { MessageService } from 'primeng/api';
import { firstValueFrom } from 'rxjs';
import { PrimengModule } from '../../../../shared/primeng.module';
import { CreateWorkStationDto, UpdateWorkStationDto, WorkStationViewModel } from '../../../../core/model/workStation.model';
import { WorkStationService } from '../../../../core/services/workStation.service';
import { CreateProductDTO, ProductViewModel, UpdateProductDTO } from '../../../../core/model/product-view.model';
import { ProductService } from '../../../../core/services/product.service';

@Component({
  selector: 'app-product-modal',
  standalone: true, 
  templateUrl: './product-modal.component.html',
 imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    PrimengModule 
  ]
})
export class ProductModalComponent implements OnChanges {
  @Input() entity!: ProductViewModel;
  @Input() showDialog = false;
  @Output() onSave = new EventEmitter<WorkStationViewModel>();
  @Output() onClose = new EventEmitter<boolean>();

  private fb = inject(FormBuilder);
  private entityService = inject(ProductService);
  private messageService = inject(MessageService);

  form: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(5)]], 
    price: [null, [Validators.min(1)]],
    isLabelRequired:  [false],
    applyDiscount : [false],
    categoryId :['1'],
    active: [true]
  });
 
  categories = [
    { label: 'GAMMA', value: '1' },
    { label: 'ZIRCONIA', value: '2' }
  ];

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['entity']) {
      this.entity?.id ? this.populateFormForEdit() : this.resetForm();
    }
  }

  private populateFormForEdit(): void {
    this.form.patchValue({
      name: this.entity.name,
      price: this.entity.price, 
      active: this.entity.active,
      categoryId: this.entity.productCategoryId,
      isLabelRequired: this.entity.isLabelRequired,
      applyDiscount: this.entity.applyDiscount,
    });

    this.name?.disable();
    this.active?.enable();
  }

  private resetForm(): void {
    this.form.reset({
      name: '',
      categoryId: 1,
      price: null,
      isLabelRequired: false,
      applyDiscount: false,
      active: true
    });

    this.name?.enable();
    this.active?.disable();
  }


  async saveClick(): Promise<void> {
 
    if (this.form.invalid) return;

    const data = { ...this.form.value };
    const isUpdate = !!this.entity?.id;

    const dto: CreateProductDTO | UpdateProductDTO = isUpdate
      ? this.mapToUpdateDTO(data)
      : this.mapToCreateDTO(data);

    try {
      const res: any = await firstValueFrom(
        isUpdate ? this.entityService.put(dto as UpdateProductDTO) : this.entityService.post(dto as CreateProductDTO)
      );
      this.handleResponse(res, data);
    } catch (err: any) {
      this.messageService.add({
        severity: 'error',
        summary: 'Error del servidor',
        detail: err.error?.errors?.Error[0] ?? 'Error desconocido',
        life: 3000
      });
    }
  }

  private handleResponse(res: any, data: WorkStationViewModel): void {
    if (res.success) {
      this.messageService.add({
        severity: 'success',
        summary: 'Éxito',
        detail: 'Producto guardado correctamente',
        life: 3000
      });

      this.onSave.emit(data);
      this.resetForm(); 
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
    this.onClose.emit(false);
  }

  private mapToCreateDTO(entity: any): CreateProductDTO {
    return {
      name: entity.name, 
      price: entity.price, 
      applyDiscount : entity.applyDiscount,
      isLabelRequired : entity.isLabelRequired,
      productCategory : +entity.categoryId,
      active: true
    };
  }

  private mapToUpdateDTO(entity: any): UpdateProductDTO {
    return {
      id: this.entity.id,
      name: this.entity.name, 
      price: entity.price?.toString(), 
      applyDiscount : entity.applyDiscount,
      isLabelRequired : entity.isLabelRequired,
      productCategory : +entity.categoryId,
      active: entity.active
    };
  }

  get active() {
    return this.form.get('active');
  }
  
  get name() {
    return this.form.get('name');
  }

  get price() {
    return this.form.get('price');
  }

}
