import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges, inject } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { PrimengModule } from '../../../../shared/primeng.module';
import { MessageService } from 'primeng/api';
import { firstValueFrom } from 'rxjs';
import { CreateSupplierDTO, SupplierViewModel, UpdateSupplierDTO } from '../../../../core/model/supplier-view.model';
import { ClientViewModel, CreateClientDto, UpdateClientDto } from '../../../../core/model/client-vew.model';
import { ClientService } from '../../../../core/services/client.service';
import { PriceListItemViewModel } from '../../../../core/model/pricelistItem-view.model';
import { REGIMENES_FISCALES, USOS_CFDI } from '../../../../core/constans/const';
import { EmployeeViewModel } from '../../../../core/model/employee-view.model';
import { AutoCompleteCompleteEvent } from 'primeng/autocomplete';
import { EmployeeModalComponent } from '../../employee/modal/employee-modal.component';
import { EmployeeService } from '../../../../core/services/employee.service';


@Component({
  selector: 'app-client-modal',
  standalone: true,
  templateUrl: './client-modal.component.html',
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    PrimengModule,
    EmployeeModalComponent
  ]
})
export class ClientModalComponent implements OnChanges {
  @Input() entity!: ClientViewModel;
  @Input() showDialog = false;
  @Output() onSave = new EventEmitter<ClientViewModel>();
  @Output() onClose = new EventEmitter<boolean>();
  @Input() priceList!: PriceListItemViewModel[];
  @Input() employees!: EmployeeViewModel[];
  filteredEmployee!: EmployeeViewModel[];
  showEmployeeDialog : boolean = false;

  readonly RFC_PATTERN = /^([A-ZÑ&]{3,4})(\d{6})([A-Z\d]{2})([A\d])?$/;
  readonly ZIP_PATTERN = /^\d{5}$/;
  private fb = inject(FormBuilder);

  private employeeService = inject(EmployeeService);
  private service = inject(ClientService);
  private messageService = inject(MessageService);

  regimenesFiscales = REGIMENES_FISCALES;
  usosCFDI = USOS_CFDI;
  submitted = false;

  categories = [
    { label: 'GAMMA', value: '1' },
    { label: 'ZIRCONIA', value: '2' }
  ];

  servicio = [
    { label: 'APLICA', value: '1' },
    { label: 'NO APLICA', value: '2' }
  ];

  factura = [
    { label: 'APLICA', value: 'true' },
    { label: 'NO APLICA', value: 'false' }
  ];

  form: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3)]],
    address: ['', Validators.required],
    contact: ['', [Validators.required, Validators.minLength(3)]],
    phone1: ['', Validators.required],
    phone2: [''],
    mobile: [''],
    generalEmail: ['', [Validators.required, Validators.email]],
    collectionEmail: ['', [Validators.email]],
    credit: [null, [Validators.required, Validators.min(0), Validators.max(60000)]],
    appliesInvoice: ['false'],
    groupId: ['1'],
    stateId: ['1'],
    remarks: [''],
    priceListId: [null, Validators.required],
    employeeId: [null],
    account: [''],
    active: [true],
    commissionPercentage: [{ value: 0 },Validators.required ],
    clientInvoice: this.fb.group({
      invoiceName: [''],
      invoiceRFC: [''],
      invoiceEmail: ['', [Validators.email]],
      phone: [''],
      cfdiUse: [''],
      regimen: [''],
      street: [''],
      exteriorNumber: [''],
      interiorNumber: [''],
      city: [''],
      zipCode: [''],
      municipality: [''],
      state: [''],
      country: ['']
    })
  });

  ngOnInit() {

    this.loadEmployees();
    this.form.get('appliesInvoice')?.valueChanges.subscribe((value) => {
      const invoiceGroup = this.form.get('clientInvoice') as FormGroup;
      console.log(this.employees);
      Object.keys(invoiceGroup.controls).forEach(key => {
        const control = invoiceGroup.get(key);

        if (!control) return;

        if (value === 'true') {
          const required = Validators.required;
          switch (key) {
            case 'invoiceName':
            case 'invoiceRFC':
            case 'invoiceEmail':
            case 'cfdiUse':
            case 'regimen':
            case 'street':
            case 'exteriorNumber':
            case 'city':
            case 'zipCode':
            case 'municipality':
            case 'state':
            case 'country':
              control.setValidators(required);
              break;
            case 'zipCode':
              control.setValidators([required, Validators.pattern(this.ZIP_PATTERN)]);
              break;
            case 'invoiceRFC':
              control.setValidators([required, Validators.pattern(this.RFC_PATTERN)]);
              break;
            case 'invoiceEmail':
              control.setValidators([required, Validators.email]);
              break;
            case 'interiorNumber':
              control.clearValidators(); // sigue siendo opcional
              break;
          }
        } else {
          control.clearValidators();
        }

        control.updateValueAndValidity({ onlySelf: true });
      });

      if (value === 'true') {
        Object.keys(invoiceGroup.controls).forEach(key => {
          invoiceGroup.get(key)?.markAsTouched({ onlySelf: true });
        });
      }

      invoiceGroup.updateValueAndValidity();
      this.form.updateValueAndValidity();
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['entity']) {
      this.entity?.id ? this.populateFormForEdit() : this.resetForm();
    }
  }


  private populateFormForEdit(): void {
    this.form.patchValue({
      name: this.entity?.name,
      address: this.entity?.address,
      contact: this.entity?.contact,
      phone1: this.entity?.phone1,
      phone2: this.entity?.phone2,
      mobile: this.entity?.mobile,
      generalEmail: this.entity?.generalEmail,
      collectionEmail: this.entity?.collectionEmail,
      credit: this.entity?.credit,
      appliesInvoice: this.entity?.appliesInvoice ? "true" : "false",
      groupId: this.entity?.groupId,
      stateId: this.entity?.stateId,
      remarks: this.entity?.remarks,
      priceListId: this.entity?.priceListId,
      employeeId: this.employees.filter( e => e.id === this.entity?.employeeId)[0] ?? null,
      account: this.entity?.account,
      active: this.entity?.active,
      commissionPercentage : this.entity?.commissionPercentage,
      clientInvoice: {
        invoiceName: this.entity?.clientInvoice?.invoiceName,
        invoiceRFC: this.entity?.clientInvoice?.invoiceRFC,
        invoiceEmail: this.entity?.clientInvoice?.invoiceEmail,
        phone: this.entity?.clientInvoice?.phone,
        cfdiUse: this.entity?.clientInvoice?.cfdiUse,
        regimen: this.entity?.clientInvoice?.regimen,
        street: this.entity?.clientInvoice?.street,
        exteriorNumber: this.entity?.clientInvoice?.exteriorNumber,
        interiorNumber: this.entity?.clientInvoice?.interiorNumber,
        city: this.entity?.clientInvoice?.city,
        zipCode: this.entity?.clientInvoice?.zipCode,
        municipality: this.entity?.clientInvoice?.municipality,
        state: this.entity?.clientInvoice?.state,
        country: this.entity?.clientInvoice?.country
      }
    });

  }
  private resetForm(): void {
    this.form.reset({
      name: '',
      address: '',
      contact: '',
      phone1: '',
      phone2: '',
      mobile: '',
      generalEmail: '',
      collectionEmail: '',
      credit: null,
      appliesInvoice: 'false',
      groupId: '1',
      stateId: '1',
      remarks: '',
      priceListId: null,
      employeeId: null,
      account: '',
      active: true,
      commissionPercentage: 0,
      clientInvoice: {
        invoiceName: '',
        invoiceRFC: '',
        invoiceEmail: '',
        phone: '',
        cfdiUse: '',
        regimen: '',
        street: '',
        exteriorNumber: '',
        interiorNumber: '',
        city: '',
        zipCode: '',
        municipality: '',
        state: '',
        country: ''
      }
    });
  }



  async saveClick(): Promise<void> {
    this.submitted = true;
    console.log(this.form)
    if (this.form.invalid) return;

    const data = { ...this.form.value };
    const isUpdate = !!this.entity?.id;

    const dto: CreateSupplierDTO | UpdateSupplierDTO = isUpdate
      ? this.mapToUpdateClientDTO(data)
      : this.mapToCreateClientDTO(data);

    try {
      const res: any = await firstValueFrom(
        isUpdate ? this.service.put(dto as UpdateSupplierDTO) : this.service.post(dto as CreateSupplierDTO)
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
        detail: 'Cliente guardado correctamente',
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

  private mapToCreateClientDTO(entity: any): CreateClientDto {
    return {
      name: entity.name,
      address: entity.address,
      contact: entity.contact,
      phone1: entity.phone1,
      phone2: entity.phone2,
      mobile: entity.mobile,
      generalEmail: entity.generalEmail,
      collectionEmail: entity.collectionEmail,
      credit: entity.credit,
      appliesInvoice: entity.appliesInvoice === 'true',
      groupId: entity.groupId,
      stateId: entity.stateId,
      remarks: entity.remarks,
      priceListId: entity.priceListId,
      employeeId: entity.employeeId?.id  ?  entity.employeeId?.id  : entity.employeeId,
      account: entity.account,
      active: entity.active,
      commissionPercentage: entity.commissionPercentage,
      clientInvoice: entity.appliesInvoice === 'true' ? {
        invoiceName: entity.clientInvoice?.invoiceName,
        invoiceRFC: entity.clientInvoice?.invoiceRFC,
        invoiceEmail: entity.clientInvoice?.invoiceEmail,
        phone: entity.clientInvoice?.phone,
        cfdiUse: entity.clientInvoice?.cfdiUse,
        regimen: entity.clientInvoice?.regimen,
        street: entity.clientInvoice?.street,
        exteriorNumber: entity.clientInvoice?.exteriorNumber,
        interiorNumber: entity.clientInvoice?.interiorNumber,
        city: entity.clientInvoice?.city,
        zipCode: entity.clientInvoice?.zipCode,
        municipality: entity.clientInvoice?.municipality,
        state: entity.clientInvoice?.state,
        country: entity.clientInvoice?.country
      } : null
    };
  }


  private mapToUpdateClientDTO(entity: any): UpdateClientDto {
    return {
      id: this.entity.id,
      name: entity.name,
      address: entity.address,
      contact: entity.contact,
      phone1: entity.phone1,
      phone2: entity.phone2,
      mobile: entity.mobile,
      generalEmail: entity.generalEmail,
      collectionEmail: entity.collectionEmail,
      credit: entity.credit,
      appliesInvoice: entity.appliesInvoice === 'true',
      groupId: entity.groupId,
      stateId: entity.stateId,
      remarks: entity.remarks,
      priceListId: entity.priceListId,
      employeeId: entity.employeeId?.id  ?  entity.employeeId?.id  : entity.employeeId,
      account: entity.account,
      active: entity.active,
      commissionPercentage: entity.commissionPercentage,
      clientInvoice: entity.appliesInvoice === 'true' ? {
        id: this.entity.clientInvoice?.id,
        invoiceName: entity.clientInvoice?.invoiceName,
        invoiceRFC: entity.clientInvoice?.invoiceRFC,
        invoiceEmail: entity.clientInvoice?.invoiceEmail,
        phone: entity.clientInvoice?.phone,
        cfdiUse: entity.clientInvoice?.cfdiUse,
        regimen: entity.clientInvoice?.regimen,
        street: entity.clientInvoice?.street,
        exteriorNumber: entity.clientInvoice?.exteriorNumber,
        interiorNumber: entity.clientInvoice?.interiorNumber,
        city: entity.clientInvoice?.city,
        zipCode: entity.clientInvoice?.zipCode,
        municipality: entity.clientInvoice?.municipality,
        state: entity.clientInvoice?.state,
        country: entity.clientInvoice?.country
      } : null
    };
  }

  get invalidInvoiceCount(): number {
    const group = this.form.get('clientInvoice') as FormGroup;

    if (!this.appliesInvoice?.value || !group) return 0;

    return Object.keys(group.controls).filter(key => {
      const ctrl = group.get(key);
      return ctrl && ctrl.invalid && (ctrl.touched || ctrl.dirty);
    }).length;
  }


  get invalidContactCount(): number {
    const excludedKeys = ['clientInvoice']; // porque es un subgrupo
    return Object.keys(this.form.controls).filter(key => {
      if (excludedKeys.includes(key)) return false;
      const ctrl = this.form.get(key);
      return ctrl && ctrl.invalid && (ctrl.touched || ctrl.dirty);
    }).length;
  }

  get name() {
    return this.form.get('name');
  }

  get contact() {
    return this.form.get('contact');
  }

  get email() {
    return this.form.get('generalEmail');
  }

  get phone() {
    return this.form.get('phone1');
  }

  get active() {
    return this.form.get('active');
  }

  get address() {
    return this.form.get('address');
  }

  get credit() {
    return this.form.get('credit');
  }

  get priceListId() {
    return this.form.get('priceListId');
  }


  get employeeId() { return this.form.get('employeeId'); }
  get appliesInvoice() { return this.form.get('appliesInvoice'); }

  get invoiceName() { return this.form.get('clientInvoice.invoiceName'); }
  get invoiceRFC() { return this.form.get('clientInvoice.invoiceRFC'); }
  get invoiceEmail() { return this.form.get('clientInvoice.invoiceEmail'); }
  get cfdiUse() { return this.form.get('clientInvoice.cfdiUse'); }
  get regimen() { return this.form.get('clientInvoice.regimen'); }
  get street() { return this.form.get('clientInvoice.street'); }
  get exteriorNumber() { return this.form.get('clientInvoice.exteriorNumber'); }
  get interiorNumber() { return this.form.get('clientInvoice.interiorNumber'); }
  get city() { return this.form.get('clientInvoice.city'); }
  get municipality() { return this.form.get('clientInvoice.municipality'); }
  get zipCode() { return this.form.get('clientInvoice.zipCode'); }
  get state() { return this.form.get('clientInvoice.state'); }
  get country() { return this.form.get('clientInvoice.country'); }

  filterEmployee(event: AutoCompleteCompleteEvent) {
    let filtered: any[] = [];
    let query = event.query;

    if (this.employees && this.employees.length > 0) {
      for (let i = 0; i < (this.employees as any[]).length; i++) {
        let employee = (this.employees as any[])[i];
        if (employee.name.toLowerCase().indexOf(query.toLowerCase()) == 0) {
          filtered.push(employee);
        }
      }     
    }
    this.filteredEmployee = filtered;
  }

  onEmployeeClose() {
    this.showEmployeeDialog = false;
  }

  onEmployeeSave(entity: EmployeeViewModel) {
    this.loadEmployees(entity);
    this.showEmployeeDialog = false;
  }

  private loadEmployees(entity?: EmployeeViewModel) {
    this.employeeService.getSales().subscribe({
      next: (res : any) => {
        if (res.success && res.payload?.length > 0) {
          this.employees  = res.payload;
          if (entity) this.employeeId?.setValue(entity);
        } else {
          if (res.errors?.length > 0){
            this.messageService.add({
              severity: 'error',
              summary: 'Error',
              detail: res.error?.errors?.Error[0] ?? 'No se pudieron cargar los empleados.',
              life: 3000
            });
          
          }     
        }
      },
      error: err =>  {
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: err.message ?? 'No se pudieron cargar los empleados.',
          life: 3000
        });
      } 
    });
  }

  get commissionPercentage() {
    return this.form.get('commissionPercentage');
  }

}

