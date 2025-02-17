using AnimeDB.Database;
using AnimeDB.Database.Tables;
using AnimeDB.UserInterface.MenuOptions;
using AnimeDB.UserInterface.prompts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AnimeDB.UserInterface
{
    /// <summary>
    /// Largely attributed to El chato because of things I didnt even know existed,
    /// 
    /// Builds menus for table templates for quick UI building
    /// </summary>
    /// <typeparam name="T">The table template</typeparam>
    public class MenuBuilder<T> where T : class, new()
    {
        private readonly T _instance;
        private readonly Dictionary<string, object> _modifiedValues = new();
        private Root root;

        public MenuBuilder(Root root, T?instance = null)
        {
            _instance = instance ?? new T();
            this.root = root;
        }

        /// <summary>
        /// Builds a menu
        /// </summary>
        /// <param name="onSave">When the save button is pressed</param>
        /// <param name="onCancel">When the operation is canceled</param>
        /// <returns></returns>
        public NestedMenu BuildMenu(Action<T> onSave, Action onCancel)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var options = new List<MenuOption>();

            foreach (var prop in properties)
            {
                if (prop.Name.ToLower() == "id") continue;

                object value = prop.GetValue(_instance) ?? "";
                string valueString = value.ToString();

                if (prop.PropertyType == typeof(bool))
                {
                    bool currentValue = (bool)value;
                    options.Add(new ToggleOption(
                        currentValue,
                        prop.Name,
                        $"Toggle {prop.Name} on/off",
                        () => _modifiedValues[prop.Name] = true,
                        () => _modifiedValues[prop.Name] = false
                    ));
                }
                else options.Add(new TextOption(
                    prop.Name, 
                    valueString,
                    valueString,
                    (input, _) => _modifiedValues[prop.Name] = ConvertValue(prop, input)
                ));
            }

            options.Add(new LambdaMenuOption("Save", "", (_) =>
            {
                ApplyChanges();
                var table = _instance as Table;
                if (table != null) new TableBuilder<T>(table.GetName(), root).Save(_instance);
                onSave(_instance);
            }));

            options.Add(new LambdaMenuOption("Cancel", "", (_) =>
            {
                onCancel();
                root.ClosePrompt();
            }));

            return new NestedMenu(options, 0);
        }
        // El chato
        private object ConvertValue(PropertyInfo prop, string input)
        {
            if (prop.PropertyType == typeof(int) && int.TryParse(input, out int intValue))
                return intValue;

            if (prop.PropertyType == typeof(float) && float.TryParse(input, out float floatValue))
                return floatValue;

            if (prop.PropertyType == typeof(DateTime))
                return ParseDatetime(input) ?? prop.GetValue(_instance);

            return input;
        }
        // El chato
        private DateTime? ParseDatetime(string dateString)
        {
            string[] formats = { "dd-MM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy HH:mm:ss" };
            if (DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
                return dateTime;
            return null;
        }
        // El chato
        private void ApplyChanges()
        {
            foreach (var entry in _modifiedValues)
            {
                try
                {
                    var prop = typeof(T).GetProperty(entry.Key);
                    if (prop != null && prop.CanWrite)
                        prop.SetValue(_instance, entry.Value);
                }
                catch(Exception ex)
                {
                    root.OpenPrompt(new ErrorPrompt("Failed to save: " + ex.Message));
                }
            }
        }
    }
}
