using BSSLTaskManagement.ViewModels;
using BSSLTaskManagement.ServicesInterfaces;
using Microsoft.EntityFrameworkCore;
using TaskManagement;
using TaskManagement.Models;

namespace BSSLTaskManagement.Services
{
    public class ConstituencyService : IConstituencyService
    {
        private readonly TaskDbContext _db;

        public ConstituencyService(TaskDbContext db)
        {
            _db = db;
        }

        // ════════════════════════════════════════════════════════════════════════
        // SHARED DROPDOWNS
        // ════════════════════════════════════════════════════════════════════════

        public async Task<List<CountryDropdownVm>> GetCountriesAsync()
        {
            return await _db.CountryTab
                .OrderBy(c => c.Description)
                .Select(c => new CountryDropdownVm
                {
                    Id          = c.Id,
                    //Code        = c.Code,
                    Description = c.Description
                })
                .ToListAsync();
        }

        public async Task<List<StateDropdownVm>> GetStatesByCountryAsync(int countryId)
        {
            return await _db.StateTab
                .Where(s => s.CountryTabId == countryId)
                .OrderBy(s => s.Description)
                .Select(s => new StateDropdownVm
                {
                    Id          = s.Id,
                    Code        = s.Code,
                    Description = s.Description
                })
                .ToListAsync();
        }

        // ════════════════════════════════════════════════════════════════════════
        // FEDERAL CONSTITUENCY
        // ════════════════════════════════════════════════════════════════════════

        public async Task<List<FederalConstituencyRowVm>> GetFederalConstituenciesAsync(int stateId)
        {
            return await _db.FederalConstituencyTab
                .Where(f => f.StateTabId == stateId)
                .OrderBy(f => f.Code)
                .Select(f => new FederalConstituencyRowVm
                {
                    Id          = f.Id,
                    //Code        = f.Code,
                    Description = f.Description
                })
                .ToListAsync();
        }

        public async Task<ConstituencyApiResponse> SaveFederalConstituenciesAsync(SaveFederalConstituencyRequest request)
        {
            // ── 1. Basic input validation ────────────────────────────────────────
            if (request.StateId <= 0)
                return Fail("A valid state must be selected before saving.");

            if (request.Rows == null || request.Rows.Count == 0)
                return Fail("No rows were submitted.");

            var emptyRow = request.Rows.FirstOrDefault(r =>
                string.IsNullOrWhiteSpace(r.Code) || string.IsNullOrWhiteSpace(r.Description));

            if (emptyRow != null)
                return Fail("Every row must have both a Code and a Description.");

            // ── 2. Duplicate check within the submitted batch ────────────────────
            // Codes must be unique — two rows in the same save cannot share a code.
            var duplicateInBatch = request.Rows
                .GroupBy(r => r.Code.Trim().ToUpper())
                .FirstOrDefault(g => g.Count() > 1);

            if (duplicateInBatch != null)
                return Fail($"Duplicate code '{duplicateInBatch.Key}' found in the submitted rows. Each code must be unique.");

            // ── 3. Duplicate check against existing DB rows ─────────────────────
            // A code already saved under this state cannot be used by a *different* Id.
            var existingRows = await _db.FederalConstituencyTab
                .Where(f => f.StateTabId == request.StateId)
                .ToListAsync();

            foreach (var row in request.Rows)
            {
                var conflict = existingRows.FirstOrDefault(e =>
                    e.Code.Trim().ToUpper() == row.Code.Trim().ToUpper() &&
                    e.Id != row.Id);  // same code, different record → true duplicate

                if (conflict != null)
                    return Fail($"Code '{row.Code}' already exists for this state. Codes must be unique.");
            }

            // ── 4. Insert or update ──────────────────────────────────────────────
            foreach (var row in request.Rows)
            {
                if (row.Id == 0)
                {
                    // New row — insert
                    _db.FederalConstituencyTab.Add(new FederalConstituencyTab
                    {
                        StateTabId  = request.StateId,
                        Code        = row.Code.Trim(),
                        Description = row.Description.Trim()
                    });
                }
                else
                {
                    // Existing row — update
                    var entity = existingRows.FirstOrDefault(e => e.Id == row.Id);
                    if (entity != null)
                    {
                        entity.Code        = row.Code.Trim();
                        entity.Description = row.Description.Trim();
                    }
                }
            }

            await _db.SaveChangesAsync();
            return Ok("Federal constituency records saved successfully.");
        }

        public async Task<ConstituencyApiResponse> DeleteFederalConstituencyAsync(int id)
        {
            var entity = await _db.FederalConstituencyTab.FindAsync(id);
            if (entity == null)
                return Fail("Record not found.");

            _db.FederalConstituencyTab.Remove(entity);
            await _db.SaveChangesAsync();
            return Ok("Record deleted successfully.");
        }

        // ════════════════════════════════════════════════════════════════════════
        // SENATORIAL DISTRICT
        // ════════════════════════════════════════════════════════════════════════

        public async Task<List<SenatorialDistrictRowVm>> GetSenatorialDistrictsAsync(int stateId)
        {
            return await _db.SenatorialDistrictTab
                .Where(s => s.StateTabId == stateId)
                .OrderBy(s => s.Code)
                .Select(s => new SenatorialDistrictRowVm
                {
                    Id          = s.Id,
                    Code        = s.Code,
                    Description = s.Description
                })
                .ToListAsync();
        }

        public async Task<ConstituencyApiResponse> SaveSenatorialDistrictsAsync(SaveSenatorialDistrictRequest request)
        {
            if (request.StateId <= 0)
                return Fail("A valid state must be selected before saving.");

            if (request.Rows == null || request.Rows.Count == 0)
                return Fail("No rows were submitted.");

            var emptyRow = request.Rows.FirstOrDefault(r =>
                string.IsNullOrWhiteSpace(r.Code) || string.IsNullOrWhiteSpace(r.Description));
            if (emptyRow != null)
                return Fail("Every row must have both a Code and a Description.");

            var duplicateInBatch = request.Rows
                .GroupBy(r => r.Code.Trim().ToUpper())
                .FirstOrDefault(g => g.Count() > 1);
            if (duplicateInBatch != null)
                return Fail($"Duplicate code '{duplicateInBatch.Key}' found in the submitted rows.");

            var existingRows = await _db.SenatorialDistrictTab
                .Where(s => s.StateTabId == request.StateId)
                .ToListAsync();

            foreach (var row in request.Rows)
            {
                var conflict = existingRows.FirstOrDefault(e =>
                    e.Code.Trim().ToUpper() == row.Code.Trim().ToUpper() && e.Id != row.Id);
                if (conflict != null)
                    return Fail($"Code '{row.Code}' already exists for this state.");
            }

            foreach (var row in request.Rows)
            {
                if (row.Id == 0)
                {
                    _db.SenatorialDistrictTab.Add(new SenatorialDistrictTab
                    {
                        StateTabId  = request.StateId,
                        Code        = row.Code.Trim(),
                        Description = row.Description.Trim()
                    });
                }
                else
                {
                    var entity = existingRows.FirstOrDefault(e => e.Id == row.Id);
                    if (entity != null)
                    {
                        entity.Code        = row.Code.Trim();
                        entity.Description = row.Description.Trim();
                    }
                }
            }

            await _db.SaveChangesAsync();
            return Ok("Senatorial district records saved successfully.");
        }

        public async Task<ConstituencyApiResponse> DeleteSenatorialDistrictAsync(int id)
        {
            var entity = await _db.SenatorialDistrictTab.FindAsync(id);
            if (entity == null) return Fail("Record not found.");
            _db.SenatorialDistrictTab.Remove(entity);
            await _db.SaveChangesAsync();
            return Ok("Record deleted successfully.");
        }

        // ════════════════════════════════════════════════════════════════════════
        // STATE CONSTITUENCY
        // ════════════════════════════════════════════════════════════════════════

        public async Task<List<StateConstituencyRowVm>> GetStateConstituenciesAsync(int stateId)
        {
            return await _db.StateConstituencyTab
                .Where(s => s.StateTabId == stateId)
                .OrderBy(s => s.Code)
                .Select(s => new StateConstituencyRowVm
                {
                    Id          = s.Id,
                    Code        = s.Code,
                    Description = s.Description
                })
                .ToListAsync();
        }

        public async Task<ConstituencyApiResponse> SaveStateConstituenciesAsync(SaveStateConstituencyRequest request)
        {
            if (request.StateId <= 0)
                return Fail("A valid state must be selected before saving.");

            if (request.Rows == null || request.Rows.Count == 0)
                return Fail("No rows were submitted.");

            var emptyRow = request.Rows.FirstOrDefault(r =>
                string.IsNullOrWhiteSpace(r.Code) || string.IsNullOrWhiteSpace(r.Description));
            if (emptyRow != null)
                return Fail("Every row must have both a Code and a Description.");

            var duplicateInBatch = request.Rows
                .GroupBy(r => r.Code.Trim().ToUpper())
                .FirstOrDefault(g => g.Count() > 1);
            if (duplicateInBatch != null)
                return Fail($"Duplicate code '{duplicateInBatch.Key}' found in the submitted rows.");

            var existingRows = await _db.StateConstituencyTab
                .Where(s => s.StateTabId == request.StateId)
                .ToListAsync();

            foreach (var row in request.Rows)
            {
                var conflict = existingRows.FirstOrDefault(e =>
                    e.Code.Trim().ToUpper() == row.Code.Trim().ToUpper() && e.Id != row.Id);
                if (conflict != null)
                    return Fail($"Code '{row.Code}' already exists for this state.");
            }

            foreach (var row in request.Rows)
            {
                if (row.Id == 0)
                {
                    _db.StateConstituencyTab.Add(new StateConstituencyTab
                    {
                        StateTabId  = request.StateId,
                        Code        = row.Code.Trim(),
                        Description = row.Description.Trim()
                    });
                }
                else
                {
                    var entity = existingRows.FirstOrDefault(e => e.Id == row.Id);
                    if (entity != null)
                    {
                        entity.Code        = row.Code.Trim();
                        entity.Description = row.Description.Trim();
                    }
                }
            }

            await _db.SaveChangesAsync();
            return Ok("State constituency records saved successfully.");
        }

        public async Task<ConstituencyApiResponse> DeleteStateConstituencyAsync(int id)
        {
            var entity = await _db.StateConstituencyTab.FindAsync(id);
            if (entity == null) return Fail("Record not found.");
            _db.StateConstituencyTab.Remove(entity);
            await _db.SaveChangesAsync();
            return Ok("Record deleted successfully.");
        }

        // ════════════════════════════════════════════════════════════════════════
        // PRIVATE HELPERS
        // ════════════════════════════════════════════════════════════════════════

        private static ConstituencyApiResponse Ok(string message) =>
            new() { Success = true, Message = message };

        private static ConstituencyApiResponse Fail(string message) =>
            new() { Success = false, Message = message };
    }
}
