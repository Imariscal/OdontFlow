using MapsterMapper;
using OdontFlow.Application.BussinesProcess.Base.Contracts;
 using OdontFlow.Domain.Repositories.Base;
using Model = OdontFlow.Domain.Entities.OrderSequence;
 
namespace OdontFlow.Application.BussinesProcess.Order.Command;

public class CreateOrderSecuenceCommand(string Date) : ICommand<Model>
{
    public string Date { get; set; } = Date;
}

public class CreateOrderSecuenceCommandHandler(
    IWriteOnlyRepository<Guid, Model> writeRepository,
        IReadOnlyRepository<Guid, Model> readRepository,
    IMapper mapper) : ICommandHandler<CreateOrderSecuenceCommand, Model>
{
    public async Task<Model> Handle(CreateOrderSecuenceCommand command)
    {
        var today = DateTime.Now;
        var sequence = await readRepository.GetAllMatchingAsync(x => x.Date == command.Date);

        if (!sequence.Any())
        {
            var secuence = new Model
            {
                Id = Guid.NewGuid(),
                Date = command.Date,
                LastNumber = 1,
                LastUpdated = today
            };
           
            await writeRepository.AddAsync(secuence);
            return secuence;

        }
        else
        {
            var seq = sequence.FirstOrDefault();
            seq.LastNumber += 1;
            seq.LastUpdated = today;
            await writeRepository.Modify(seq);
            return seq;
        }
    }
}