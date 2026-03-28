using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagement.Models
{
    

   

    // ─── NEW: Federal Constituency ───────────────────────────────────────────────
    /// <summary>
    /// Stores federal constituency codes scoped to a state.
    /// Codes must be unique within a state (enforced at the service layer).
    /// </summary>
    public class FederalConstituencyTab
    {
        public int Id { get; set; }

        /// <summary>Links this constituency to the state it belongs to.</summary>
        public int StateTabId { get; set; }

        [Required]
        public string Code { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [ForeignKey(nameof(StateTabId))]
        public StateTab? StateTab { get; set; }
    }

    // ─── Senatorial District (same shape — ready for the next page) ───────────
    /// <summary>
    /// Stores senatorial district codes scoped to a state.
    /// Follows the same pattern as FederalConstituencyTab.
    /// </summary>
    public class SenatorialDistrictTab
    {
        public int Id { get; set; }

        public int StateTabId { get; set; }

        [Required]
        public string Code { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [ForeignKey(nameof(StateTabId))]
        public StateTab? StateTab { get; set; }
    }

    // ─── State Constituency (same shape — ready for the third page) ───────────
    /// <summary>
    /// Stores state constituency codes scoped to a state.
    /// Follows the same pattern as FederalConstituencyTab.
    /// </summary>
    public class StateConstituencyTab
    {
        public int Id { get; set; }

        public int StateTabId { get; set; }

        [Required]
        public string Code { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [ForeignKey(nameof(StateTabId))]
        public StateTab? StateTab { get; set; }
    }
}