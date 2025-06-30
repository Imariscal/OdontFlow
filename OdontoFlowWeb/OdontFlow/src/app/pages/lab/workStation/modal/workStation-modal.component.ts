import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges, inject } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms'; 
import { MessageService } from 'primeng/api';
import { firstValueFrom } from 'rxjs';
import { PrimengModule } from '../../../../shared/primeng.module';
import { CreateWorkStationDto, UpdateWorkStationDto, WorkStationViewModel } from '../../../../core/model/workStation.model';
import { WorkStationService } from '../../../../core/services/workStation.service';

@Component({
  selector: 'app-workstation-modal',
  standalone: true, 
  templateUrl: './wokStation-modal.component.html',
 imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    PrimengModule 
  ]
})
export class WorkStationModalComponent implements OnChanges {
  @Input() entity!: WorkStationViewModel;
  @Input() showDialog = false;
  @Output() onSave = new EventEmitter<WorkStationViewModel>();
  @Output() onClose = new EventEmitter<boolean>();

  private fb = inject(FormBuilder);
  private entityService = inject(WorkStationService);
  private messageService = inject(MessageService);

  form: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(5)]], 
    days: [null, [Validators.min(1)]],
    active: [true]
  });
 

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['entity']) {
      this.entity?.id ? this.populateFormForEdit() : this.resetForm();
    }
  }

  private populateFormForEdit(): void {
    this.form.patchValue({
      name: this.entity.name,
      days: this.entity.days?.toString(), 
      active: this.entity.active
    });

    this.name?.disable();
    this.active?.enable();
  }

  private resetForm(): void {
    this.form.reset({
      name: '',
      productCategory: 1,
      price: null,
      isLabelRequired: false,
      applyDiscount: false,
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

    const dto: CreateWorkStationDto | UpdateWorkStationDto = isUpdate
      ? this.mapToUpdateProductDTO(data)
      : this.mapToCreateProductDTO(data);

    try {
      const res: any = await firstValueFrom(
        isUpdate ? this.entityService.put(dto as UpdateWorkStationDto) : this.entityService.post(dto as CreateWorkStationDto)
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
        detail: 'Estacion de trabajo guardada correctamente',
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

  private mapToCreateProductDTO(entity: any): CreateWorkStationDto {
    return {
      name: entity.name, 
      days: entity.days, 
      active: true
    };
  }

  private mapToUpdateProductDTO(entity: any): UpdateWorkStationDto {
    return {
      id: this.entity.id,
      name: this.entity.name, 
      days: entity.days, 
      active: entity.active
    };
  }

  get active() {
    return this.form.get('active');
  }
  
  get name() {
    return this.form.get('name');
  }

  get days() {
    return this.form.get('days');
  }

}
