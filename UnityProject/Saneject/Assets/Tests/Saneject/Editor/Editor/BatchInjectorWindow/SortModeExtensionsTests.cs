using System.Linq;
using NUnit.Framework;
using Plugins.Saneject.Editor.EditorWindows.BatchInjector.Enums;
using Plugins.Saneject.Editor.EditorWindows.BatchInjector.Extensions;

namespace Tests.Saneject.Editor.Editor.BatchInjectorWindow
{
    public class SortModeExtensionsTests
    {
        [Test]
        public void GetDisplayString_GivenAllSortModes_ReturnsExpectedDisplayStrings()
        {
            // Get display strings
            string[] displayStrings = new[]
            {
                SortMode.NameAtoZ,
                SortMode.NameZtoA,
                SortMode.PathAtoZ,
                SortMode.PathZtoA,
                SortMode.EnabledToDisabled,
                SortMode.DisabledToEnabled,
                SortMode.StatusSuccessToError,
                SortMode.StatusErrorToSuccess,
                SortMode.Custom
            }.Select(mode => mode.GetDisplayString()).ToArray();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                "Name A-Z",
                "Name Z-A",
                "Path A-Z",
                "Path Z-A",
                "Enabled-Disabled",
                "Disabled-Enabled",
                "Success-Error",
                "Error-Success",
                "Custom"
            }, displayStrings);
        }
    }
}
