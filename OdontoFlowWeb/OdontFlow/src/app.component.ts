import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { PrimengModule } from './app/shared/primeng.module';
import { AppSpinnerComponent } from './app/shared/components/app-spinner.component';

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [RouterModule, PrimengModule, AppSpinnerComponent],
    template: `<app-spinner></app-spinner><p-toast></p-toast><router-outlet></router-outlet>`
})
export class AppComponent {}
