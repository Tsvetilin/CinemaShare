using System.ComponentModel.DataAnnotations;

namespace Data.Enums
{
    
    public enum ProjectionType
    {
        [Display(Name ="2D")]
        _2D,
        [Display(Name = "3D")]
        _3D,
        [Display(Name ="4D")]
        _4D,
    }
}
