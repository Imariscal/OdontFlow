import { Routes } from '@angular/router';
import { Documentation } from './documentation/documentation'; 
import { Empty } from './empty/empty';   
import { ProductsComponents } from './admin/products/products.component';
import { SupplierComponent } from './admin/suppliers/supplier.component';
import { PriceListComponent } from './admin/pricelist/price-list.component';
import { PriceListDetailsComponent } from './admin/priceListDetails/price-list-details.component';
import { ClientComponent } from './admin/client/client.component';
import { WorkStationComponent } from './lab/workStation/workStation.component';
import { WorkPlanComponent } from './lab/workPlan/workPlan.component';
import { EmployeeComponent } from './admin/employee/employee.component';
import { OrderComponent } from './admin/order/order.component';
import { LabDashboardComponent } from './lab/dashboard/dashboard.component';
import { WorkDetailComponent } from './lab/workdetail/workDetail.component';
import { UsersComponent } from './security/users/user.component';
import { ProductivityComponent } from './lab/reports/productivity.component'; 
import { OrdersReportComponent } from './admin/reports/orders/orders-report.component';
import { PaymentsReportComponent } from './admin/reports/payments/payments-report.component';
import { DebtsReportComponent } from './admin/reports/debts/debts-report.component';
import { PiecesReportComponent } from './admin/reports/pieces/pieces-report.component'; 
import { CommissionReportComponent } from './admin/reports/commission/comission.component';
import { PaymentsDebtsComponent } from './admin/reports/payments-debts/payments-debts.component';

export default [
    { path: 'documentation', component: Documentation }, 
    { path: 'products', component: ProductsComponents }, 
    { path: 'suppliers', component: SupplierComponent }, 
    { path: 'priceList', component: PriceListComponent }, 
    { path: 'priceListItem', component: PriceListDetailsComponent }, 
    { path: 'client', component: ClientComponent }, 
    { path: 'workStations', component: WorkStationComponent }, 
    { path: 'workPlan', component: WorkPlanComponent }, 
    { path: 'employee', component: EmployeeComponent }, 
    { path: 'order', component: OrderComponent }, 
    { path: 'laboratoryDashboard', component: LabDashboardComponent }, 
    { path: 'station/:id', component: WorkDetailComponent }, 
    { path: 'users', component: UsersComponent }, 
    { path: 'productivity', component: ProductivityComponent }, 
    { path: 'comissions', component: CommissionReportComponent }, 
    { path: 'orderReport', component: OrdersReportComponent },  
    { path: 'paymentReport', component: PaymentsReportComponent },  
    { path: 'debtReport', component: DebtsReportComponent },  
    { path: 'workPiecesReport', component: PiecesReportComponent },  
    { path: 'paymentsDebtsReport', component: PaymentsDebtsComponent },  
    { path: 'empty', component: Empty }, 

    { path: '**', redirectTo: '/notfound' }
] as Routes;
