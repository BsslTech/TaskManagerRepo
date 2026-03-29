using BSSLTaskManagement.ViewModels;

namespace BSSLTaskManagement.ServicesInterfaces
{
    /// <summary>
    /// Single service interface covering shared dropdown lookups
    /// plus separate CRUD operations for each of the three constituency pages.
    ///
    /// ─ Federal Constituency page   → GetFederalConstituencies / SaveFederalConstituencies / DeleteFederalConstituency
    /// ─ Senatorial District page    → GetSenatorialDistricts   / SaveSenatorialDistricts   / DeleteSenatorialDistrict
    /// ─ State Constituency page     → GetStateConstituencies   / SaveStateConstituencies   / DeleteStateConstituency
    /// </summary>
    public interface IConstituencyService
    {
        // ── Shared dropdowns (called by ALL three pages) ─────────────────────────

        /// <summary>Returns all countries for the Country dropdown.</summary>
        Task<List<CountryDropdownVm>> GetCountriesAsync();

        /// <summary>
        /// Returns states that belong to <paramref name="countryId"/>.
        /// Used to cascade-filter the State dropdown after a country is selected.
        /// </summary>
        Task<List<StateDropdownVm>> GetStatesByCountryAsync(int countryId);

        // ── Federal Constituency ─────────────────────────────────────────────────

        /// <summary>
        /// Loads all existing federal constituency rows for the given state.
        /// Returns an empty list (not null) when none exist yet.
        /// </summary>
        Task<List<FederalConstituencyRowVm>> GetFederalConstituenciesAsync(int stateId);

        /// <summary>
        /// Saves (insert or update) a batch of federal constituency rows for a state.
        ///
        /// Business rules enforced here:
        ///   • Codes must be unique within the state — duplicate codes cause failure.
        ///   • Rows with Id = 0 are inserted; rows with Id > 0 are updated.
        ///   • Empty Code or Description is rejected.
        ///
        /// Returns a response indicating success or a descriptive error message.
        /// </summary>
        Task<ConstituencyApiResponse> SaveFederalConstituenciesAsync(SaveFederalConstituencyRequest request);

        /// <summary>Deletes a single federal constituency row by its Id.</summary>
        Task<ConstituencyApiResponse> DeleteFederalConstituencyAsync(int id);

        // ── Senatorial District ──────────────────────────────────────────────────

        /// <summary>Loads all senatorial district rows for the given state.</summary>
        Task<List<SenatorialDistrictRowVm>> GetSenatorialDistrictsAsync(int stateId);

        /// <summary>
        /// Saves (insert or update) senatorial district rows for a state.
        /// Same duplicate-code and validation rules as Federal.
        /// </summary>
        Task<ConstituencyApiResponse> SaveSenatorialDistrictsAsync(SaveSenatorialDistrictRequest request);

        /// <summary>Deletes a single senatorial district row by its Id.</summary>
        Task<ConstituencyApiResponse> DeleteSenatorialDistrictAsync(int id);

        // ── State Constituency ───────────────────────────────────────────────────

        /// <summary>Loads all state constituency rows for the given state.</summary>
        Task<List<StateConstituencyRowVm>> GetStateConstituenciesAsync(int stateId);

        /// <summary>
        /// Saves (insert or update) state constituency rows for a state.
        /// Same duplicate-code and validation rules as Federal.
        /// </summary>
        Task<ConstituencyApiResponse> SaveStateConstituenciesAsync(SaveStateConstituencyRequest request);

        /// <summary>Deletes a single state constituency row by its Id.</summary>
        Task<ConstituencyApiResponse> DeleteStateConstituencyAsync(int id);
    }
}
