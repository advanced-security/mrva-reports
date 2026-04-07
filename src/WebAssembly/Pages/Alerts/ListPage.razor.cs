using Microsoft.AspNetCore.Components;
using MRVA.Reports.Data.Models;
using MRVA.Reports.Data.Services;
using MRVA.Reports.WebAssembly.Properties;
using MudBlazor;

namespace MRVA.Reports.WebAssembly.Pages.Alerts;

public partial class ListPage
{
    [Inject]
    public required DataStore DataStore { get; set; }

    private List<BreadcrumbItem> BreadcrumbItems =>
    [
        new(ScreenText.Home, href: "/"),
        new(ScreenText.Alerts, href: null, disabled: true),
    ];

    public record AlertRow(Alert Alert, string RuleName, string RuleKind, string RepositoryName, string Severity);

    [Parameter]
    [SupplyParameterFromQuery(Name = "search")]
    public string? InitialSearch { get; set; }

    private string? SearchString { get; set; }

    private MudDataGrid<AlertRow>? _dataGrid;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        SearchString = InitialSearch;
    }

    private Task<GridData<AlertRow>> LoadServerData(GridState<AlertRow> state)
    {
        var (details, totalItems) = DataStore.GetAlertDetailsPaged(state.Page, state.PageSize, SearchString);

        var rows = details
            .Select(d => new AlertRow(d.Alert, d.RuleName, d.RuleKind, d.RepositoryName, d.Severity))
            .ToList();

        return Task.FromResult(new GridData<AlertRow>
        {
            TotalItems = totalItems,
            Items = rows,
        });
    }

    private void OnSearchChanged(string? value)
    {
        SearchString = value;
        _dataGrid?.ReloadServerData();
    }
}
