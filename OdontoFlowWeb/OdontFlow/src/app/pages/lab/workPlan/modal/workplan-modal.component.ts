import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges, inject } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { firstValueFrom } from 'rxjs';
import { PrimengModule } from '../../../../shared/primeng.module';
import { CreateWorkStationDto, UpdateWorkStationDto, WorkStationViewModel } from '../../../../core/model/workStation.model';
import { WorkStationService } from '../../../../core/services/workStation.service';
import { CreateWorkPlanDto, UpdateWorkPlanDto, WorkPlanViewModel } from '../../../../core/model/workplan-view.mode';
import { ProductService } from '../../../../core/services/product.service';
import { ProductViewModel } from '../../../../core/model/product-view.model';
import { WorkPlanService } from '../../../../core/services/workplan.service';

@Component({
  selector: 'app-workplan-modal',
  standalone: true,
  templateUrl: './workplan-modal.component.html',
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    PrimengModule
  ]
})
export class WorkPlanModelComponent implements OnChanges {
  @Input() entity!: WorkPlanViewModel;
  @Input() showDialog = false;
  @Output() onSave = new EventEmitter<WorkPlanViewModel>();
  @Output() onClose = new EventEmitter<boolean>();
  @Input() workStations: WorkStationViewModel[] = [];
  workStationsSelected: WorkStationViewModel[] = [];
  @Input() products: ProductViewModel[] = [];
  selectedProducts: ProductViewModel[] = [];

  private fb = inject(FormBuilder);
  private entityService = inject(WorkPlanService);
  private messageService = inject(MessageService);

  form: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(5)]],
    active: [true]
  });


  ngOnChanges(changes: SimpleChanges): void {
    if (changes['entity']) {
      this.entity?.id ? this.populateFormForEdit() : this.resetForm();
    }
  }

  private populateFormForEdit(): void {
    this.selectedProducts = [];
    this.workStationsSelected = [];
    this.form.patchValue({
      name: this.entity.name,
      active: this.entity.active
    });

    if(this.entity.products) {
      this.selectedProducts = this.entity.products.map(p => ({
        id: p.productId,
        name: p.productName
      }));
    }

    if (this.entity.stations) {
      this.workStationsSelected = this.entity.stations.sort(o=> o.order).map(p => ({
        id: p.workStationId,
        name: p.workStationName
      }));
    } 
    this.name?.disable();
    this.active?.enable();
  }

  private resetForm(): void {
    this.form.reset({
      name: '',
      active: true
    });

    this.name?.enable();
    this.active?.disable();
  }

  private parsePrice(formatted?: string): number | null {
    return formatted ? parseFloat(formatted.replace(/[^0-9.]/g, '')) : null;
  }

  async saveClick(): Promise<void> {
    if (this.form.invalid) return;
  
    const data = { ...this.form.value };
    const isUpdate = !!this.entity?.id;
  
    const dto: CreateWorkPlanDto | UpdateWorkPlanDto = isUpdate
      ? this.mapToUpdateDTO(data)
      : this.mapToCreateDTO(data);
  
    try {
      const res: any = await firstValueFrom(
        isUpdate
          ? this.entityService.put(dto as UpdateWorkPlanDto)
          : this.entityService.post(dto as CreateWorkPlanDto)
      );
  
      this.handleResponse(res, data);
    } catch (err: any) {
      this.messageService.add({
        severity: 'error',
        summary: 'Error',
        detail: err.error?.errors?.Error?.[0] ?? 'Error desconocido',
        life: 3000
      });
    }
  }

  private handleResponse(res: any, data: WorkStationViewModel): void {
    if (res.success) {
      this.messageService.add({
        severity: 'success',
        summary: 'Éxito',
        detail: 'Plan de trabajo guardado correctamente',
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

  private mapToCreateDTO(entity: any): CreateWorkPlanDto {
    return {
      name: entity.name,
      stations: this.workStationsSelected.map((station, index) => ({
        workStationId: station.id!,
        order: index + 1
      })),
      productIds: this.selectedProducts.map(p => p.id!),
      active: true
    };
  }
  private mapToUpdateDTO(entity: any): UpdateWorkPlanDto {
    return {
      id: this.entity.id,
      name: entity.name,
      stations: this.workStationsSelected.map((station, index) => ({
        workStationId: station.id!,
        order: index + 1
      })),
      productIds: this.selectedProducts.map(p => p.id!),
      active: entity.active
    };
  }

  get active() {
    return this.form.get('active');
  }

  get name() {
    return this.form.get('name');
  }
}
