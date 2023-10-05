using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SmartEco.Models.ASM.PollutionSources
{
    public class AirPollutionSourceInfo
    {
        public int SourceId { get; set; }
        public AirPollutionSource Source { get; set; }

        [Display(Name = "Координаты")]
        public string Coordinate { get; set; }

        [Display(Name = "Коэффициент рельефа")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public int? TerrainCoefficient { get; set; }

        [Display(Name = "Рассчитывать по газу")]
        public bool IsCalculateByGas { get; set; }

        [Display(Name = "Отклонение по вертикали")]
        public bool IsVerticalDeviation { get; set; }

        [Display(Name = "Угол отклонения по вертикали")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public decimal? AngleDeflection { get; set; }

        [Display(Name = "Угол поворота")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public decimal? AngleRotation { get; set; }

        [Display(Name = "Оборудован зонтом или крышкой")]
        public bool IsCovered { get; set; }

        [Display(Name = "Признак факельного горения")]
        public bool IsSignFlare { get; set; }

        [Display(Name = "Высота")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public decimal? Hight { get; set; }

        [Display(Name = "Диаметр")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public decimal? Diameter { get; set; }

        [Display(Name = "Отношение к фону")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public int? RelationBackgroundId { get; set; }
        public RelationBackground RelationBackground { get; set; }

        public List<RelationBackground> DropdownBackgrounds { get; set; }
    }

    public class RelationBackground
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
