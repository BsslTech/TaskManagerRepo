using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManagement.Models
{
        public class CountryTab
        {
            public int Id { get; set; }
            public string Code { get; set; }
            public string Description { get; set; }
        }
        public class StateTab
        {
            public int Id { get; set; }
            public int CountryTabId { get; set; }
            public string Code { get; set; }
            public string Description { get; set; }
            public CountryTab CountryTab { get; set; }
        }
        public class LgaTab
        {
            public int Id { get; set; }
            public int StateTabId { get; set; }
            public string Code { get; set; }
            public string Description { get; set; }
            public StateTab StateTab { get; set; }
        }
}
