using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CleanArc.Core.Entites;

[Table("medical_record")]
public partial class MedicalRecord
{

    
    public int Recordid { get; set; }
    public int Animalid { get; set; }
    public virtual Animal? Animal { get; set; }
    public string? injurys { get; set; }
    public string? Status { get; set; }
    public virtual ICollection<VaccinationNeeded> VaccinationNeededs { get; set; } = new List<VaccinationNeeded>();
}