using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CareerTrack.Core;

public class JobApplicationService
{
    public static IReadOnlyList<string> Statuses { get; } =
        Array.AsReadOnly(new[]
        {
            "Saved",
            "Applied",
            "Under Review",
            "Interview",
            "Offer",
            "Rejected",
            "Withdrawn"
        });

    public string DataFilePath { get; }

    public JobApplicationService()
    {
        string folder = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "CareerTrackDesktop");

        DataFilePath = Path.Combine(folder, "applications.json");
    }

    public List<JobApplication> GetAll()
    {
        if (!File.Exists(DataFilePath))
        {
            return new List<JobApplication>();
        }

        string json = File.ReadAllText(DataFilePath);

        return JsonSerializer.Deserialize<List<JobApplication>>(json)
            ?? throw new InvalidDataException(
                "The applications file does not contain a valid list.");
    }

    public void Add(JobApplication application)
    {
        Validate(application);

        List<JobApplication> applications = GetAll();

        application.Id = Guid.NewGuid();
        applications.Add(application);

        Save(applications);
    }

    public void Update(JobApplication application)
    {
        Validate(application);

        List<JobApplication> applications = GetAll();

        int index = applications.FindIndex(
            item => item.Id == application.Id);

        if (index == -1)
        {
            throw new InvalidOperationException(
                "The selected application could not be found.");
        }

        applications[index] = application;

        Save(applications);
    }

    public void Delete(Guid id)
    {
        List<JobApplication> applications = GetAll();

        int removed = applications.RemoveAll(item => item.Id == id);

        if (removed == 0)
        {
            throw new InvalidOperationException(
                "The selected application could not be found.");
        }

        Save(applications);
    }

    private static void Validate(JobApplication application)
    {
        if (string.IsNullOrWhiteSpace(application.Company))
        {
            throw new ArgumentException("Company name is required.");
        }

        if (string.IsNullOrWhiteSpace(application.JobTitle))
        {
            throw new ArgumentException("Job title is required.");
        }

        if (!Statuses.Contains(application.Status))
        {
            throw new ArgumentException("Select a valid status.");
        }

        application.Company = application.Company.Trim();
        application.JobTitle = application.JobTitle.Trim();
        application.Location = application.Location?.Trim() ?? "";
        application.Notes = application.Notes?.Trim() ?? "";
    }

    private void Save(List<JobApplication> applications)
    {
        string folder = Path.GetDirectoryName(DataFilePath)!;
        Directory.CreateDirectory(folder);

        string json = JsonSerializer.Serialize(
            applications,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        string temporaryPath = DataFilePath + ".tmp";

        File.WriteAllText(temporaryPath, json);
        File.Move(temporaryPath, DataFilePath, overwrite: true);
    }
}