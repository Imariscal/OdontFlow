import { StationWorkDetailViewModel } from "./station-work-details.model";
import { StationWorkByEmployeeViewModel, StationWorkByStationViewModel } from "./station-work-summary.model";
 
export interface ProductivityReportViewModel {
  totalTrabajos?: number;
  trabajosEnEspera?: number;
  trabajosEnProceso?: number;
  trabajosTerminados?: number;
  trabajosBloqueados?: number;
  trabajosRechazados?: number;
  trabajosEnTiempo?: number;
  trabajosConAlarma?: number;
  trabajosConRetraso?: number;
  trabajosTerminadosHoy?: number;
  trabajosComprometidosHoy?: number;
  porEstacion?: StationWorkByStationViewModel[];
  porEmpleado?: StationWorkByEmployeeViewModel[];
  detallePorEstacion?: StationWorkDetailViewModel[];
  detallePorEmpleado?: StationWorkDetailViewModel[];
}
