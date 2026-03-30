using BSSLTaskManagement.ServicesInterfaces;
using BSSLTaskManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BSSLTaskManagement.Pages.Constituency
{
    public class StateModel : PageModel
    {
        private readonly IConstituencyService _constituencyService;

        public StateModel(IConstituencyService constituencyService)
        {
            _constituencyService = constituencyService;
        }

        // ── Page Load ────────────────────────────────────────────────────────────
        public void OnGet()
        {
            // Dropdowns are populated via AJAX — nothing needed here on initial load.
        }

        // ── AJAX: Get all countries for the Country dropdown ─────────────────────
        public async Task<IActionResult> OnGetCountriesAsync()
        {
            var countries = await _constituencyService.GetCountriesAsync();
            return new JsonResult(countries);
        }

        // ── AJAX: Get states filtered by the selected country ────────────────────
        public async Task<IActionResult> OnGetStatesByCountryAsync(int countryId)
        {
            if (countryId <= 0)
                return new JsonResult(new List<StateDropdownVm>());

            var states = await _constituencyService.GetStatesByCountryAsync(countryId);
            return new JsonResult(states);
        }

        // ── AJAX: Load existing federal constituency rows for the selected state ──
        public async Task<IActionResult> OnGetStateRowsAsync(int stateId)
        {
            if (stateId <= 0)
                return new JsonResult(new List<StateDropdownVm>());

            var rows = await _constituencyService.GetStateConstituenciesAsync(stateId);
            return new JsonResult(rows);
        }

        // ── AJAX: Save all rows (insert new + update existing) ───────────────────
        public async Task<IActionResult> OnPostSaveStateAsync([FromBody] SaveStateConstituencyRequest request)
        {
            if (request == null)
                return new JsonResult(new ConstituencyApiResponse
                {
                    Success = false,
                    Message = "Invalid request payload."
                });

            var result = await _constituencyService.SaveStateConstituenciesAsync(request);
            return new JsonResult(result);
        }

        // ── AJAX: Delete a single row by Id ─────────────────────────────────────
        public async Task<IActionResult> OnPostDeleteStateAsync(int id)
        {
            if (id <= 0)
                return new JsonResult(new ConstituencyApiResponse
                {
                    Success = false,
                    Message = "Invalid record ID."
                });

            var result = await _constituencyService.DeleteStateConstituencyAsync(id);
            return new JsonResult(result);
        }
    }
}