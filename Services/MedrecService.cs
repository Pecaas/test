using DocumentFormat.OpenXml.InkML;
using Medrec.Data;
using Medrec.Models.medrec;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Radzen;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Medrec
{
    public partial class medrecService
    {
        medrecContext Context
        {
           get
           {
             return this.context;
           }
        }

        private readonly medrecContext context;
        private readonly NavigationManager navigationManager;

        public medrecService(medrecContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        public void ApplyQuery<T>(ref IQueryable<T> items, Query query = null)
        {
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }
        }

        public async Task ExportT01patsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/medrec/t01pats/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/medrec/t01pats/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportT01patsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/medrec/t01pats/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/medrec/t01pats/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// ///////////////////////////////////////////////

        public async Task<List<T01pat>> GetT01pats(Query query, CancellationToken cancellationToken = default)
        {
            if (query == null || query.FilterParameters == null || query.FilterParameters.Length == 0)
                return await context.T01pats.ToListAsync(cancellationToken);

            var search = query.FilterParameters[0]?.ToString();
            if (string.IsNullOrWhiteSpace(search))
                return await context.T01pats.ToListAsync(cancellationToken);

            // pøipravíme LIKE s procenty
            var searchLike = "%" + search + "%";

            // SQL podle tvého pùvodního vzoru
            var sql = @"
        SELECT * FROM T01PAT
        WHERE PERSID LIKE {0}
           OR LNAME LIKE {0}
           OR INAME LIKE {0}";

            return await context.T01pats
                .FromSqlRaw(sql, searchLike)
                .ToListAsync(cancellationToken);
        }

        public class QueryResult<T>
        {
            public IEnumerable<T> Result { get; set; }
            public int Count { get; set; }
        }

        public async Task<QueryResult<T01pat>> SearchT01pats(Query query)
        {
            // --------------- FILTER ----------------
            var search = query.FilterParameters?.FirstOrDefault()?.ToString() ?? "";
            var searchLike = "%" + search + "%";

            // WHERE pøes LIKE (Firebird)
            var whereSql = @"
        FROM T01PAT
        WHERE PERSID LIKE {0}
           OR LNAME LIKE {0}
           OR FNAME LIKE {0}
           OR INSID LIKE {0}
           OR POSTCODE LIKE {0}
           OR IDENTNUM LIKE {0}
           OR V LIKE {0}
           OR DG LIKE {0}
           OR WARNING LIKE {0}
           OR USERID LIKE {0}
           OR INDICLINK LIKE {0}
           OR DATA1X LIKE {0}
           OR PARAMS LIKE {0}
           OR DYNITEMS LIKE {0}
           OR INAME LIKE {0}
    ";

            // --------------- COUNT ----------------
            var countSql = "SELECT COUNT(*) " + whereSql;

            var count = await context.Set<T01pat>()
                .FromSqlRaw(countSql, searchLike)
                .Select(r => EF.Property<int>(r, "COUNT"))
                .FirstAsync();

            // --------------- ORDER BY ----------------
            string orderBy = "";
            if (!string.IsNullOrEmpty(query.OrderBy))
                orderBy = " ORDER BY " + query.OrderBy; // Query je bezpeèné, generuje Radzen

            // --------------- PAGING ----------------
            string paging = "";
            if (query.Skip.HasValue && query.Top.HasValue)
                paging = $" ROWS {query.Skip.Value + 1} TO {query.Skip.Value + query.Top.Value}";

            // --------------- SELECT DATA ----------------
            var selectSql = "SELECT * " + whereSql + orderBy + paging;

            var items = await context.Set<T01pat>()
                .FromSqlRaw(selectSql, searchLike)
                .ToListAsync();

            return new QueryResult<T01pat>()
            {
                Count = count,
                Result = items
            };
        }

        public async Task<QueryResult<T01pat>> SearchT01patsFromLoadArgs(LoadDataArgs args)
        {
            var search = args.Filter ?? "";
            var searchLike = "%" + search + "%";

            // WHERE
            var whereSql = @"
        FROM T01PAT
        WHERE PERSID LIKE {0}
           OR LNAME LIKE {0}
           OR FNAME LIKE {0}
           OR INSID LIKE {0}
           OR POSTCODE LIKE {0}
           OR IDENTNUM LIKE {0}
           OR V LIKE {0}
           OR DG LIKE {0}
           OR WARNING LIKE {0}
           OR USERID LIKE {0}
           OR INDICLINK LIKE {0}
           OR DATA1X LIKE {0}
           OR PARAMS LIKE {0}
           OR DYNITEMS LIKE {0}
           OR INAME LIKE {0}
    ";

            // COUNT
            var count = await context.T01pats
                .FromSqlRaw("SELECT COUNT(*) " + whereSql, searchLike)
                .Select(r => EF.Property<int>(r, "COUNT"))
                .FirstAsync();

            // ORDER BY
            string orderBy = "";
            if (!string.IsNullOrEmpty(args.OrderBy))
                orderBy = " ORDER BY " + args.OrderBy;

            // PAGING
            var skip = args.Skip ?? 0;
            var top = args.Top ?? 20;

            string paging = $" ROWS {skip + 1} TO {skip + top}";

            // SELECT
            var items = await context.T01pats
                .FromSqlRaw("SELECT * " + whereSql + orderBy + paging, searchLike)
                .ToListAsync();

            return new QueryResult<T01pat>
            {
                Count = count,
                Result = items
            };
        }

        /// ///////////////////////////////////////////

        partial void OnT01patsRead(ref IQueryable<Medrec.Models.medrec.T01pat> items);

        public async Task<IQueryable<Medrec.Models.medrec.T01pat>> GetT01pats(Query query = null)
        {
            var items = Context.T01pats.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnT01patsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnT01patGet(Medrec.Models.medrec.T01pat item);
        partial void OnGetT01patByMednum(ref IQueryable<Medrec.Models.medrec.T01pat> items);


        public async Task<Medrec.Models.medrec.T01pat> GetT01patByMednum(int mednum)
        {
            var items = Context.T01pats
                              .AsNoTracking()
                              .Where(i => i.Mednum == mednum);

 
            OnGetT01patByMednum(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnT01patGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnT01patCreated(Medrec.Models.medrec.T01pat item);
        partial void OnAfterT01patCreated(Medrec.Models.medrec.T01pat item);

        public async Task<Medrec.Models.medrec.T01pat> CreateT01pat(Medrec.Models.medrec.T01pat t01pat)
        {
            OnT01patCreated(t01pat);

            var existingItem = Context.T01pats
                              .Where(i => i.Mednum == t01pat.Mednum)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.T01pats.Add(t01pat);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(t01pat).State = EntityState.Detached;
                throw;
            }

            OnAfterT01patCreated(t01pat);

            return t01pat;
        }

        public async Task<Medrec.Models.medrec.T01pat> CancelT01patChanges(Medrec.Models.medrec.T01pat item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnT01patUpdated(Medrec.Models.medrec.T01pat item);
        partial void OnAfterT01patUpdated(Medrec.Models.medrec.T01pat item);

        public async Task<Medrec.Models.medrec.T01pat> UpdateT01pat(int mednum, Medrec.Models.medrec.T01pat t01pat)
        {
            OnT01patUpdated(t01pat);

            var itemToUpdate = Context.T01pats
                              .Where(i => i.Mednum == t01pat.Mednum)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(t01pat);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterT01patUpdated(t01pat);

            return t01pat;
        }

        partial void OnT01patDeleted(Medrec.Models.medrec.T01pat item);
        partial void OnAfterT01patDeleted(Medrec.Models.medrec.T01pat item);

        public async Task<Medrec.Models.medrec.T01pat> DeleteT01pat(int mednum)
        {
            var itemToDelete = Context.T01pats
                              .Where(i => i.Mednum == mednum)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnT01patDeleted(itemToDelete);


            Context.T01pats.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterT01patDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportT01patidsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/medrec/t01patids/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/medrec/t01patids/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportT01patidsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/medrec/t01patids/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/medrec/t01patids/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnT01patidsRead(ref IQueryable<Medrec.Models.medrec.T01patid> items);

        public async Task<IQueryable<Medrec.Models.medrec.T01patid>> GetT01patids(Query query = null)
        {
            var items = Context.T01patids.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnT01patidsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnT01patidGet(Medrec.Models.medrec.T01patid item);
        partial void OnGetT01patidBySerial(ref IQueryable<Medrec.Models.medrec.T01patid> items);


        public async Task<Medrec.Models.medrec.T01patid> GetT01patidBySerial(int serial)
        {
            var items = Context.T01patids
                              .AsNoTracking()
                              .Where(i => i.Serial == serial);

 
            OnGetT01patidBySerial(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnT01patidGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnT01patidCreated(Medrec.Models.medrec.T01patid item);
        partial void OnAfterT01patidCreated(Medrec.Models.medrec.T01patid item);

        public async Task<Medrec.Models.medrec.T01patid> CreateT01patid(Medrec.Models.medrec.T01patid t01patid)
        {
            OnT01patidCreated(t01patid);

            var existingItem = Context.T01patids
                              .Where(i => i.Serial == t01patid.Serial)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.T01patids.Add(t01patid);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(t01patid).State = EntityState.Detached;
                throw;
            }

            OnAfterT01patidCreated(t01patid);

            return t01patid;
        }

        public async Task<Medrec.Models.medrec.T01patid> CancelT01patidChanges(Medrec.Models.medrec.T01patid item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnT01patidUpdated(Medrec.Models.medrec.T01patid item);
        partial void OnAfterT01patidUpdated(Medrec.Models.medrec.T01patid item);

        public async Task<Medrec.Models.medrec.T01patid> UpdateT01patid(int serial, Medrec.Models.medrec.T01patid t01patid)
        {
            OnT01patidUpdated(t01patid);

            var itemToUpdate = Context.T01patids
                              .Where(i => i.Serial == t01patid.Serial)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(t01patid);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterT01patidUpdated(t01patid);

            return t01patid;
        }

        partial void OnT01patidDeleted(Medrec.Models.medrec.T01patid item);
        partial void OnAfterT01patidDeleted(Medrec.Models.medrec.T01patid item);

        public async Task<Medrec.Models.medrec.T01patid> DeleteT01patid(int serial)
        {
            var itemToDelete = Context.T01patids
                              .Where(i => i.Serial == serial)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnT01patidDeleted(itemToDelete);


            Context.T01patids.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterT01patidDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportT03eventsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/medrec/t03events/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/medrec/t03events/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportT03eventsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/medrec/t03events/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/medrec/t03events/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnT03eventsRead(ref IQueryable<Medrec.Models.medrec.T03event> items);

        public async Task<IQueryable<Medrec.Models.medrec.T03event>> GetT03events(Query query = null)
        {
            var items = Context.T03events.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnT03eventsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnT03eventGet(Medrec.Models.medrec.T03event item);
        partial void OnGetT03eventBySerial(ref IQueryable<Medrec.Models.medrec.T03event> items);


        public async Task<Medrec.Models.medrec.T03event> GetT03eventBySerial(int serial)
        {
            var items = Context.T03events
                              .AsNoTracking()
                              .Where(i => i.Serial == serial);

 
            OnGetT03eventBySerial(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnT03eventGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnT03eventCreated(Medrec.Models.medrec.T03event item);
        partial void OnAfterT03eventCreated(Medrec.Models.medrec.T03event item);

        public async Task<Medrec.Models.medrec.T03event> CreateT03event(Medrec.Models.medrec.T03event t03event)
        {
            OnT03eventCreated(t03event);

            var existingItem = Context.T03events
                              .Where(i => i.Serial == t03event.Serial)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.T03events.Add(t03event);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(t03event).State = EntityState.Detached;
                throw;
            }

            OnAfterT03eventCreated(t03event);

            return t03event;
        }

        public async Task<Medrec.Models.medrec.T03event> CancelT03eventChanges(Medrec.Models.medrec.T03event item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnT03eventUpdated(Medrec.Models.medrec.T03event item);
        partial void OnAfterT03eventUpdated(Medrec.Models.medrec.T03event item);

        public async Task<Medrec.Models.medrec.T03event> UpdateT03event(int serial, Medrec.Models.medrec.T03event t03event)
        {
            OnT03eventUpdated(t03event);

            var itemToUpdate = Context.T03events
                              .Where(i => i.Serial == t03event.Serial)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(t03event);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterT03eventUpdated(t03event);

            return t03event;
        }

        partial void OnT03eventDeleted(Medrec.Models.medrec.T03event item);
        partial void OnAfterT03eventDeleted(Medrec.Models.medrec.T03event item);

        public async Task<Medrec.Models.medrec.T03event> DeleteT03event(int serial)
        {
            var itemToDelete = Context.T03events
                              .Where(i => i.Serial == serial)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnT03eventDeleted(itemToDelete);


            Context.T03events.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterT03eventDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportT02descrsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/medrec/t02descrs/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/medrec/t02descrs/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportT02descrsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/medrec/t02descrs/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/medrec/t02descrs/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnT02descrsRead(ref IQueryable<Medrec.Models.medrec.T02descr> items);

        public async Task<IQueryable<Medrec.Models.medrec.T02descr>> GetT02descrs(Query query = null)
        {
            var items = Context.T02descrs.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnT02descrsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnT02descrGet(Medrec.Models.medrec.T02descr item);
        partial void OnGetT02descrBySerial(ref IQueryable<Medrec.Models.medrec.T02descr> items);


        public async Task<Medrec.Models.medrec.T02descr> GetT02descrBySerial(int serial)
        {
            var items = Context.T02descrs
                              .AsNoTracking()
                              .Where(i => i.Serial == serial);

 
            OnGetT02descrBySerial(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnT02descrGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnT02descrCreated(Medrec.Models.medrec.T02descr item);
        partial void OnAfterT02descrCreated(Medrec.Models.medrec.T02descr item);

        public async Task<Medrec.Models.medrec.T02descr> CreateT02descr(Medrec.Models.medrec.T02descr t02descr)
        {
            OnT02descrCreated(t02descr);

            var existingItem = Context.T02descrs
                              .Where(i => i.Serial == t02descr.Serial)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.T02descrs.Add(t02descr);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(t02descr).State = EntityState.Detached;
                throw;
            }

            OnAfterT02descrCreated(t02descr);

            return t02descr;
        }

        public async Task<Medrec.Models.medrec.T02descr> CancelT02descrChanges(Medrec.Models.medrec.T02descr item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnT02descrUpdated(Medrec.Models.medrec.T02descr item);
        partial void OnAfterT02descrUpdated(Medrec.Models.medrec.T02descr item);

        public async Task<Medrec.Models.medrec.T02descr> UpdateT02descr(int serial, Medrec.Models.medrec.T02descr t02descr)
        {
            OnT02descrUpdated(t02descr);

            var itemToUpdate = Context.T02descrs
                              .Where(i => i.Serial == t02descr.Serial)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(t02descr);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterT02descrUpdated(t02descr);

            return t02descr;
        }

        partial void OnT02descrDeleted(Medrec.Models.medrec.T02descr item);
        partial void OnAfterT02descrDeleted(Medrec.Models.medrec.T02descr item);

        public async Task<Medrec.Models.medrec.T02descr> DeleteT02descr(int serial)
        {
            var itemToDelete = Context.T02descrs
                              .Where(i => i.Serial == serial)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnT02descrDeleted(itemToDelete);


            Context.T02descrs.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterT02descrDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportT02measuresToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/medrec/t02measures/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/medrec/t02measures/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportT02measuresToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/medrec/t02measures/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/medrec/t02measures/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnT02measuresRead(ref IQueryable<Medrec.Models.medrec.T02measure> items);

        public async Task<IQueryable<Medrec.Models.medrec.T02measure>> GetT02measures(Query query = null)
        {
            var items = Context.T02measures.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnT02measuresRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnT02measureGet(Medrec.Models.medrec.T02measure item);
        partial void OnGetT02measureBySerial(ref IQueryable<Medrec.Models.medrec.T02measure> items);


        public async Task<Medrec.Models.medrec.T02measure> GetT02measureBySerial(int serial)
        {
            var items = Context.T02measures
                              .AsNoTracking()
                              .Where(i => i.Serial == serial);

 
            OnGetT02measureBySerial(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnT02measureGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnT02measureCreated(Medrec.Models.medrec.T02measure item);
        partial void OnAfterT02measureCreated(Medrec.Models.medrec.T02measure item);

        public async Task<Medrec.Models.medrec.T02measure> CreateT02measure(Medrec.Models.medrec.T02measure t02measure)
        {
            OnT02measureCreated(t02measure);

            var existingItem = Context.T02measures
                              .Where(i => i.Serial == t02measure.Serial)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.T02measures.Add(t02measure);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(t02measure).State = EntityState.Detached;
                throw;
            }

            OnAfterT02measureCreated(t02measure);

            return t02measure;
        }

        public async Task<Medrec.Models.medrec.T02measure> CancelT02measureChanges(Medrec.Models.medrec.T02measure item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnT02measureUpdated(Medrec.Models.medrec.T02measure item);
        partial void OnAfterT02measureUpdated(Medrec.Models.medrec.T02measure item);

        public async Task<Medrec.Models.medrec.T02measure> UpdateT02measure(int serial, Medrec.Models.medrec.T02measure t02measure)
        {
            OnT02measureUpdated(t02measure);

            var itemToUpdate = Context.T02measures
                              .Where(i => i.Serial == t02measure.Serial)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(t02measure);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterT02measureUpdated(t02measure);

            return t02measure;
        }

        partial void OnT02measureDeleted(Medrec.Models.medrec.T02measure item);
        partial void OnAfterT02measureDeleted(Medrec.Models.medrec.T02measure item);

        public async Task<Medrec.Models.medrec.T02measure> DeleteT02measure(int serial)
        {
            var itemToDelete = Context.T02measures
                              .Where(i => i.Serial == serial)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnT02measureDeleted(itemToDelete);


            Context.T02measures.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterT02measureDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportT03measuresToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/medrec/t03measures/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/medrec/t03measures/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportT03measuresToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/medrec/t03measures/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/medrec/t03measures/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnT03measuresRead(ref IQueryable<Medrec.Models.medrec.T03measure> items);

        public async Task<IQueryable<Medrec.Models.medrec.T03measure>> GetT03measures(Query query = null)
        {
            var items = Context.T03measures.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnT03measuresRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnT03measureGet(Medrec.Models.medrec.T03measure item);
        partial void OnGetT03measureBySerial(ref IQueryable<Medrec.Models.medrec.T03measure> items);


        public async Task<Medrec.Models.medrec.T03measure> GetT03measureBySerial(int serial)
        {
            var items = Context.T03measures
                              .AsNoTracking()
                              .Where(i => i.Serial == serial);

 
            OnGetT03measureBySerial(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnT03measureGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnT03measureCreated(Medrec.Models.medrec.T03measure item);
        partial void OnAfterT03measureCreated(Medrec.Models.medrec.T03measure item);

        public async Task<Medrec.Models.medrec.T03measure> CreateT03measure(Medrec.Models.medrec.T03measure t03measure)
        {
            OnT03measureCreated(t03measure);

            var existingItem = Context.T03measures
                              .Where(i => i.Serial == t03measure.Serial)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.T03measures.Add(t03measure);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(t03measure).State = EntityState.Detached;
                throw;
            }

            OnAfterT03measureCreated(t03measure);

            return t03measure;
        }

        public async Task<Medrec.Models.medrec.T03measure> CancelT03measureChanges(Medrec.Models.medrec.T03measure item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnT03measureUpdated(Medrec.Models.medrec.T03measure item);
        partial void OnAfterT03measureUpdated(Medrec.Models.medrec.T03measure item);

        public async Task<Medrec.Models.medrec.T03measure> UpdateT03measure(int serial, Medrec.Models.medrec.T03measure t03measure)
        {
            OnT03measureUpdated(t03measure);

            var itemToUpdate = Context.T03measures
                              .Where(i => i.Serial == t03measure.Serial)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(t03measure);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterT03measureUpdated(t03measure);

            return t03measure;
        }

        partial void OnT03measureDeleted(Medrec.Models.medrec.T03measure item);
        partial void OnAfterT03measureDeleted(Medrec.Models.medrec.T03measure item);

        public async Task<Medrec.Models.medrec.T03measure> DeleteT03measure(int serial)
        {
            var itemToDelete = Context.T03measures
                              .Where(i => i.Serial == serial)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnT03measureDeleted(itemToDelete);


            Context.T03measures.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterT03measureDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportT90tab1SToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/medrec/t90tab1s/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/medrec/t90tab1s/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportT90tab1SToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/medrec/t90tab1s/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/medrec/t90tab1s/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnT90tab1SRead(ref IQueryable<Medrec.Models.medrec.T90tab1> items);

        public async Task<IQueryable<Medrec.Models.medrec.T90tab1>> GetT90tab1S(Query query = null)
        {
            var items = Context.T90tab1S.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnT90tab1SRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnT90tab1Get(Medrec.Models.medrec.T90tab1 item);
        partial void OnGetT90tab1BySerial(ref IQueryable<Medrec.Models.medrec.T90tab1> items);


        public async Task<Medrec.Models.medrec.T90tab1> GetT90tab1BySerial(int serial)
        {
            var items = Context.T90tab1S
                              .AsNoTracking()
                              .Where(i => i.Serial == serial);

 
            OnGetT90tab1BySerial(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnT90tab1Get(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnT90tab1Created(Medrec.Models.medrec.T90tab1 item);
        partial void OnAfterT90tab1Created(Medrec.Models.medrec.T90tab1 item);

        public async Task<Medrec.Models.medrec.T90tab1> CreateT90tab1(Medrec.Models.medrec.T90tab1 t90tab1)
        {
            OnT90tab1Created(t90tab1);

            var existingItem = Context.T90tab1S
                              .Where(i => i.Serial == t90tab1.Serial)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.T90tab1S.Add(t90tab1);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(t90tab1).State = EntityState.Detached;
                throw;
            }

            OnAfterT90tab1Created(t90tab1);

            return t90tab1;
        }

        public async Task<Medrec.Models.medrec.T90tab1> CancelT90tab1Changes(Medrec.Models.medrec.T90tab1 item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnT90tab1Updated(Medrec.Models.medrec.T90tab1 item);
        partial void OnAfterT90tab1Updated(Medrec.Models.medrec.T90tab1 item);

        public async Task<Medrec.Models.medrec.T90tab1> UpdateT90tab1(int serial, Medrec.Models.medrec.T90tab1 t90tab1)
        {
            OnT90tab1Updated(t90tab1);

            var itemToUpdate = Context.T90tab1S
                              .Where(i => i.Serial == t90tab1.Serial)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(t90tab1);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterT90tab1Updated(t90tab1);

            return t90tab1;
        }

        partial void OnT90tab1Deleted(Medrec.Models.medrec.T90tab1 item);
        partial void OnAfterT90tab1Deleted(Medrec.Models.medrec.T90tab1 item);

        public async Task<Medrec.Models.medrec.T90tab1> DeleteT90tab1(int serial)
        {
            var itemToDelete = Context.T90tab1S
                              .Where(i => i.Serial == serial)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnT90tab1Deleted(itemToDelete);


            Context.T90tab1S.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterT90tab1Deleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportVersionsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/medrec/versions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/medrec/versions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportVersionsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/medrec/versions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/medrec/versions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnVersionsRead(ref IQueryable<Medrec.Models.medrec.Version> items);

        public async Task<IQueryable<Medrec.Models.medrec.Version>> GetVersions(Query query = null)
        {
            var items = Context.Versions.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnVersionsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnVersionGet(Medrec.Models.medrec.Version item);
        partial void OnGetVersionByVersionnum(ref IQueryable<Medrec.Models.medrec.Version> items);


        public async Task<Medrec.Models.medrec.Version> GetVersionByVersionnum(int versionnum)
        {
            var items = Context.Versions
                              .AsNoTracking()
                              .Where(i => i.Versionnum == versionnum);

 
            OnGetVersionByVersionnum(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnVersionGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnVersionCreated(Medrec.Models.medrec.Version item);
        partial void OnAfterVersionCreated(Medrec.Models.medrec.Version item);

        public async Task<Medrec.Models.medrec.Version> CreateVersion(Medrec.Models.medrec.Version version)
        {
            OnVersionCreated(version);

            var existingItem = Context.Versions
                              .Where(i => i.Versionnum == version.Versionnum)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Versions.Add(version);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(version).State = EntityState.Detached;
                throw;
            }

            OnAfterVersionCreated(version);

            return version;
        }

        public async Task<Medrec.Models.medrec.Version> CancelVersionChanges(Medrec.Models.medrec.Version item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnVersionUpdated(Medrec.Models.medrec.Version item);
        partial void OnAfterVersionUpdated(Medrec.Models.medrec.Version item);

        public async Task<Medrec.Models.medrec.Version> UpdateVersion(int versionnum, Medrec.Models.medrec.Version version)
        {
            OnVersionUpdated(version);

            var itemToUpdate = Context.Versions
                              .Where(i => i.Versionnum == version.Versionnum)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(version);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterVersionUpdated(version);

            return version;
        }

        partial void OnVersionDeleted(Medrec.Models.medrec.Version item);
        partial void OnAfterVersionDeleted(Medrec.Models.medrec.Version item);

        public async Task<Medrec.Models.medrec.Version> DeleteVersion(int versionnum)
        {
            var itemToDelete = Context.Versions
                              .Where(i => i.Versionnum == versionnum)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnVersionDeleted(itemToDelete);


            Context.Versions.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterVersionDeleted(itemToDelete);

            return itemToDelete;
        }
        }
}