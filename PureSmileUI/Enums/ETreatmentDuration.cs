using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PureSmileUI.Enums
{
    public enum ETreatmentDuration
    {
        [Display(Name = "15")]
        Fifteen = 15,
        [Display(Name = "30")]
        Thirty = 30,
        [Display(Name = "45")]
        FortyFive = 45,
        [Display(Name = "60")]
        Sixty = 60,
        [Display(Name = "90")]
        Ninety = 90,

    }
}