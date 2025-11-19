using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using System.Security.Cryptography;

namespace Medrec.Components.Pages
{
    public partial class T01pats
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

        protected IEnumerable<Medrec.Models.medrec.T01pat> t01pats;

        protected RadzenDataGrid<Medrec.Models.medrec.T01pat> grid0;

        protected string search = "";
        protected void OnInput(ChangeEventArgs args)
        {
            search = args.Value?.ToString() ?? "";
        }

        protected async Task Search(KeyboardEventArgs args)
        {
            if (args.Key != "Enter") return;

            await grid0.GoToPage(0);
            t01pats = await medrecService.GetT01pats(
                new Query { Filter = $@"i => i.Persid.Contains(@0) || i.Lname.Contains(@0) || i.Iname.Contains(@0)", 
                    FilterParameters = new object[] { search } });

            //t01pats = await medrecService.GetT01pats(search);
        }

        protected override async Task OnInitializedAsync()
        {
            //   t01pats = await medrecService.GetT01pats("");
            //    t01pats = await medrecService.GetT01pats(new Query { Filter = $@"i => i.Persid.Contains(@0) || i.Lname.Contains(@0) || i.Iname.Contains(@0)", FilterParameters = new object[] { search } });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddT01pat>("Add T01pat", null);
            await grid0.Reload();
        }

        protected async Task EditRow(DataGridRowMouseEventArgs<Medrec.Models.medrec.T01pat> args)
        {
            await DialogService.OpenAsync<EditT01pat>("Edit T01pat", new Dictionary<string, object> { { "Mednum", args.Data.Mednum } });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Medrec.Models.medrec.T01pat t01pat)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await medrecService.DeleteT01pat(t01pat.Mednum);

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
                    Detail = $"Unable to delete T01pat"
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await medrecService.ExportT01patsToCSV(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter) ? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "T01pats");
            }

            if (args == null || args.Value == "xlsx")
            {
                await medrecService.ExportT01patsToExcel(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter) ? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "T01pats");
            }
        }
    }
}