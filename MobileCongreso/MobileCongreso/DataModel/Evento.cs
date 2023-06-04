using System;
using System.Collections.Generic;
using System.Text;

namespace MobileCongreso.DataModel
{
    public class Evento
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public DateTime? HoraInicio { get; set; }
        public DateTime? HoraTermino { get; set; }
    }
}
