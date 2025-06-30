import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild, inject, signal } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { PrimengModule } from '../../../../shared/primeng.module';
import { MessageService } from 'primeng/api';
import { firstValueFrom } from 'rxjs';
import { OrderService } from '../../../../core/services/order.service';
import { OrderViewModel } from '../../../../core/model/order.model';
import { CreateOrderDto } from '../../../../core/model/create-order.dto';
import { UpdateOrderDto } from '../../../../core/model/update-order.dto';
import { ClientViewModel } from '../../../../core/model/client-vew.model';
import { ClientService } from '../../../../core/services/client.service';
import { AutoCompleteCompleteEvent } from 'primeng/autocomplete';
import { ProductViewModel } from '../../../../core/model/product-view.model';
import { ProductService } from '../../../../core/services/product.service';
import { PrimeNG } from 'primeng/config';
import { PaymentComponent } from '../payment/payment.component';
import { OrderPaymentViewModel } from '../../../../core/model/order-payment-view.model';
import { Router } from '@angular/router';
import { OrderPrintComponent } from '../../../../shared/components/order-print.component';

interface EventItem {
  status?: string;
  date?: string;
  icon?: string;
  color?: string;
  image?: string;
}

enum PType {
  SUPERIOR = 49,
  INFERIOR = 50
}
@Component({
  selector: 'app-order-modal',
  standalone: true,
  templateUrl: './order-modal.component.html',
  styleUrls: ['./order-modal.component.scss'],
  imports: [CommonModule, FormsModule, ReactiveFormsModule, PaymentComponent, PrimengModule, OrderPrintComponent]
})
export class OrderModalComponent implements OnChanges, OnInit {
  // Inputs / Outputs
  @Input() entity!: OrderViewModel;
  @Input() showDialog = false;
  @Input() products!: ProductViewModel[];
  @Input() isLabView: boolean = false;
  @Output() onSave = new EventEmitter<OrderViewModel>();
  @Output() onClose = new EventEmitter<boolean>();

  // Services
  private fb = inject(FormBuilder);
  private service = inject(OrderService);
  private messageService = inject(MessageService);
  private clientService = inject(ClientService);
  private cdr = inject(ChangeDetectorRef);
  private productService = inject(ProductService);
  private router = inject(Router)
  private config = inject(PrimeNG);
  paymentsViewModel: OrderPaymentViewModel[] = [];


  // Constants & State
  today: Date = new Date();
  orderTypes = [
    { value: '1', text: 'NORMAL', committedDate: this.addDays(new Date(), 7) },
    { value: '2', text: 'URGENTE', committedDate: this.addDays(new Date(), 3) },
    { value: '3', text: 'REPETICION', committedDate: this.addDays(new Date(), 7) }
  ];
  muestrasBooleanas = [
    { label: 'Mordida', key: 'bite' },
    { label: 'Modelos', key: 'models' },
    { label: 'Dados', key: 'casts' },
    { label: 'Cucharillas', key: 'spoons' },
    { label: 'Aditamentos', key: 'attachments' },
    { label: 'Análogos', key: 'analogs' },
    { label: 'Tornillos', key: 'screws' },
    { label: 'Articulador Metálico', key: 'metalArticulator' },
    { label: 'Articulador Desechable', key: 'disposableArticulator' },
  ];


  teethSuperiorIzquierdo = [
    { id: 18, label: '18', cx: 80, cy: 60 },
    { id: 17, label: '17', cx: 120, cy: 60 },
    { id: 16, label: '16', cx: 160, cy: 60 },
    { id: 15, label: '15', cx: 200, cy: 60 },
    { id: 14, label: '14', cx: 240, cy: 60 },
    { id: 13, label: '13', cx: 280, cy: 60 },
    { id: 12, label: '12', cx: 320, cy: 60 },
    { id: 11, label: '11', cx: 360, cy: 60 },
  ];

  teethSuperiorDerecho = [
    { id: 21, label: '21', cx: 440, cy: 60 },
    { id: 22, label: '22', cx: 480, cy: 60 },
    { id: 23, label: '23', cx: 520, cy: 60 },
    { id: 24, label: '24', cx: 560, cy: 60 },
    { id: 25, label: '25', cx: 600, cy: 60 },
    { id: 26, label: '26', cx: 640, cy: 60 },
    { id: 27, label: '27', cx: 680, cy: 60 },
    { id: 28, label: '28', cx: 720, cy: 60 },
  ];

  teethInferiorIzquierdo = [
    { id: 48, label: '48', cx: 80, cy: 150 },
    { id: 47, label: '47', cx: 120, cy: 150 },
    { id: 46, label: '46', cx: 160, cy: 150 },
    { id: 45, label: '45', cx: 200, cy: 150 },
    { id: 44, label: '44', cx: 240, cy: 150 },
    { id: 43, label: '43', cx: 280, cy: 150 },
    { id: 42, label: '42', cx: 320, cy: 150 },
    { id: 41, label: '41', cx: 360, cy: 150 },
  ];

  teethInferiorDerecho = [
    { id: 31, label: '31', cx: 440, cy: 150 },
    { id: 32, label: '32', cx: 480, cy: 150 },
    { id: 33, label: '33', cx: 520, cy: 150 },
    { id: 34, label: '34', cx: 560, cy: 150 },
    { id: 35, label: '35', cx: 600, cy: 150 },
    { id: 36, label: '36', cx: 640, cy: 150 },
    { id: 37, label: '37', cx: 680, cy: 150 },
    { id: 38, label: '38', cx: 720, cy: 150 },
  ];

  //
  PType = PType;

  // Form & Related State
  form: FormGroup = this.fb.group({
    clientId: [null, [Validators.required]],
    patientName: [null, [Validators.required]],
    requesterName: [null],
    orderTypeId: [1, [Validators.required]],
    commitmentDate: [{ value: null, disabled: true }],
    color: [null, [Validators.required]],
    applyInvoice: [false],
    others: [''],
    observations: [''],
    collectionNotes: [''],
    deliveryNotes: [''],
    bite: [false], models: [false], casts: [false], spoons: [false],
    attachments: [false], analogs: [false], screws: [false],
    metalArticulator: [false], disposableArticulator: [false],
    payment: [0],
    uncollectible: [false],
    items: this.fb.array([])
  });

  totals = { subtotal: 0, iva: 0, total: 0, payments: 0, balance: 0 };
  events = signal<EventItem[]>([]);

  // Client & Product Related
  clients!: ClientViewModel[];
  filteredClients!: ClientViewModel[];
  selectedClient!: ClientViewModel;
  showAddClient = false;

  // Items Modal
  productDialog = false;
  itemDialog = false;
  editingItem: FormGroup | null = null;
  currentItemIndex: number | null = null;
  selectedItems: FormGroup[] = [];
  selectedTeeth: number[] = [];

  emptyForm = this.fb.group({});
  submitted = false;
  noAplicaPiezasFlag = false;
  //Files:
  files = [];
  totalSize: number = 0;
  totalSizePercent: number = 0;

  //butons
  disableGridButtons = signal(false);
  //FechasEstados
  confirmDate: Date | null = null;
  deliveryDate: Date | null = null;
  processDate: Date | null = null;
  completeDate: Date | null = null;

  // Impresion
  mostrarPreview = false;
  printOrder!: OrderViewModel;
  isEditMode = false;
  private ignoreOrderTypeChange = true;

  public selectedTab: any = '0';
  isPaymentSaved = false;

  ngOnInit(): void {
    this.initFormListeners();
    this.loadClients(this.entity?.client);
    this.loadProducts();
    this.initEvents();
    this.updateTotals(this.entity.payments);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['entity']) {
      if (this.entity?.id) {
        this.populateFormForEdit(); // Aquí ya se setea commitmentDate
      } else {
        this.resetForm();
      }
    }
  }



  // --- Initialization Methods ---
  private initFormListeners() {
    this.form.get('clientId')?.valueChanges.subscribe(value => {
      if (!value) this.selectedClient = {};
    });

    this.form.get('applyInvoice')?.valueChanges.subscribe(value => {
      const applyInvoice = value === true;

      this.items.controls.forEach(item => {
        this.recalculateItemCost(item as FormGroup, applyInvoice);
      });

      this.updateTotals(this.paymentsViewModel);
    });

    this.form.get('orderTypeId')?.valueChanges.subscribe(value => {
      if (this.isEditMode) return; // <--- importante
      const orderType = this.orderTypes.find(o => o.value === value);
      if (orderType) {
        this.commitmentDate?.setValue(orderType.committedDate);
      }
    });

    this.items.valueChanges.subscribe(() => {
      const applyInvoice = this.form.get('applyInvoice')?.value === true;
      this.items.controls.forEach(item => {
        this.recalculateItemCost(item as FormGroup, applyInvoice);
      });
      this.updateTotals(this.paymentsViewModel);
    });
    this.form.get('payment')?.valueChanges.subscribe(() => this.updateTotals(this.paymentsViewModel));
  }


  private recalculateItemCost(item: FormGroup, applyInvoice: boolean): void {
    const unitCost = Number(item.get('unitCost')?.value) || 0;
    const quantity = Number(item.get('quantity')?.value) || 0;
    const unitTax = applyInvoice ? +(unitCost * 0.16).toFixed(2) : 0;

    item.get('unitTax')?.setValue(unitTax, { emitEvent: false });

    const totalCost = +(quantity * (unitCost + unitTax)).toFixed(2);
    item.get('totalCost')?.setValue(totalCost, { emitEvent: false });
  }

  private generateEvents(): EventItem[] {
    const formatDate = (date: Date | string | null | undefined) => {
      if (!date) return '';
      const parsedDate = new Date(date);
      return parsedDate.getFullYear() === 1 ? '' : parsedDate.toLocaleDateString('es-MX', { day: '2-digit', month: '2-digit', year: 'numeric' });
    };
    const currentStation = this.entity?.currentStationWork;
    return [
      {
        status: 'Creado',
        date: formatDate(this.entity?.creationDate),
        icon: 'pi pi-shopping-cart',
        color: '#9C27B0',
      },
      {
        status: 'Confirmada',
        date: this.entity?.orderStatusId ? formatDate(this.confirmDate) : '',
        icon: 'pi pi-file-check',
        color: '#8fce00',
      },
      {
        status: currentStation
          ? `<strong>${currentStation.workStationName}</strong><br><span style="font-size: 13px;">${currentStation.employeeName}</span>`
          : 'En Proceso',
        date: formatDate(currentStation?.employeeStartDate ?? this.processDate),
        icon: 'pi pi-cog',
        color: '#673AB7',
      },
      {
        status: 'Terminado',
        date: formatDate(this.completeDate),
        icon: 'pi pi-check',
        color: '#607D8B',
      },
      {
        status: 'Entregado',
        date: formatDate(this.deliveryDate),
        icon: 'pi pi-shopping-cart',
        color: '#FF9800',
      },
    ];
  }


  private initEvents() {
    this.events.set(this.generateEvents());
  }

  // --- Utility Getters ---
  get clientId() { return this.form.get('clientId'); }
  get patientName() { return this.form.get('patientName'); }
  get requesterName() { return this.form.get('requesterName'); }
  get commitmentDate() { return this.form.get('commitmentDate'); }
  get applyInvoice() { return this.form.get('applyInvoice'); }
  get color() { return this.form.get('color'); }
  get orderTypeId() { return this.form.get('orderTypeId'); }
  get items(): FormArray { return this.form.get('items') as FormArray; }
  get subtotal() { return this.items.controls.reduce((s, i) => s + (i.get('quantity')?.value || 0) * (i.get('unitCost')?.value || 0), 0); }
  get iva() { return this.subtotal * 0.16; }
  get total() { return this.subtotal + this.iva; }
  get payments() { return this.form.get('payment')?.value || 0; }
  get balance() { return this.total - this.payments; }

  addDays(date: Date, days: number): Date {
    const result = new Date(date);
    result.setDate(result.getDate() + days);
    return result;
  }


  private async populateFormForEdit(): Promise<void> {
    this.selectedClient = this.entity?.client ?? {};
    await this.loadClients(this.entity?.client);
    await this.loadProducts();

    this.items.clear();
    this.clientId?.setValue(this.entity?.client, { emitEvent: false });
    this.patientName?.setValue(this.entity?.patientName, { emitEvent: false });
    this.requesterName?.setValue(this.entity?.requesterName, { emitEvent: false });
    this.color?.setValue(this.entity?.color, { emitEvent: false });
    this.applyInvoice?.setValue(this.entity?.applyInvoice, { emitEvent: false });
    this.orderTypeId?.setValue(this.entity?.orderTypeId?.toString(), { emitEvent: false });

    // ✅ Aquí seteas la fecha directamente
    setTimeout(() => {
      const date = new Date(this.entity?.commitmentDate!);
      this.commitmentDate?.setValue(date); // 🔄 Dispara el change detection
    }, 100);

    this.entity.items?.forEach(item => {
      const itemForm = this.createItem(
        item.id,
        item.productId,
        item.quantity,
        item.unitCost,
        item.teeth ?? [],
        item.unitTax ?? 0
      );
      this.items.push(itemForm);
    });

    this.paymentsViewModel = this.entity.payments ?? [];
    this.isEditMode = true;

    if (this.entity?.orderStatusId !== 1) {
      this.clientId?.disable();
      this.disableGridButtons.set(true);
    }

    this.confirmDate = this.entity?.confirmDate ? new Date(this.entity?.confirmDate) : null;
    this.deliveryDate = this.entity?.deliveryDate ? new Date(this.entity?.deliveryDate) : null;
    this.processDate = this.entity?.processDate ? new Date(this.entity?.processDate) : null;
    this.completeDate = this.entity?.completeDate ? new Date(this.entity?.completeDate) : null;

    this.updateTotals(this.entity.payments);
    this.initEvents();

    console.log('Fecha seteada:', this.form.get('commitmentDate')?.value);

    this.cdr.detectChanges();
  }


  private resetForm(): void {
    this.form.reset({
      clientId: null,
      patientName: null,
      requesterName: null,
      orderTypeId: '1',
      commitmentDate: this.today,
      color: null,
      applyInvoice: false,
      others: '',
      observations: '',
      collectionNotes: '',
      deliveryNotes: '',
      bite: false,
      models: false,
      casts: false,
      spoons: false,
      attachments: false,
      analogs: false,
      screws: false,
      metalArticulator: false,
      disposableArticulator: false,
      uncollectible: false
    });
    this.items.clear();
    this.selectedClient = {};
    this.clientId?.enable();
    this.disableGridButtons.set(false);
    this.confirmDate = new Date();
    this.deliveryDate = null;
    this.processDate = null;
    this.completeDate = null;
    this.isEditMode = false;
    this.entity.payments = undefined;
    this.selectedTab = '0';
  }

  async saveClick(): Promise<void> {

    if (this.items?.length === 0) {
      this.messageService.add({
        severity: 'error',
        summary: 'Falta de productos',
        detail: 'Agregue al menos un producto',
        life: 3000
      });

      return;
    }
    this.submitted = true;
    if (this.form.invalid) return;
    const data = { ...this.form.value };
    const isUpdate = !!this.entity?.id;
    const dto: CreateOrderDto | UpdateOrderDto = isUpdate
      ? this.mapToUpdateDTO(data)
      : this.mapToCreateDTO(data);

    try {
      const res: any = await firstValueFrom(
        isUpdate ? this.service.put(dto as UpdateOrderDto) : this.service.post(dto as CreateOrderDto)
      );
      this.handleResponse(res, data, isUpdate);
    } catch (err: any) {
      this.messageService.add({
        severity: 'error',
        summary: 'Error del servidor',
        detail: err?.error?.errors?.Error?.[0] ?? 'Error inesperado',
        life: 3000
      });
    }
  }

  private handleResponse(res: any, data: any, isUpdate: boolean): void {
    if (res.success) {
      this.messageService.add({
        severity: 'success',
        summary: 'Éxito',
        detail: 'Orden guardada correctamente',
        life: 3000
      });

      if (!isUpdate) {
        this.service.printOrderPdf(res.payload.id);
      }
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

  private mapToCreateDTO(entity: any): CreateOrderDto {
    return {
      ...entity,
      clientId: this.selectedClient.id,
      orderTypeId: +entity.orderTypeId,
      observations: entity.observations,
      collectionNotes: entity.collectionNotes,
      deliveryNotes: entity.deliveryNotes,
      items: this.items.getRawValue()
    };
  }

  private mapToUpdateDTO(entity: any): UpdateOrderDto {
    return {
      id: this.entity.id,
      ...entity,
      clientId: this.selectedClient.id,
      orderTypeId: +entity.orderTypeId,
      observations: entity.observations,
      collectionNotes: entity.collectionNotes,
      deliveryNotes: entity.deliveryNotes,
      items: this.items.getRawValue()
    };
  }

  async loadProducts(): Promise<void> {
    if (!this.selectedClient?.id) return;
    try {
      const res: any = await firstValueFrom(this.productService.getByPlanId(this.selectedClient.priceListId));
      this.products = res.success && res.payload?.length > 0 ? res.payload : [];
      if (!res.success) {
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: res.error?.errors?.Error[0] ?? 'No se pudieron cargar los productos.',
          life: 3000
        });
      }
    } catch (err: any) {
      this.messageService.add({
        severity: 'error',
        summary: 'Error',
        detail: err?.message ?? 'No se pudieron cargar los productos.',
        life: 3000
      });
    }
  }

  private async loadClients(entity?: ClientViewModel): Promise<void> {
    try {
      const res: any = await firstValueFrom(this.clientService.get());
      if (res.success && res.payload?.length > 0) {
        this.clients = res.payload;
        if (entity) this.clientId?.setValue(entity);
      } else {
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: res.error?.errors?.Error[0] ?? 'No se pudieron cargar los clientes.',
          life: 3000
        });
      }
    } catch (err: any) {
      this.messageService.add({
        severity: 'error',
        summary: 'Error',
        detail: err.message ?? 'No se pudieron cargar los clientes.',
        life: 3000
      });
    }
  }

  filterClient(event: AutoCompleteCompleteEvent) {
    let query = event.query.toLowerCase();
    this.filteredClients = this.clients.filter(client => (client?.name ?? '').toLowerCase().startsWith(query));
  }

  onClientSelected(event: any) {
    const client = event;
    this.selectedClient = client.value;
    this.loadProducts();
  }

  changeOrderTypeEvent(type: any) {
    const orderType = this.orderTypes.find(o => o.value === type.value);
    if (orderType) {
      this.commitmentDate?.setValue(orderType.committedDate);
    }
  }


  // --- Item Management ---
  createItem(id: string = '', productId: string = '', quantity = 1, unitCost = 0, teeth: any[] = [], tax = 0): FormGroup {
    const item = this.fb.group({
      id: [id],
      productId: [productId, [Validators.required]],
      quantity: [quantity, [Validators.required, Validators.min(1)]],
      unitCost: [unitCost, [Validators.required, Validators.min(0)]],
      tax: [tax],
      totalCost: [quantity * (unitCost + tax)],
      teeth: [teeth]
    });

    item.get('teeth')?.valueChanges.subscribe(() => {
      this.syncTeethWithQuantity(item);
    });

    item.valueChanges.subscribe(() => {
      const qty = item.get('quantity')?.value || 0;
      const unitCost = item.get('unitCost')?.value || 0;
      const tax = item.get('tax')?.value || 0;
      const effectiveCost = unitCost + tax;
      item.get('totalCost')?.setValue(parseFloat((qty * effectiveCost).toFixed(2)), { emitEvent: false });
      item.get('productId')?.updateValueAndValidity({ emitEvent: false });
    });

    return item;
  }


  private syncTeethWithQuantity(item: FormGroup) {
    const teeth: any[] = item.get('teeth')?.value || [];
    const unitCost = item.get('unitCost')?.value || 0;
    const tax = item.get('tax')?.value || 0;
    const effectiveCost = unitCost + tax;

    const regionCount = teeth.filter(t => t === PType.SUPERIOR || t === PType.INFERIOR).length;
    const individualTeethCount = teeth.filter(t => t !== PType.SUPERIOR && t !== PType.INFERIOR).length;
    const quantity = regionCount + individualTeethCount;

    const qty = quantity > 0 ? quantity : 1;
    item.get('quantity')?.setValue(qty, { emitEvent: false });

    const total = parseFloat((qty * effectiveCost).toFixed(2));
    item.get('totalCost')?.setValue(total, { emitEvent: false });
  }


  addNewItem() {
    this.editingItem = this.createItem();
    this.currentItemIndex = null;
    this.itemDialog = true;
  }

  editItem(index: number) {
    const item = this.items.at(index) as FormGroup;
    this.currentItemIndex = index;
    if (!this.editingItem) this.editingItem = this.createItem();
    this.editingItem.patchValue(item.getRawValue());
    this.selectedTeeth = this.editingItem.get('teeth')?.value ?? [];
    this.itemDialog = true;
  }

  saveItem() {
    if (!this.editingItem) return;

    const newValue = this.editingItem.getRawValue();
    newValue.teeth = this.selectedTeeth;
    this.editingItem.get('teeth')?.setValue(this.selectedTeeth);

    if (this.selectedTeeth.length === 0 && !this.noAplicaPiezasFlag) {
      this.messageService.add({
        severity: 'error',
        summary: 'Error',
        detail: 'Seleccione al menos una pieza',
        life: 3000
      });
      return;
    }

    if (this.currentItemIndex !== null) {
      this.items.at(this.currentItemIndex).patchValue(newValue);
    } else {
      this.items.push(this.createItem(
        '',
        newValue.productId,
        newValue.quantity,
        newValue.unitCost,
        newValue.teeth,
        newValue.tax // ✅ Ahora también tax
      ));
    }

    this.itemDialog = false;
    this.editingItem = null;
    this.currentItemIndex = null;
  }


  async deleteItem(index: number) {
    const item = this.items.value[index];
    if (item.id && item.id !== '') {

      try {
        const res: any = await firstValueFrom(this.service.deleteItem(item.id));
        if (!res.success) {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: res.error?.errors?.Error[0] ?? 'No se pudieron cargar los productos.',
            life: 3000
          });
        }
        else {
          this.items.removeAt(index);
        }
      } catch (err: any) {
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: err.error?.errors?.Error[0] ?? 'No se pudieron cargar los productos.',
          life: 3000
        });
      }
    }
    else {
      this.items.removeAt(index);
    }

  }

  deleteSelectedItems() {
    if (!this.selectedItems?.length) return;
    this.selectedItems.forEach(item => {
      const index = this.items.controls.indexOf(item);
      if (index !== -1) this.items.removeAt(index);
    });
    this.selectedItems = [];
  }

  getProductName(productId: string): string {
    console.log('getProductName', productId);
    return this.products.find(p => p.id === productId)?.name ?? '';
  }

  onProductSelected(event: any): void {
    const productId = event.value;

    if (productId) {
      const product = this.products.find(p => p.id === productId);

      if (product) {
        const unitCost = Number(product.price) || 0;
        const applyInvoice = this.form.get('applyInvoice')?.value === true;
        const tax = applyInvoice ? parseFloat((unitCost * 0.16).toFixed(0)) : 0;

        this.editingItem?.get('unitCost')?.setValue(unitCost);
        this.editingItem?.get('tax')?.setValue(tax);

        const quantity = Number(this.editingItem?.get('quantity')?.value) || 0;
        const total = parseFloat((quantity * (unitCost + tax)).toFixed(0));
        this.editingItem?.get('totalCost')?.setValue(total, { emitEvent: false });

        // 🔍 Validación manual de duplicado
        const selectedProductId = productId;
        const formArray = this.items;
        const currentGroup = this.editingItem;

        const isDuplicate = formArray.controls.some(ctrl =>
          ctrl !== currentGroup && ctrl.get('productId')?.value === selectedProductId
        );

        if (isDuplicate) {
          currentGroup?.get('productId')?.setErrors({ duplicateProduct: true });
        } else {
          currentGroup?.get('productId')?.setErrors(null);
        }
      }
    }
  }


  // --- Teeth Selection ---
  toggleTooth(toothId: number): void {
    if (!this.editingItem) return;

    const teethControl = this.editingItem.get('teeth');
    const unitCost = this.editingItem.get('unitCost')?.value || 0;
    if (!teethControl) return;

    let currentTeeth: number[] = teethControl.value || [];

    // 🔥 Si está seleccionado SUPERIOR o INFERIOR NO permitimos seleccionar piezas
    if (currentTeeth.includes(PType.SUPERIOR) || currentTeeth.includes(PType.INFERIOR)) {
      return; // No permitir tocar piezas si hay región activa
    }

    // ✅ Comportamiento normal para piezas individuales
    const index = currentTeeth.indexOf(toothId);
    if (index >= 0) {
      currentTeeth.splice(index, 1);
    } else {
      currentTeeth.push(toothId);
    }

    teethControl.setValue([...currentTeeth]);
    this.selectedTeeth = [...currentTeeth];
    this.updateToothQuantities(currentTeeth, unitCost);
  }


  selectRegion(region: PType): void {
    if (!this.editingItem) return;

    const teethControl = this.editingItem.get('teeth');
    const unitCost = this.editingItem.get('unitCost')?.value || 0;
    if (!teethControl) return;

    let currentTeeth: number[] = teethControl.value || [];

    // 🔥 Eliminar todas las piezas individuales (11–48) si se selecciona región
    currentTeeth = currentTeeth.filter(t => t >= PType.SUPERIOR); // solo quedan regiones

    if (currentTeeth.includes(region)) {
      // Si ya estaba seleccionada, deseleccionarla
      currentTeeth = currentTeeth.filter(t => t !== region);
    } else {
      currentTeeth.push(region);
    }

    teethControl.setValue([...currentTeeth]);
    this.selectedTeeth = [...currentTeeth];
    this.updateToothQuantities(currentTeeth, unitCost);
  }



  private updateToothQuantities(teeth: number[], unitCost: number): void {
    if (!this.editingItem) return;

    const regionCount = teeth.filter(t => t === PType.SUPERIOR || t === PType.INFERIOR).length;
    const individualTeethCount = teeth.filter(t => t !== PType.SUPERIOR && t !== PType.INFERIOR).length;

    const quantity = regionCount + individualTeethCount;

    this.editingItem.get('quantity')?.setValue(quantity > 0 ? quantity : 1, { emitEvent: false });
    const total = parseFloat((quantity * unitCost).toFixed(2));
    this.editingItem.get('totalCost')?.setValue(total, { emitEvent: false });
  }


  // --- Validation ---
  private duplicateProductValidator(currentGroup: FormGroup) {
    return (control: AbstractControl): ValidationErrors | null => {
      const formArray = this.items; // tu FormArray
      if (!formArray || !control.value) return null;

      const currentProductId = control.value;

      const duplicates = formArray.controls.filter(group =>
        group !== currentGroup && group.get('productId')?.value === currentProductId
      );

      return duplicates.length > 0 ? { duplicateProduct: true } : null;
    };
  }


  // --- Final Actions ---
  closeClick() {
    this.resetForm();
    this.submitted = false;
    this.onClose.emit(false);
    if (this.isPaymentSaved) {
      this.isPaymentSaved = false;
      this.onSave.emit(this.entity)
    }
  }

  
  updateTotals(payments: OrderPaymentViewModel[] = []): void {
    const items = this.items.controls;
    const applyInvoice = this.form.get('applyInvoice')?.value === true;

    let subtotal = 0;
    let iva = 0;

    // Detectar si todos los items tienen unitTax = 0
    const allUnitTaxZero = items.every(item => {
      const tax = Number(item.get('unitTax')?.value) || 0;
      return tax === 0;
    });

    items.forEach(item => {
      const quantity = Number(item.get('quantity')?.value) || 1;
      const unitCost = Number(item.get('unitCost')?.value) || 0;
      let unitTax = Number(item.get('unitTax')?.value) || 0;

      if (applyInvoice && allUnitTaxZero) {
        unitTax = +(unitCost * 0.16).toFixed(2);
        item.get('unitTax')?.setValue(unitTax, { emitEvent: false });
        item.get('totalCost')?.setValue(+((unitCost + unitTax) * quantity).toFixed(2), { emitEvent: false });
      }

      subtotal += unitCost * quantity;
      iva += unitTax * quantity;
    });

    const total = subtotal + iva;
    const totalPayments = payments.reduce((sum, p) => sum + (p.amount || 0), 0);
    const balance = total - totalPayments;

    this.totals = {
      subtotal,
      iva,
      total,
      payments: totalPayments,
      balance
    };
  }

    onSavePayments(payments: OrderPaymentViewModel[]) {
    debugger;
    this.paymentsViewModel = payments;
    this.updateTotals(this.paymentsViewModel);
    this.isPaymentSaved = true;
  }
}

