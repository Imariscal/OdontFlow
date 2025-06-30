import {
  Component, Input, Output, ViewChild, SimpleChanges,
  OnInit, EventEmitter, inject, signal
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Table } from 'primeng/table';
import { ConfirmationService, MessageService } from 'primeng/api';
import { firstValueFrom } from 'rxjs';

import { PrimengModule } from '../../../../shared/primeng.module';
import { OrderViewModel } from '../../../../core/model/order.model';
import { OrderPaymentService } from '../../../../core/services/payment.service';
import { OrderPaymentViewModel } from '../../../../core/model/order-payment-view.model';

interface Column {
  field: string;
  header: string;
  customExportHeader?: string;
}

interface ExportColumn {
  title: string;
  dataKey: string;
}

@Component({
  standalone: true,
  selector: 'app-payment',
  templateUrl: './payment.component.html',
  imports: [CommonModule, FormsModule, ReactiveFormsModule, PrimengModule]
})
export class PaymentComponent implements OnInit {
  // Inputs & Outputs
  @Input() entity: OrderViewModel = {};
  @Input() totalVisual: number = 0;
  @Output() onSave = new EventEmitter<OrderPaymentViewModel[]>();

  // ViewChild
  @ViewChild('dt') dt!: Table;

  // Flags & State
  showDialog = false;
  submitted = false;
  isDeleting = false;
  isConfirming = false;
  selectedPayment: OrderPaymentViewModel | null = null;
  payments = signal<OrderPaymentViewModel[]>([]);

  // UI Config
  cols: Column[] = [];
  exportColumns: ExportColumn[] = [];

  // Services
  private service = inject(OrderPaymentService);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);
  private fb = inject(FormBuilder);

  // Form
  form: FormGroup = this.fb.group({
    id: [null],
    paymentTypeId: [null, Validators.required],
    amount: [null, [
      Validators.required,
      Validators.min(0.01),
      (control: any) => {
        const value = parseFloat(control.value ?? 0);
        const restante = parseFloat(this.getRemainingAmount().toFixed(2));
        return value - restante > 0.01 ? { exceedsRemaining: true } : null;
      }
    ]],
    reference: ['']
  });

  paymentTypes = [
    { label: 'Efectivo', value: 1 },
    { label: 'Transferencia', value: 2 },
    { label: 'Cheque', value: 3 },
    { label: 'Tarjeta de Crédito', value: 4 },
    { label: 'Otro', value: 5 },
    { label: 'Depósito', value: 6 }
  ];

  // Lifecycle
  async ngOnInit() {
    this.initializeColumns();
    await this.loadEntities();
  }

  async ngOnChanges(changes: SimpleChanges) {
    if (changes['entity'] && this.entity?.id) {
      await this.loadEntities();
    }
  }

  // Column setup
  private initializeColumns() {
    this.cols = [
      { field: 'name', header: 'Name' },
      { field: 'price', header: 'Price' },
      { field: 'category', header: 'Category' }
    ];
    this.exportColumns = this.cols.map(col => ({
      title: col.header,
      dataKey: col.field
    }));
  }

  // Load payments
  private async loadEntities(): Promise<OrderPaymentViewModel[]> {
    this.payments.set([]);
    try {
      const res: any = await firstValueFrom(this.service.get(this.entity.id));
      if (res.success && res.payload) {
        this.payments.set(res.payload);
        return res.payload;
      } else {
        this.showError(res.error?.errors?.Error[0] ?? 'No se pudieron cargar los pagos realizados.');
        return [];
      }
    } catch (err: any) {
      this.showError(err.message);
      return [];
    }
  }

  // Get remaining amount
  getRemainingAmount(): number {
    const pagos = this.payments || [];
    const idEditando = this.form?.get('id')?.value;
    const pagosValidos = idEditando ? pagos().filter(p => p.id !== idEditando) : pagos();
    const totalPagado = pagosValidos.reduce((acc, p) => acc + p.amount, 0);
    const raw = (this.totalVisual || 0) - totalPagado;
    return Math.round((raw + Number.EPSILON) * 100) / 100;
  }

  // Create or Update payment
  async submitForm() {
    if (this.form.invalid || !this.entity?.id) return;

    const nuevoPago = parseFloat(this.form.get('amount')?.value || 0);
    const restante = parseFloat(this.getRemainingAmount().toFixed(2));

    if (nuevoPago - restante > 0.01) {
      this.showError(`El pago excede el saldo restante de $${restante.toLocaleString('es-MX', { minimumFractionDigits: 2 })}.`);
      return;
    }

    const dto = {
      orderId: this.entity.id,
      ...this.form.value
    };

    const request$ = dto.id ? this.service.put(dto) : this.service.post(dto);

    request$.subscribe({
      next: async (res: any) => {
        if (res.success) {
          this.showSuccess(dto.id ? 'Pago actualizado.' : 'Pago registrado exitosamente.');
          this.form.reset();
          this.showDialog = false;
          const pagos = await this.loadEntities();
          this.onSave.emit(pagos);
        } else {
          this.showError(res.error?.errors?.Error[0] ?? 'No se pudo guardar el pago.');
        }
      },
      error: err => this.showError(err.message)
    });
  }

  // Delete payment
  async deleteEntity(entity: OrderPaymentViewModel) {
    if (this.isConfirming || this.isDeleting) return;
    this.isConfirming = true;

    this.confirmationService.confirm({
      message: '¿Deseas eliminar el pago seleccionado?',
      header: 'Confirmación',
      icon: 'pi pi-exclamation-triangle',
      accept: async () => {
        this.isDeleting = true;
        try {
          const res: any = await firstValueFrom(this.service.delete(entity.id!));
          if (!res.success) {
            this.showError(res.error?.errors?.Error[0] ?? 'No se pudo eliminar el pago seleccionado.');
            return;
          }
          const pagos = await this.loadEntities();
          this.onSave.emit(pagos);
          this.showSuccess('Pago eliminado correctamente.');
        } catch (err: any) {
          this.showError(err.message);
        } finally {
          this.isDeleting = false;
          this.isConfirming = false;
        }
      },
      reject: () => {
        this.isConfirming = false;
      }
    });
  }

  // UI Methods
  openNew() {
    this.form.reset();
    this.submitted = false;
    this.showDialog = true;
  }

  editEntity(entity: OrderPaymentViewModel) {
    this.form.patchValue({
      id: entity.id,
      paymentTypeId: entity.paymentTypeId,
      amount: entity.amount,
      reference: entity.reference
    });
    this.showDialog = true;
  }

  exportCSV() {
    this.dt.exportCSV();
  }

  clear(table: Table) {
    table.clear();
  }

  onGlobalFilter(table: Table, event: Event) {
    table.filterGlobal((event.target as HTMLInputElement).value, 'contains');
  }

  private showSuccess(detail: string) {
    this.messageService.add({ severity: 'success', summary: 'Éxito', detail, life: 3000 });
  }

  private showError(detail: string) {
    this.messageService.add({ severity: 'error', summary: 'Error', detail, life: 3000 });
  }
}
