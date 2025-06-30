using OdontFlow.Domain.Entities.Base;
using OdontFlow.Domain.Entities.Base.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdontFlow.Domain.Entities
{
    public class OrderSequence : Auditable<Guid>
    { 
        [MaxLength(6)]
        public string Date { get; set; } = default!; 
        public int LastNumber { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
