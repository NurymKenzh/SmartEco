using System;

namespace SmartEco.Models.ASM.Responses
{
    public class JuridicalAccountResponse
    {
        public bool Success { get; set; }
        public Obj Obj { get; set; }
        public object Description { get; set; }
    }

    public class Obj
    {
        public int Id { get; set; }
        public string Bin { get; set; }
        public string Name { get; set; }
        public DateTime RegisterDate { get; set; }
        public string OkedCode { get; set; }
        public string OkedName { get; set; }
        public object SecondOkeds { get; set; }
        public string KrpCode { get; set; }
        public string KrpName { get; set; }
        public string KrpBfCode { get; set; }
        public string KrpBfName { get; set; }
        public string KseCode { get; set; }
        public string KseName { get; set; }
        public string KfsCode { get; set; }
        public string KfsName { get; set; }
        public string KatoCode { get; set; }
        public int KatoId { get; set; }
        public string KatoAddress { get; set; }
        public string Fio { get; set; }
        public bool Ip { get; set; }

        public string KatoComplex { get; set; }
    }
}
