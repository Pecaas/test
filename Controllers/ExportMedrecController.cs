using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using Medrec.Data;

namespace Medrec.Controllers
{
    public partial class ExportmedrecController : ExportController
    {
        private readonly medrecContext context;
        private readonly medrecService service;

        public ExportmedrecController(medrecContext context, medrecService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/medrec/t01pats/csv")]
        [HttpGet("/export/medrec/t01pats/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportT01patsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetT01pats(), Request.Query, false), fileName);
        }

        [HttpGet("/export/medrec/t01pats/excel")]
        [HttpGet("/export/medrec/t01pats/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportT01patsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetT01pats(), Request.Query, false), fileName);
        }

        [HttpGet("/export/medrec/t01patids/csv")]
        [HttpGet("/export/medrec/t01patids/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportT01patidsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetT01patids(), Request.Query, false), fileName);
        }

        [HttpGet("/export/medrec/t01patids/excel")]
        [HttpGet("/export/medrec/t01patids/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportT01patidsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetT01patids(), Request.Query, false), fileName);
        }

        [HttpGet("/export/medrec/t03events/csv")]
        [HttpGet("/export/medrec/t03events/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportT03eventsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetT03events(), Request.Query, false), fileName);
        }

        [HttpGet("/export/medrec/t03events/excel")]
        [HttpGet("/export/medrec/t03events/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportT03eventsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetT03events(), Request.Query, false), fileName);
        }

        [HttpGet("/export/medrec/t02descrs/csv")]
        [HttpGet("/export/medrec/t02descrs/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportT02descrsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetT02descrs(), Request.Query, false), fileName);
        }

        [HttpGet("/export/medrec/t02descrs/excel")]
        [HttpGet("/export/medrec/t02descrs/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportT02descrsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetT02descrs(), Request.Query, false), fileName);
        }

        [HttpGet("/export/medrec/t02measures/csv")]
        [HttpGet("/export/medrec/t02measures/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportT02measuresToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetT02measures(), Request.Query, false), fileName);
        }

        [HttpGet("/export/medrec/t02measures/excel")]
        [HttpGet("/export/medrec/t02measures/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportT02measuresToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetT02measures(), Request.Query, false), fileName);
        }

        [HttpGet("/export/medrec/t03measures/csv")]
        [HttpGet("/export/medrec/t03measures/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportT03measuresToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetT03measures(), Request.Query, false), fileName);
        }

        [HttpGet("/export/medrec/t03measures/excel")]
        [HttpGet("/export/medrec/t03measures/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportT03measuresToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetT03measures(), Request.Query, false), fileName);
        }

        [HttpGet("/export/medrec/t90tab1s/csv")]
        [HttpGet("/export/medrec/t90tab1s/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportT90tab1SToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetT90tab1S(), Request.Query, false), fileName);
        }

        [HttpGet("/export/medrec/t90tab1s/excel")]
        [HttpGet("/export/medrec/t90tab1s/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportT90tab1SToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetT90tab1S(), Request.Query, false), fileName);
        }

        [HttpGet("/export/medrec/versions/csv")]
        [HttpGet("/export/medrec/versions/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportVersionsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetVersions(), Request.Query, false), fileName);
        }

        [HttpGet("/export/medrec/versions/excel")]
        [HttpGet("/export/medrec/versions/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportVersionsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetVersions(), Request.Query, false), fileName);
        }
    }
}
