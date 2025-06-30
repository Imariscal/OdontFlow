import { Component, OnInit, ViewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

// PrimeNG
import { Table } from 'primeng/table';


// App Modules
import { ReportService } from '../../../../core/services/report.service';
import { OrderViewModel } from '../../../../core/model/order.model';
import { GetOrdersByAdvancedFilterQuery } from '../../../../core/model/get-orders-by-advanced-filter.model';
import { OrderPaymentViewModel } from '../../../../core/model/order-payment-view.model';
import { ClientOrdersSummary, PatientOrdersSummary } from '../../../../core/model/patient-orders-summary.model';
import { OrderModalComponent } from '../../order/modal/order-modal.component';
import { PrimengModule } from '../../../../shared/primeng.module';

@Component({
  standalone: true,
  selector: 'app-payments-debts',
  templateUrl: './payments-debts.component.html',
  imports: [
    CommonModule,
    FormsModule,
    PrimengModule,
    OrderModalComponent
  ]
})
export class PaymentsDebtsComponent implements OnInit {
  @ViewChild('dt') dt!: Table;

  private readonly reportService = inject(ReportService);

  advancedFiltersVisible = false;
  scrollHeight = '65vh';
  showDialog = false;
  orderEntity!: OrderViewModel;

  filters: GetOrdersByAdvancedFilterQuery = {
    page: 1,
    pageSize: 50,
    creationDateStart: (() => {
      const d = new Date();
      d.setMonth(d.getMonth() - 1);
      return d;
    })() as any,
    creationDateEnd: new Date() as any
  };

  payments: OrderPaymentViewModel[] = [];
  debtOrders: OrderViewModel[] = [];
  clientSummaries: ClientOrdersSummary[] = [];

  paymentTypeOptions = [
    { label: 'EFECTIVO', value: 1 },
    { label: 'TRANSFERENCIA', value: 2 },
    { label: 'CHEQUE', value: 3 },
    { label: 'TARJETA DE CREDITO', value: 4 },
    { label: 'DEPOSITO', value: 5 },
    { label: 'OTRO', value: 6 }
  ];

  ngOnInit(): void {
    this.loadPayments();
  }

  ngAfterViewInit(): void {
    const offset = 310;
    this.scrollHeight = `${window.innerHeight - offset}px`;
  }

  loadPayments(): void {
    this.reportService.getPaymentsAndDebtsByAdvancedFilter(this.filters).subscribe({
      next: (res: any) => {
        this.clientSummaries = res?.payload?.items ?? [];
      }
    });
  }



  search(): void {
    this.filters.page = 1;
    this.loadPayments();
  }

  clearFilters(): void {
    this.filters = {
      page: 1,
      pageSize: 50
    };
    this.loadPayments();
  }

  exportCSV(): void {
    this.dt.exportCSV();
  }

  openOrderModal(orderDetail: OrderViewModel): void {
    this.orderEntity = orderDetail;
    this.showDialog = true;
  }

  getPaymentCardClass(label: string): string {
    switch (label?.toUpperCase()) {
      case 'EFECTIVO': return 'text-green-500';
      case 'TRANSFERENCIA': return 'text-blue-500';
      case 'CHEQUE': return 'text-yellow-500';
      case 'TARJETA DE CREDITO': return 'text-purple-500';
      case 'DEPOSITO': return 'text-cyan-500';
      case 'OTRO': return 'text-gray-500';
      default: return 'text-gray-400';
    }
  }

  getDebtSeverity(days: number): 'success' | 'warn' | 'danger' {
    if (days <= 15) return 'success';
    else if (days <= 30) return 'warn';
    else return 'danger';
  }

  getAllPayments(client: ClientOrdersSummary): OrderPaymentViewModel[] {
    return client.patients.flatMap(p => p.payments ?? []);
  }

  getAllDebts(client: ClientOrdersSummary): OrderViewModel[] {
    return client.patients.flatMap(p => p.debtOrders ?? []);
  }

}
