using System;
using System.Collections.Generic;
using System.Linq;

namespace Taadol.Helpers
{
    public static class ValidationHelper
    {
        public static bool IsValidNationalCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return false;
            code = code.Trim().Replace(" ", "").Replace("-", "");

            var digits = new System.Text.StringBuilder();
            foreach (char c in code)
            {
                if (c >= '۰' && c <= '۹') digits.Append((char)('0' + (c - '۰')));
                else digits.Append(c);
            }
            code = digits.ToString();

            if (code.Length != 10 || !code.All(char.IsDigit)) return false;
            if (code.Distinct().Count() == 1) return false;

            int[] weights = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int sum = 0;
            for (int i = 0; i < 9; i++)
                sum += (code[i] - '0') * weights[i];

            int remainder = sum % 11;
            int checkDigit = remainder < 2 ? remainder : 11 - remainder;
            return checkDigit == (code[9] - '0');
        }

        public static bool IsValidShaba(string shaba)
        {
            if (string.IsNullOrWhiteSpace(shaba)) return false;
            shaba = shaba.Trim().Replace(" ", "").ToUpper();
            return shaba.StartsWith("IR") && shaba.Length == 26 && shaba.Substring(2).All(char.IsDigit);
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                var addr = new System.Net.Mail.MailAddress(email.Trim());
                return addr.Address == email.Trim();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// اعتبارسنجی شماره موبایل (دقیقاً 11 رقم، شروع با 09)
        /// </summary>
        public static bool IsValidMobile(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile)) return false;
            var digits = mobile.Trim().Replace(" ", "").Replace("-", "");
            return digits.Length == 11
                   && digits.StartsWith("09")
                   && digits.All(char.IsDigit);
        }

        /// <summary>
        /// اعتبارسنجی شماره تلفن ثابت (8 تا 11 رقم)
        /// </summary>
        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;
            var digits = phone.Trim().Replace(" ", "").Replace("-", "");
            return digits.Length >= 8
                   && digits.Length <= 11
                   && digits.All(char.IsDigit);
        }

        /// <summary>
        /// اعتبارسنجی کد پستی (دقیقاً ۱۰ رقم عددی)
        /// </summary>
        public static bool IsValidPostalCode(string postalCode)
        {
            if (string.IsNullOrWhiteSpace(postalCode)) return false;
            var digits = postalCode.Trim().Replace(" ", "").Replace("-", "");
            return digits.Length == 10 && digits.All(char.IsDigit);
        }
    }
}