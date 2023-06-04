using System;
using System.Collections.Generic;
using System.Text;

namespace MobileCongreso.DataModel
{
    public class Registro
    {
        public string Id { get; set; }
        public string ParticipanteId { get; set; }
        public string EventoId { get; set; }
        public DateTime? HoraDeRegistro { get; set; }
        public bool Asistencia { get; set; } = false;
    }
}
