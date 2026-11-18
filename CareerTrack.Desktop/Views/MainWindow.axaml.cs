using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CareerTrack.Core;

namespace CareerTrack.Desktop.Views;

public partial class MainWindow : Window
{
    private readonly JobApplicationService _service = new();

    private List<JobApplication> _applications = new();

    private Guid? _selectedId;
    private bool _ready;

    public MainWindow()
    {
        InitializeComponent();

        StatusBox.ItemsSource = JobApplicationService.Statuses;

        ClearForm();

        _ready = true;

        try
        {
            LoadApplications();
            MessageText.Text = "Ready. Add an application to get started.";
        }
        catch (Exception ex)
        {
            MessageText.Text = $"Could not load applications: {ex.Message}";
        }
    }

    private void LoadApplications()
    {
        _applications = _service.GetAll();
        ApplySearch();
    }

    private void ApplySearch()
    {
        string search = SearchBox.Text?.Trim() ?? "";

        var filtered = _applications
            .Where(application =>
                application.Company.Contains(
                    search, StringComparison.OrdinalIgnoreCase)
                || application.JobTitle.Contains(
                    search, StringComparison.OrdinalIgnoreCase)
                || application.Location.Contains(
                    search, StringComparison.OrdinalIgnoreCase)
                || application.Status.Contains(
                    search, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(application => application.DateApplied)
            .ThenBy(application => application.Company)
            .ToList();

        ApplicationsList.ItemsSource = filtered;

        int interviews = _applications.Count(
            application => application.Status == "Interview");

        int offers = _applications.Count(
            application => application.Status == "Offer");

        SummaryText.Text =
            $"Total: {_applications.Count}   |   " +
            $"Interviews: {interviews}   |   " +
            $"Offers: {offers}   |   Showing: {filtered.Count}";
    }

    private void SearchBox_TextChanged(
        object? sender, TextChangedEventArgs e)
    {
        if (!_ready)
        {
            return;
        }

        ApplySearch();
        ClearForm();
    }

    private void ApplicationsList_SelectionChanged(
        object? sender, SelectionChangedEventArgs e)
    {
        if (!_ready)
        {
            return;
        }

        if (ApplicationsList.SelectedItem is not JobApplication application)
        {
            ClearForm();
            return;
        }

        _selectedId = application.Id;

        CompanyBox.Text = application.Company;
        JobTitleBox.Text = application.JobTitle;
        LocationBox.Text = application.Location;
        StatusBox.SelectedItem = application.Status;

        AppliedDatePicker.SelectedDate =
            new DateTimeOffset(application.DateApplied);

        NotesBox.Text = application.Notes;

        FormHeading.Text = "Edit application";
        DeleteButton.IsEnabled = true;
        ConfirmDeleteBox.IsChecked = false;

        MessageText.Text = "Edit the fields, then click Save application.";
    }

    private void Save_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (AppliedDatePicker.SelectedDate is not DateTimeOffset date)
            {
                MessageText.Text = "Please select an application date.";
                return;
            }

            bool updating = _selectedId.HasValue;

            var application = new JobApplication
            {
                Id = _selectedId ?? Guid.NewGuid(),
                Company = CompanyBox.Text ?? "",
                JobTitle = JobTitleBox.Text ?? "",
                Location = LocationBox.Text ?? "",
                Status = StatusBox.SelectedItem as string ?? "",
                DateApplied = date.Date,
                Notes = NotesBox.Text ?? ""
            };

            if (updating)
            {
                _service.Update(application);
            }
            else
            {
                _service.Add(application);
            }

            LoadApplications();
            ClearForm();

            MessageText.Text = updating
                ? "Application updated successfully."
                : "Application added successfully.";
        }
        catch (Exception ex)
        {
            MessageText.Text = $"Could not save application: {ex.Message}";
        }
    }

    private void New_Click(object? sender, RoutedEventArgs e)
    {
        ClearForm();
        MessageText.Text = "Enter the new application's details.";
    }

    private void Delete_Click(object? sender, RoutedEventArgs e)
    {
        if (_selectedId is not Guid id)
        {
            MessageText.Text = "Select an application first.";
            return;
        }

        if (ConfirmDeleteBox.IsChecked != true)
        {
            MessageText.Text =
                "Check the confirmation box before deleting.";
            return;
        }

        try
        {
            _service.Delete(id);

            LoadApplications();
            ClearForm();

            MessageText.Text = "Application deleted successfully.";
        }
        catch (Exception ex)
        {
            MessageText.Text = $"Could not delete application: {ex.Message}";
        }
    }

    private void ClearForm()
    {
        _selectedId = null;

        ApplicationsList.SelectedItem = null;

        CompanyBox.Text = "";
        JobTitleBox.Text = "";
        LocationBox.Text = "";
        StatusBox.SelectedItem = "Applied";
        AppliedDatePicker.SelectedDate = DateTimeOffset.Now;
        NotesBox.Text = "";

        FormHeading.Text = "New application";
        DeleteButton.IsEnabled = false;
        ConfirmDeleteBox.IsChecked = false;
    }
}