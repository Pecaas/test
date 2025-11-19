using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace Medrec.Components.Pages
{
    public partial class T90tab1S
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        public medrecService medrecService { get; set; }

        protected IEnumerable<Medrec.Models.medrec.T90tab1> t90tab1S;

        protected RadzenDataGrid<Medrec.Models.medrec.T90tab1> grid0;

        protected string search = "";

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            t90tab1S = await medrecService.GetT90tab1S(new Query { Filter = $@"i => i.Abbrev.Contains(@0) || i.Descr.Contains(@0) || i.Descr1.Contains(@0) || i.Par01.Contains(@0) || i.Par02.Contains(@0) || i.Par03.Contains(@0) || i.Extid.Contains(@0) || i.Data1x.Contains(@0)", FilterParameters = new object[] { search } });
        }
        protected override async Task OnInitializedAsync()
        {
            t90tab1S = await medrecService.GetT90tab1S(new Query { Filter = $@"i => i.Abbrev.Contains(@0) || i.Descr.Contains(@0) || i.Descr1.Contains(@0) || i.Par01.Contains(@0) || i.Par02.Contains(@0) || i.Par03.Contains(@0) || i.Extid.Contains(@0) || i.Data1x.Contains(@0)", FilterParameters = new object[] { search } });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddT90tab1>("Add T90tab1", null);
            await grid0.Reload();
        }

        protected async Task EditRow(DataGridRowMouseEventArgs<Medrec.Models.medrec.T90tab1> args)
        {
            await DialogService.OpenAsync<EditT90tab1>("Edit T90tab1", new Dictionary<string, object> { {"Serial", args.Data.Serial} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Medrec.Models.medrec.T90tab1 t90tab1)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await medrecService.DeleteT90tab1(t90tab1.Serial);

                    if (deleteResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete T90tab1"
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await medrecService.ExportT90tab1SToCSV(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "T90tab1S");
            }

            if (args == null || args.Value == "xlsx")
            {
                await medrecService.ExportT90tab1SToExcel(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "T90tab1S");
            }
        }
    }
}