using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfcm1.Common
{
    public static class UserDetails
    {
        private static string sfKom;
        private static string regBrVozila;
        private static string sfPos;
        private static string currentToken;

        public static string SfKom { get => sfKom; set => sfKom = value; }
        public static string RegBrVozila { get => regBrVozila; set => regBrVozila = value; }
        public static string SfPos { get => sfPos; set => sfPos = value; }
        public static string CurrentToken { get => currentToken; set => currentToken = value; }
    }
}
