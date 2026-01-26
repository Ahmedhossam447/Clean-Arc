using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CleanArc.Core.Entites;

public partial class VaccinationNeeded
{
    public int Id { get; set; }

    public int? Medicalid { get; set; }

    public virtual MedicalRecord? MedicalRecord { get; set; }

    public string? VaccineName { get; set; }
}