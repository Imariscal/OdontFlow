import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { AppMenuitem } from './app.menuitem';
import { AuthService } from '../../core/services/auth.service';

@Component({
    selector: 'app-menu',
    standalone: true,
    imports: [CommonModule, AppMenuitem, RouterModule],
    template: `<ul class="layout-menu">
        <ng-container *ngFor="let item of model; let i = index">
            <li app-menuitem *ngIf="!item.separator" [item]="item" [index]="i" [root]="true" ></li>
            <li *ngIf="item.separator" class="menu-separator"></li>
        </ng-container>
    </ul> `
})
export class AppMenu {


    authService = inject(AuthService);

    model: MenuItem[] = [];

    private adminMenu: MenuItem[] = [
        {
          label: 'Home',
          items: [{ label: 'Dashboard', icon: 'pi pi-fw pi-home', routerLink: ['/'] }]
        },
        {
          label: 'Administración',
          items: [
            {
              label: 'Dashboard', icon: 'pi pi-fw pi-home', routerLink: ['/pages/laboratoryDashboard'] 
            },
            {
              label: 'Catálogos',
              icon: 'pi pi-fw pi-wrench',
              items: [
                { label: 'Clientes', icon: 'pi pi-fw pi-address-book', routerLink: ['/pages/client'] },
                { label: 'Empleados', icon: 'pi pi-fw pi-address-book', routerLink: ['/pages/employee'] },
                {
                  label: 'Lista Precios',
                  icon: 'pi pi-fw pi-list',
                  items: [
                    { label: 'Lista', icon: 'pi pi-fw pi-bars', routerLink: ['/pages/priceList'] },
                    { label: 'Productos', icon: 'pi pi-fw pi-bars', routerLink: ['/pages/priceListItem'] }
                  ]
                },
                { label: 'Productos', icon: 'pi pi-fw pi-shop', routerLink: ['/pages/products'] },
                { label: 'Proveedores', icon: 'pi pi-fw pi-id-card', routerLink: ['/pages/suppliers'] }
              ]
            },
            { label: 'Ordenes', icon: 'pi pi-fw pi-inbox', routerLink: ['/pages/order'] }
          ]
        },
        {
          label: 'Laboratorio',
          items: [

            {
              label: 'Configuración',
              icon: 'pi pi-fw pi-wrench',
              items: [
                { label: 'Estaciones', icon: 'pi pi-fw pi-desktop', routerLink: ['/pages/workStations'] },
                { label: 'Planes de Trabajo', icon: 'pi pi-fw pi-sitemap', routerLink: ['/pages/workPlan'] }
              ]
            },
            {
              label: 'Reportes',
              icon: 'pi pi-fw pi-chart-bar',
              items: [
                { label: 'Productividad', icon: 'pi pi-fw pi-chart-bar', routerLink: ['/pages/productivity'] }
              ]
            }
          ]
        },
        {
          label: 'Reportes',
          icon: 'pi pi-fw pi-chart-bar',
          items: [
            { label: 'Adeudos', icon: 'pi pi-fw pi-chart-bar', routerLink: ['/pages/debtReport'] }, 
            { label: 'Adeudos/Pagos', icon: 'pi pi-fw pi-chart-bar', routerLink: ['/pages/paymentsDebtsReport'] },
            { label: 'Comisiones', icon: 'pi pi-fw pi-chart-bar', routerLink: ['/pages/comissions'] },
            { label: 'General', icon: 'pi pi-fw pi-chart-bar', routerLink: ['/pages/orderReport'] },
            { label: 'Pagos', icon: 'pi pi-fw pi-chart-bar', routerLink: ['/pages/paymentReport'] },
            { label: 'Piezas Trabajadas', icon: 'pi pi-fw pi-chart-bar', routerLink: ['/pages/workPiecesReport'] }       ]
        },
        {
          label: 'Seguridad',
          icon: 'pi pi-fw pi-briefcase',
          items: [
            { label: 'Usuarios', icon: 'pi pi-fw pi-address-book', routerLink: ['/pages/users'] },
            { label: 'Log out', icon: 'pi pi-fw pi-sign-out', routerLink: ['/auth/logout'] }
          ]
        }
      ];
      
      private labMenu: MenuItem[] = [
        {
          label: 'Home',
          items: [{ label: 'Dashboard', icon: 'pi pi-fw pi-home', routerLink: ['/pages/laboratoryDashboard'] }]
        },
        {
          label: 'Laboratorio',
          items: [ 
            {
              label: 'Reportes',
              icon: 'pi pi-fw pi-chart-bar',
              items: [
                { label: 'Productividad', icon: 'pi pi-fw pi-chart-bar', routerLink: ['/pages/productivity'] }
              ]
            }
          ]
        },
        {
          label: 'Seguridad',
          icon: 'pi pi-fw pi-briefcase',
          items: [
            { label: 'Log out', icon: 'pi pi-fw pi-sign-out', routerLink: ['/auth/logout'] }
          ]
        }
      ];
      
      private clientMenu: MenuItem[] = [
        {
          label: 'Home',
          items: [{ label: 'Dashboard', icon: 'pi pi-fw pi-home', routerLink: ['/'] }]
        },
        {
          label: 'Mis Órdenes',
          icon: 'pi pi-fw pi-inbox',
          items: [
            { label: 'Ver Órdenes', icon: 'pi pi-fw pi-eye', routerLink: ['/pages/client-orders'] }
          ]
        },
        {
          label: 'Seguridad',
          icon: 'pi pi-fw pi-briefcase',
          items: [
            { label: 'Log out', icon: 'pi pi-fw pi-sign-out', routerLink: ['/auth/logout'] }
          ]
        }
      ];
      


    ngOnInit() {
        const role = this.authService.getUserRole();

        switch (role) {
          case 'ADMIN':
            this.model = this.adminMenu;
            break;
          case 'LABORATORIO':
            this.model = this.labMenu;
            break;
          case 'CLIENTE':
            this.model = this.clientMenu;
            break;
          default:
            this.model = []; // o podrías redirigir o mostrar error
        }
    }

    onClick(item: any) {
        console.log(item);
    }
}
