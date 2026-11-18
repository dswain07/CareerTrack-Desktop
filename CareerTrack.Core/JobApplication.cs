using System;

namespace CareerTrack.Core;

public class JobApplication
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Company { get; set; } = "";

    public string JobTitle { get; set; } = "";

    public string Location { get; set; } = "";

    public string Status { get; set; } = "Applied";

    public DateTime DateApplied { get; set; } = DateTime.Today;

    public string Notes { get; set; } = "";
}