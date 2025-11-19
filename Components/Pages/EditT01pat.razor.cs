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
    public partial class EditT01pat
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

        [Parameter]
        public int Mednum { get; set; }

        protected override async Task OnInitializedAsync()
        {
            t01pat = await medrecService.GetT01patByMednum(Mednum);
        }
        protected bool errorVisible;
        protected Medrec.Models.medrec.T01pat t01pat;

        protected async Task FormSubmit()
        {
            try
            {
                await medrecService.UpdateT01pat(Mednum, t01pat);
                DialogService.Close(t01pat);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}