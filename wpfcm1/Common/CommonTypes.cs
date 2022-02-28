using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfcm1.Common
{
    public static class PoliSignTypes
    {
        public class Linija
        {
            private string id_lin;
            private string sf_kom;
            private string sf_pos;
            private string sf_lin;
            private string sf_voz;
            private string opis;

            public string Id_lin { get => id_lin; set => id_lin = value; }
            public string Sf_kom { get => sf_kom; set => sf_kom = value; }
            public string Sf_pos { get => sf_pos; set => sf_pos = value; }
            public string Sf_lin { get => sf_lin; set => sf_lin = value; }
            public string Sf_voz { get => sf_voz; set => sf_voz = value; }
            public string Opis { get => opis; set => opis = value; }
        }

        public class FileToDownload
        {
            private string f_naziv;
            public string F_naziv { get => f_naziv; set => f_naziv = value; }
        }



    }
}
