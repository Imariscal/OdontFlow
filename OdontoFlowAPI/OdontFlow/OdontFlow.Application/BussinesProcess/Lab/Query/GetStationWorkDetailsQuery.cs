using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
using OdontFlow.Domain.Repositories.Base;
using OdontFlow.Domain.ViewModel.StationWork;
using ViewModel = OdontFlow.Domain.ViewModel.StationWork.StationWorkDetailViewModel;
using Model = OdontFlow.Domain.Entities.StationWork;

namespace OdontFlow.Application.BussinesProcess.Lab.Query
{
    public class GetStationWorkDetailsQuery(Guid stationId)
    {
        public Guid StationId { get; set; } = stationId;
    }

    public class GetStationWorkDetailsQueryHandler(
       IReadOnlyRepository<Guid, Model> stationWorkRepo,
       IMapper mapper
   ) : IQueryHandler<GetStationWorkDetailsQuery, IEnumerable<ViewModel>>
    {
        public async Task<IEnumerable<ViewModel>> Handle(GetStationWorkDetailsQuery query)
        {
            var now = DateTime.Now;
            var result = new List<ViewModel>();

            var ordenes = await stationWorkRepo.GetAllMatchingAsync(sw => sw.WorkStationId == query.StationId);

            var ordenesId = ordenes.Select(sw => sw.OrderId)
                        .Distinct().ToList();

            var stationWorksAll = await stationWorkRepo.GetAllMatchingAsync(
                sw => ordenesId.Contains(sw.OrderId),
                new[] { "WorkStation", "Employee", "Order", "Order.Client", "Order.Items", "Order.Items.Product" });

            var stationWorks = stationWorksAll
             .Where(sw =>
                 sw.WorkStationId == query.StationId &&
                 (sw.WorkStatus == WORK_STATUS.ESPERA ||
                  sw.WorkStatus == WORK_STATUS.PROCESO ||
                  sw.WorkStatus == WORK_STATUS.BLOQUEADO ||
                  sw.WorkStatus == WORK_STATUS.RECHAZADO) &&
                 (
                     sw.Step == 1 ||
                     (
                         sw.Step > 1 &&
                         stationWorksAll.Any(prev =>
                             prev.OrderId == sw.OrderId &&
                             prev.Barcode == sw.Barcode &&
                             prev.Step == sw.Step - 1 &&
                             prev.WorkStatus == WORK_STATUS.TERMINADO
                         )
                     )
                 ))
             .ToList();

            foreach (var sw in stationWorks)
            {
                var matchingItem = sw.Order.Items.FirstOrDefault(i => i.ProductId == sw.ProductId);

                var teethNames = matchingItem?.Teeth?.Select(p => p.ToString()).ToList() ?? new List<string>();
                var workStatusIndicator = GetWorkStatusIndicator(sw, now);
                var tiempoPlaneado = (sw.StationEndDate - sw.StationStartDate).TotalMinutes;
                var tiempoReal = (sw.EmployeeEndDate != default && sw.EmployeeStartDate != default)
                                    ? (sw.EmployeeEndDate - sw.EmployeeStartDate).TotalMinutes : 0;

                var detail = new StationWorkDetailViewModel
                {
                    OrderId = sw.OrderId.Value,
                    OrderNumber = sw.Order.Barcode,
                    Barcode = sw.Barcode,
                    WorkStationName = sw.WorkStation.Name,
                    StationWorkId = sw.Id,
                    ProductName = matchingItem?.Product?.Name ?? "",
                    OrderColor = sw.Order.Color,
                    ClientName = sw.Order.Client?.Name ?? "",
                    Teeth = teethNames,
                    StationStartDate = sw.StationStartDate,
                    StationEndDate = sw.StationEndDate,
                    WorkStatusIndicator = workStatusIndicator,
                    EmployeeStartDate = sw.EmployeeStartDate,
                    EmployeeEndDate = sw.EmployeeEndDate,
                    WorkStatus = (int)sw.WorkStatus,
                    InProgress = sw.InProgress,
                    Step = sw.Step,
                    PreviousEmployeeName = sw.Employee?.Name,
                    WorkedOnTime = sw.EmployeeEndDate <= sw.StationEndDate,
                    ProductivityPercent = tiempoPlaneado > 0 ? $"{(tiempoReal / tiempoPlaneado * 100):F0}%" : "N/A",
                    DelayMinutes = now > sw.StationEndDate ? (int?)(now - sw.StationEndDate).TotalMinutes : null
                };

                if (sw.Step > 1)
                {
                    var previousResult = await stationWorkRepo.GetAllMatchingAsync(
                        p => p.OrderId == sw.OrderId && p.Step == sw.Step - 1,
                        new[] { "WorkStation", "Employee" });

                    var previousStep = previousResult.FirstOrDefault();

                    if (previousStep == null || previousStep.WorkStatus != WORK_STATUS.TERMINADO)
                        continue;

                    detail.PreviousStationName = previousStep.WorkStation.Name;
                    detail.PreviousEmployeeName = previousStep.Employee?.Name ?? "";
                    detail.PreviousEndDate = previousStep.EmployeeEndDate;
                }
                else
                {
                    detail.PreviousStationName = "N/A";
                    detail.PreviousEmployeeName = "N/A";
                    detail.PreviousEndDate = null;
                }

                result.Add(detail);
            }
            result = result.OrderBy(o => o.StationStartDate).ToList();
            return result;
        }

        private int GetWorkStatusIndicator(Model sw, DateTime now)
        {
            if (sw.WorkStatus == WORK_STATUS.TERMINADO)
                return 0; // Terminado siempre OK

            if (now > sw.StationEndDate)
                return 3; // Con retraso (ya pasó la fecha planeada)

            var total = (sw.StationEndDate - sw.StationStartDate).TotalMinutes;
            var elapsed = (now - sw.StationStartDate).TotalMinutes;

            if (elapsed <= total * 0.5)
                return 1; // A tiempo
            if (elapsed <= total * 0.75)
                return 2; // Con alarma

            return 3; // Con retraso
        } 
    }
}