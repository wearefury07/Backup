using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;


public static class InputFieldExtend

{
    private static Regex regexUserName = new Regex(@"^(?=[a-zA-Z0-9])[-\w.]{0,23}([a-zA-Z0-9\d]|(?<![-.])_)$", RegexOptions.IgnoreCase);
    private static Regex regexDisplayName = new Regex(@"^[\p{L}\p{M}' \.\-]+$", RegexOptions.IgnoreCase);

    public static string ValidateEmpty(this InputField inputField, string name, bool autoFocus)
    {
        if (string.IsNullOrEmpty(inputField.text))
        {
            if (autoFocus)
                FocusInputField(inputField);
            return name + " không được để trống";
        }
        return "";
    }

    public static string ValidateLength(this InputField inputField, string name, int minLength, int maxLength, bool autoFocus)
    {
        if (inputField.text.Length < minLength || inputField.text.Length > maxLength)
        {
            if (autoFocus)
                FocusInputField(inputField);
            return name + " chứa từ " + minLength + " - " + maxLength + " kí tự";
        }
        return "";
    }

    public static string ValidateNumb(this InputField inputField, string name, bool autoFocus)
    {
        int numb = 0;
        if (!int.TryParse(inputField.text, out numb))
        {
            if (autoFocus)
                FocusInputField(inputField);
            return name + " phải là dạng số";
        }
        return "";
    }

    public static string ValidateUserName(this InputField inputField, string name, bool autoFocus)
    {
        if (!regexUserName.IsMatch(inputField.text))
        {
            if (autoFocus)
                FocusInputField(inputField);
            return name + " chỉ gồm chữ cái hoặc số";
        }

        if (inputField.text.ToLower().Contains("admin"))
        {
            if (autoFocus)
                FocusInputField(inputField);
            return name + " tên tài khoản không hợp lệ";
        }
        return "";
    }

    public static string ValidateDisplayName(this InputField inputField, string name, bool autoFocus)
    {
        if (!regexDisplayName.IsMatch(inputField.text))
        {
            if (autoFocus)
                FocusInputField(inputField);
            return name + " không được chứa ký hiệu đặc biệt";
        }

        else if (inputField.text.ToLower().Contains("admin"))
        {
            if (autoFocus)
                FocusInputField(inputField);
            return name + " tên tài khoản không hợp lệ";
        }
        return "";
    }

    public static void FocusInputField(this InputField inputField)
    {
        if (!inputField.isFocused)
        {
            EventSystem.current.SetSelectedGameObject(inputField.gameObject, null);
            inputField.OnPointerClick(new PointerEventData(EventSystem.current));
        }
    }
}
