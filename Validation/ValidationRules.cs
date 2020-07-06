using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace AccoutingDocs.Validation
{
    public class ValidationRules : ValidationRule
    {
        /// <summary>
        /// Валидация данных
        /// </summary>
        /// <param name="value">Значение</param>
        /// <param name="ci"></param>
        /// <returns>Результат валидации</returns>
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo ci)
        {
            int result = 0;
            if(int.TryParse((string)value, out result))
            {
                return new ValidationResult(false, "Нельзя использовать только числовые значения");
            }
            if ((string)value == "")
            {
                return new ValidationResult(false, "Данное поле обязательно");
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }
    }
}
