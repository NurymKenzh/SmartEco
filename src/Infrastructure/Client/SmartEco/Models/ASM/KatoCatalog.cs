using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartEco.Models.ASM
{
    public class KatoCatalog
    {
        [Display(Name = "№ п/п")]
        public int Id { get; set; }

        [Display(Name = "Внешний объект")]
        public int ParentId { get; set; }

        [Display(Name = "Код")]
        public string Code { get; set; }

        [Display(Name = "Наименование")]
        public string Name { get; set; }

        public string CodeName => $"{Code} {Name}";
    }

    class KatoCatalogDownHierarchy : KatoCatalog
    {
        public List<KatoCatalogDownHierarchy> Children { get; set; }
    }

    class KatoCatalogUpHierarchy : KatoCatalog
    {
        public KatoCatalogUpHierarchy Parent { get; set; }
    }
}
