import { NgModule } from '@angular/core';

// PrimeNG Modules
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { CardModule } from 'primeng/card';
import { TableModule } from 'primeng/table';
import { DropdownModule } from 'primeng/dropdown';
import { DialogModule } from 'primeng/dialog';
import { ToastModule } from 'primeng/toast';
import { ToolbarModule } from 'primeng/toolbar';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TooltipModule } from 'primeng/tooltip';
import { InputNumberModule } from 'primeng/inputnumber';
import { CheckboxModule } from 'primeng/checkbox';
import { RadioButtonModule } from 'primeng/radiobutton';
import { SidebarModule } from 'primeng/sidebar';
import { MenubarModule } from 'primeng/menubar';
import { PanelMenuModule } from 'primeng/panelmenu';
import { SplitButtonModule } from 'primeng/splitbutton';
import { BadgeModule } from 'primeng/badge';
import { MegaMenuModule } from 'primeng/megamenu';
import { MessagesModule } from 'primeng/messages';
import { MessageModule } from 'primeng/message';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { ConfirmationService } from 'primeng/api';
import { TabsModule } from 'primeng/tabs';
import { RippleModule } from 'primeng/ripple'; 
import { RatingModule } from 'primeng/rating'; 
import { TextareaModule } from 'primeng/textarea';
import { SelectModule } from 'primeng/select'; 
import { TagModule } from 'primeng/tag';
import { InputIconModule } from 'primeng/inputicon';
import { IconFieldModule } from 'primeng/iconfield'; 
import { ToggleSwitchModule } from 'primeng/toggleswitch';
import { PickListModule } from 'primeng/picklist';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { FieldsetModule } from 'primeng/fieldset';
import { DatePickerModule } from 'primeng/datepicker';
import { TimelineModule } from 'primeng/timeline';
import { InputNumber } from 'primeng/inputnumber';
import { DividerModule } from 'primeng/divider';
import { ProgressBar } from 'primeng/progressbar';
import { FileUpload } from 'primeng/fileupload';
import { MenuModule } from 'primeng/menu';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { PanelModule } from 'primeng/panel'; 
 
const primengModules = [
  ProgressBar, 
  FileUpload,
  ButtonModule,
  InputTextModule,
  PasswordModule,
  CardModule,
  TableModule,
  DropdownModule,
  DialogModule,
  ToastModule,
  ToolbarModule,
  ConfirmDialogModule,
  TooltipModule,
  InputNumberModule,
  CheckboxModule,
  RadioButtonModule, 
  SidebarModule,
  MenubarModule,
  PanelMenuModule,
  SplitButtonModule,
  BadgeModule,
  MegaMenuModule,
  MessagesModule,
  MessageModule,
  ProgressSpinnerModule,  
  RippleModule,
  RatingModule,
  SelectModule,
  TextareaModule,
  TagModule,
  InputIconModule,
  IconFieldModule,
  TabsModule,
  ToggleSwitchModule,
  PickListModule,
  AutoCompleteModule,
  FieldsetModule,
  DatePickerModule,
  TimelineModule,
  InputNumber,
  DividerModule,
  MenuModule,
  OverlayPanelModule,
  PanelModule 
];

@NgModule({
  imports: [...primengModules],
  exports: [...primengModules],
  providers: [ConfirmationService]
})
export class PrimengModule {}
