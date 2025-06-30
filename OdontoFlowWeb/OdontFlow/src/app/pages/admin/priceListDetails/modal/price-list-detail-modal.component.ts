import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges, inject } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { PrimengModule } from '../../../../shared/primeng.module';
import { ProductViewModel } from '../../../../core/model/product-view.model';
import { ProductService } from '../../../../core/services/product.service';
import { MessageService } from 'primeng/api';
import { firstValueFrom } from 'rxjs';
import { CreatePriceListItemDTO, PriceListItemViewModel, UpdatePriceListItemDTO } from '../../../../core/model/pricelistItem-view.model';
import { PriceListItemService } from '../../../../core/services/pricelistItem.service';
import { PriceListViewModel } from '../../../../core/model/pricelist-view.model';

@Component({
  selector: 'app-price-list-detail-modal',
  standalone: true,
  templateUrl: './price-list-detail-modal.component.html',
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    PrimengModule
  ]
})
export class ProductListDetailModalComponent implements OnChanges {
  @Input() entity!: PriceListItemViewModel;
  @Input() plan!: PriceListViewModel;
  @Input() showDialog = false;
  @Input() products! : ProductViewModel[];
  @Output() onSave = new EventEmitter<ProductViewModel>();
  @Output() onClose = new EventEmitter<boolean>();

  private fb = inject(FormBuilder); 
  private messageService = inject(MessageService);
  private productItemService = inject(PriceListItemService);

  selectedProduct!: ProductViewModel; 

  onProductSelected(product: ProductViewModel) {
    this.selectedProduct = product;
    this.productPrice?.setValue(product?.price ?? null);
  }

  form: FormGroup = this.fb.group({
    productId: ['', [Validators.required]],
    productPrice: [null],
    price: [null, [Validators.required]],
    comments:  [null]
  });

  submitted = false;

  get productId() {
    return this.form.get('productId');
  }

  get price() {
    return this.form.get('price');
  }

  get productPrice() {
    return this.form.get('productPrice');
  }

  ngOnInit() {
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['entity']) {   
      if (this.entity?.id) {
        this.populateFormForEdit();
      } else {
        this.resetForm();
      }
    }
  }
  
  private populateFormForEdit(): void {
    this.form.patchValue({
      productId: this.entity.product,
      price: this.entity.price,
      productPrice : this.entity?.product?.price,
      comments: this.entity.comments
    }); 
 
    this.productId?.disable();
    this.productPrice?.disable();
  }


  private resetForm(): void {
    this.form.reset({
      productCategory: null,
      price: null,
      productPrice : null
    });

    this.productPrice?.disable();
    this.productId?.enable();
  }

  async saveClick(): Promise<void> {
    this.submitted = true;
    if (this.form.invalid) return;

    const data = { ...this.form.value };
    const isUpdate = !!this.entity?.id;

    let dto: any= isUpdate
      ? this.mapToUpdateProductDTO(data)
      : this.mapToCreateProductDTO(data);

      try {
      const res: any = await firstValueFrom(
        isUpdate ? this.productItemService.put(dto as UpdatePriceListItemDTO) : this.productItemService.post(dto as CreatePriceListItemDTO)
      );
      this.handleResponse(res, data, isUpdate);
    } catch (err: any) {
      this.messageService.add({
        severity: 'error',
        summary: 'Error del servidor',
        detail: err.error?.errors?.Error[0] ?? 'Error desconocido',
        life: 3000
      });
    }
  }

  private handleResponse(res: any, data: PriceListItemViewModel, isUpdate :boolean = false): void {
    if (res.success) {
      this.messageService.add({
        severity: 'success',
        summary: 'Éxito',
        detail: 'Producto guardado a la lista correctamente',
        life: 3000
      });

      this.onSave.emit(data);
      this.resetForm();
      this.submitted = false;

      if (isUpdate)this.onClose.emit(false);
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

  private mapToCreateProductDTO(product: any): CreatePriceListItemDTO {
    return {
      productId: product.productId.id,
      price: product.price,
      priceListId: this.plan.id ?? '',
      comments: product.comments
    };
  }

  private mapToUpdateProductDTO(product: any): UpdatePriceListItemDTO {
    return {
      id: this.entity.id,
      price: product.price,
      comments: product.comments
    };
  } 
  
}
