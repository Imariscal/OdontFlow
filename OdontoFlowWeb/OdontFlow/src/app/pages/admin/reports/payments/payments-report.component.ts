import { Component, OnInit, ViewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Table } from 'primeng/table';
import { PrimengModule } from '../../../../shared/primeng.module';
import { ReportService } from '../../../../core/services/report.service';
import { OrderViewModel } from '../../../../core/model/order.model';
import { GetOrdersByAdvancedFilterQuery } from '../../../../core/model/get-orders-by-advanced-filter.model';
import { OrderModalComponent } from '../../order/modal/order-modal.component';

@Component({
  standalone: true,
  templateUrl: './payments-report.component.html',
  imports: [CommonModule, FormsModule, PrimengModule, OrderModalComponent]
})
export class PaymentsReportComponent implements OnInit {
  advancedFiltersVisible = false;
  payments: any[] = [];
  selectedPayments: any[] = [];
  @ViewChild('dt') dt!: Table;
  showDialog = false;
  orderEntity!: OrderViewModel;
  expandedRows: { [key: string]: boolean } = {}; 

  filters: GetOrdersByAdvancedFilterQuery = {
  page: 1,
  pageSize: 50,
  creationDateStart: (() => {
    const d = new Date();
    d.setMonth(d.getMonth() - 1);
    return d;
  }) () as any,
  creationDateEnd: new Date() as any
  };

  
  indicators = {
    totalPayments: 0,
    totalAmount: 0
  };

  paymentTypeOptions = [
    { label: 'EFECTIVO', value: 1 },
    { label: 'TRANSFERENCIA', value: 2 },
    { label: 'CHEQUE', value: 3 },
    { label: 'TARJETA DE CREDITO', value: 4 },
    { label: 'DEPOSITO', value: 5 },
    { label: 'OTRO', value: 6 },
  ];

  groupOptions = [
    { label: 'GAMMA', value: 1 },
    { label: 'ZIRCONIA', value: 2 }
  ];

  scrollHeight: string = '65vh'; 

  ngAfterViewInit() {
    const offset = 310; // píxeles a restar por header, toolbar, etc.
    const vh = window.innerHeight;
    this.scrollHeight = `${vh - offset}px`;
  }


  paymentTypeIndicators: { label: string; count: number; amount: number }[] = [];

  private reportService = inject(ReportService);

  ngOnInit() {
    this.loadPayments();
  }

  private loadPayments() {
    this.reportService.getPaymentsByAdvancedFilter(this.filters).subscribe({
      next: (res: any) => {
        if (res.success && res.payload?.items?.length > 0) {
          this.payments = res.payload.items.flatMap((order: OrderViewModel) =>
            (order.payments || []).map(payment => ({
              barcode: order.barcode,
              paymentComplete: order.paymentComplete,
              paymentCompleteDate: order.paymentDate,
              clientName: order.clientName,
              patientName: order.patientName,
              workGroup: order.workGroup,
              paymentType: payment.paymentType,
              paymentTypeId: payment.paymentTypeId,
              reference: payment.reference,
              paymentDate: payment.creationDate,
              amount: payment.amount,
              order: order
            }))
          );
          this.payments.sort((a, b) => (a.workGroup || '').localeCompare(b.workGroup || ''));
        } else {
          this.payments = [];
        }
        this.calculateIndicators(this.payments);
      }
    });
  }

  private calculateIndicators(data: any[]): void {
    this.indicators = {
      totalPayments: data.length,
      totalAmount: data.reduce((sum, p) => sum + (p.amount ?? 0), 0)
    };
    this.paymentTypeIndicators = [];
    this.paymentTypeOptions.forEach(option => {
      const paymentsOfType = data.filter(p => p.paymentType === option.label);
      if (paymentsOfType.length > 0) {
        this.paymentTypeIndicators.push({
          label: option.label,
          count: paymentsOfType.length,
          amount: paymentsOfType.reduce((sum, p) => sum + (p.amount ?? 0), 0)
        });
      }
    });
  }

  search() {
    this.filters.page = 1;
    this.loadPayments();
  }

  clearFilters() {
    this.filters = {
      page: 1,
      pageSize: 50
    };
    this.loadPayments();
  }

  exportCSV() {
    this.dt.exportCSV();
  }

  openOrderModal(orderDetail: OrderViewModel) {
    this.orderEntity = orderDetail;
    this.showDialog = true;
  }

  getPaymentCardClass(label: string): string {
    switch (label.toUpperCase()) {
      case 'EFECTIVO':
        return 'text-green-500';
      case 'TRANSFERENCIA':
        return 'text-blue-500';
      case 'CHEQUE':
        return 'text-yellow-500';
      case 'TARJETA DE CREDITO':
        return 'text-purple-500';
      case 'DEPOSITO':
        return 'text-cyan-500';
      case 'OTRO':
        return 'text-gray-500';
      default:
        return 'text-gray-400';
    }
  }

  // ✅ Para hacer el rowspan REAL:
  getRowspan(index: number): number {
    const current = this.payments[index];
    const group = current?.workGroup;
    if (!group) return 1;
    if (index > 0 && this.payments[index - 1]?.workGroup === group) {
      return 0;
    }
    return this.payments.filter(p => p.workGroup === group).length;
  }
}
