//#nullable disable
namespace BSSLTaskManagement.ViewModels
{
    // ─── Shared dropdown view models (used by all three pages) ──────────────────

    /// <summary>Represents a single item in the Country dropdown.</summary>
    public class CountryDropdownVm
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>Represents a single item in the State dropdown (filtered by country).</summary>
    public class StateDropdownVm
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    // ─── Federal Constituency ────────────────────────────────────────────────────

    /// <summary>
    /// One row in the inline-editable Federal Constituency table.
    /// Id = 0 means it is a new (unsaved) row.
    /// </summary>
    public class FederalConstituencyRowVm
    {
        public int Id { get; set; }          // 0 for new rows
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// Full page model sent to / received from the Federal Constituency page.
    /// </summary>
    public class FederalConstituencyPageVm
    {
        public int SelectedCountryId { get; set; }
        public int SelectedStateId { get; set; }
        public List<FederalConstituencyRowVm> Rows { get; set; } = new();
    }

    /// <summary>
    /// The payload the page POSTs when saving rows.
    /// </summary>
    public class SaveFederalConstituencyRequest
    {
        public int StateId { get; set; }
        public List<FederalConstituencyRowVm> Rows { get; set; } = new();
    }

    /// <summary>Standard response wrapper returned from all AJAX handlers.</summary>
    public class ConstituencyApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Optional payload — the caller casts this to whatever type it expects.
        /// </summary>
        public object? Data { get; set; }
    }

    // ─── Senatorial District (same shape — wired up when that page is built) ──

    public class SenatorialDistrictRowVm
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class SaveSenatorialDistrictRequest
    {
        public int StateId { get; set; }
        public List<SenatorialDistrictRowVm> Rows { get; set; } = new();
    }

    // ─── State Constituency (same shape — wired up when that page is built) ────

    public class StateConstituencyRowVm
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class SaveStateConstituencyRequest
    {
        public int StateId { get; set; }
        public List<StateConstituencyRowVm> Rows { get; set; } = new();
    }
}



