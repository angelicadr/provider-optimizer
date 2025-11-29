using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProviderOptimizer.Domain.Entities;

[Table("providers")]
public class Provider
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("isavailable")]
    public bool IsAvailable { get; set; } = true;

    [Column("latitude")]
    public double Latitude { get; set; }

    [Column("longitude")]
    public double Longitude { get; set; }

    [Column("rating")]
    public double Rating { get; set; }

    [Column("services")]
    public List<string> Services { get; set; } = new();
}
