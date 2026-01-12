using System.Collections.Generic;
using Web.Models;

namespace UnitTest.Fakes
{
    public static class FakeData
    {
        public static List<SelectOption> GetFakeSchoolOptions()
        {
            return new List<SelectOption>
            {
                new SelectOption { Value = "1", Text = "Escola Test 1" },
                new SelectOption { Value = "2", Text = "Escola Test 2" }
            };
        }
    }
}
