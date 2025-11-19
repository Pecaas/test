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
    public partial class T03events
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

        protected IEnumerable<Medrec.Models.medrec.T03event> t03events;

        protected RadzenDataGrid<Medrec.Models.medrec.T03event> grid0;

        protected string search = "";

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            t03events = await medrecService.GetT03events(new Query { Filter = $@"i => i.Eventnum.Contains(@0) || i.Ownertag.Contains(@0) || i.Userid.Contains(@0) || i.Indiclink.Contains(@0) || i.Extid.Contains(@0) || i.Data1x.Contains(@0) || i.Params.Contains(@0) || i.Dynitems.Contains(@0)", FilterParameters = new object[] { search } });
        }
        protected override async Task OnInitializedAsync()
        {
            t03events = await medrecService.GetT03events(new Query { Filter = $@"i => i.Eventnum.Contains(@0) || i.Ownertag.Contains(@0) || i.Userid.Contains(@0) || i.Indiclink.Contains(@0) || i.Extid.Contains(@0) || i.Data1x.Contains(@0) || i.Params.Contains(@0) || i.Dynitems.Contains(@0)", FilterParameters = new object[] { search } });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddT03event>("Add T03event", null);
            await grid0.Reload();
        }

        protected async Task EditRow(DataGridRowMouseEventArgs<Medrec.Models.medrec.T03event> args)
        {
            await DialogService.OpenAsync<EditT03event>("Edit T03event", new Dictionary<string, object> { {"Serial", args.Data.Serial} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Medrec.Models.medrec.T03event t03event)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await medrecService.DeleteT03event(t03event.Serial);

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
                    Detail = $"Unable to delete T03event"
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await medrecService.ExportT03eventsToCSV(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "T03events");
            }

            if (args == null || args.Value == "xlsx")
            {
                await medrecService.ExportT03eventsToExcel(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "T03events");
            }
        }
    }
}