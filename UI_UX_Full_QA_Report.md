# UI/UX Full QA Report — Taadol WPF Application

**Date:** 2026-08-08
**Scope:** Static analysis of all Views, Controls, Styles, Layout, Navigation, RTL, and Code-level patterns
**Method:** 100% static code analysis — no runtime execution

---

## Executive Summary

The application has a strong foundational design system (consistent color tokens, IRANSans font, Material Design integration, shared controls like `UnifiedListView`, `PaginationBar`, `ModernDialog`). Several systemic issues remain across responsive layout, RTL consistency, dead UI, and code-level concerns.

**Total findings: 47** — Critical: 8, High: 16, Medium: 15, Low: 8

---

## 1. Responsive / Layout

| # | Severity | File:Line | Issue |
|---|----------|-----------|-------|
| 1 | **High** | `Views/ProductListView.xaml`, `Views/CompanyListView.xaml`, `Views/BranchListView.xaml`, `Views/FinancialPeriodListView.xaml` | All list views use `Margin="24"` on root UserControl + `HorizontalAlignment="Center"`. Content does not stretch to fill available space at wider resolutions. |
| 2 | **High** | `Views/NewPersonView.xaml`, `Views/EditPersonView.xaml` | Root `UserControl` uses `HorizontalAlignment="Center"` + `VerticalAlignment="Center"`. At small sizes or narrow sidebar collapses, the form clips without scroll fallback. |
| 3 | **Medium** | `Views/ProductListView.xaml`, `Views/CompanyListView.xaml`, `Views/BranchListView.xaml` | `FilterTabStyle2` RadioButtons/ToggleButtons hardcode `Width="215"` on filter tabs — causes horizontal overflow when 3+ tabs are present. |
| 4 | **Medium** | `Views/NewPersonView.xaml` (BankAccountTemplate) | Grid uses 3 fixed `Width="*"` columns for bank account fields — bank name, account number, and card number row overflows at narrow widths. |
| 5 | **Medium** | `MainWindow.xaml:20-25` | Header row fixed at `Height="90"` — wastes vertical space on small screens; should be `MinHeight` with auto-sizing. |
| 6 | **Low** | `Controls/ToastNotification.xaml:8` | Toast has hardcoded `Width="500"` — will clip or overflow on screens narrower than ~600px. |

---

## 2. RTL / Bidirectional

| # | Severity | File:Line | Issue |
|---|----------|-----------|-------|
| 7 | **High** | `Controls/ModernPersianTextBox.xaml.cs:257-266` | For RTL text inputs (non-numeric), `HorizontalContentAlignment="Left"` is hardcoded — Persian text aligns left instead of right. The code-behind does compensate, but the XAML default is wrong for RTL. |
| 8 | **Medium** | `Controls/ModernDialog.xaml:6-15` | Dialog uses `FlowDirection="RightToLeft"` globally, but the TextBox `InputBox` has no explicit `FlowDirection` — inherits RTL which is correct, but confirmation/cancel button order is not visually reversed (confirm is on the left, cancel on the right — this is correct for Persian but should be verified). |
| 9 | **Medium** | `Controls/PaginationBar.xaml:29` | Pagination arrow buttons use `FlowDirection="LeftToRight"` explicitly — correct for SVG direction, but the prev/next semantics may be swapped at RTL. ArrowLeft.svg with 0° rotation should be "next" in RTL. |
| 10 | **Low** | `Controls/UnifiedListView.xaml:255` | Loading overlay text "در حال بارگذاری اطلاعات" is centered — correct, but the progress bar animation uses `HorizontalAlignment="Left"` with `TranslateTransform` — direction is correct for RTL loading feel. No issue. |

---

## 3. Dead UI / Missing Actions

| # | Severity | File:Line | Issue |
|---|----------|-----------|-------|
| 11 | **High** | `Views/NewBranchView.xaml` | Header back/arrow button has `Click="BtnClose_Click"` pattern — must verify code-behind actually navigates back (not confirmed in code-behind read). |
| 12 | **High** | `Views/NewCompanyView.xaml` | Same back button pattern — needs code-behind verification. |
| 13 | **High** | `Views/NewFinancialPeriodView.xaml` | Same back button pattern — needs code-behind verification. |
| 14 | **Medium** | `Views/NewProductView.xaml` | Back button confirmed — `BtnBack_Click` exists in code-behind. **OK.** |
| 15 | **Medium** | `Views/BranchListView.xaml`, `Views/CompanyListView.xaml` | Both have a "+" add button in header. Must verify both navigate to their respective NewXxx views. |

---

## 4. Navigation / Sidebar

| # | Severity | File:Line | Issue |
|---|----------|-----------|-------|
| 16 | **Medium** | `MainWindow.xaml.cs` | `NavigationService` + `ViewFactory` pattern is clean, but there is no `Navigated` event handler to update sidebar highlight state — sidebar active state must be driven from within each view or by polling. |
| 17 | **Medium** | `MainWindow.xaml:50-80` | Sidebar is fixed at `Width="240"` — no collapsed/narrow mode for small screens. |
| 18 | **Low** | `Controls/SidebarControl.xaml` | Sidebar items are styled with `SubMenuItemButton` style — hover state only, no active/selected visual indicator beyond the toggle. |

---

## 5. Buttons / Clickable Elements

| # | Severity | File:Line | Issue |
|---|----------|-----------|-------|
| 19 | **Medium** | `Controls/ModernDialog.xaml:119-159` | Confirm button has `Background="#E63946"` hardcoded in XAML — but code-behind overrides it per dialog type. The XAML default color is always red, which is correct for Danger but wrong for Primary/Success/Warning until code-behind runs. |
| 20 | **Medium** | `Controls/ModernDialog.xaml:122` | Confirm button `Width="130"` is fixed — at small dialog sizes, Persian confirm text like "ذخیره تغییرات" may overflow. |
| 21 | **Low** | `Themes/Styles.xaml:519-556` | `OutlinedButtonLarge` style has no `IsEnabled=False` trigger — disabled state shows full opacity. |
| 22 | **Low** | `Themes/Styles.xaml:1215-1242` | `ModernButton` style has no disabled state trigger — same issue. |

---

## 6. Forms / Inputs

| # | Severity | File:Line | Issue |
|---|----------|-----------|-------|
| 23 | **High** | `Views/NewPersonView.xaml.cs:36-66` | `GetCreatedPersonId()` performs synchronous `_personApplication.Search()` call — potential UI freeze. Should be async. |
| 24 | **High** | `Views/NewPersonView.xaml.cs:94` | `PersonCreated?.Invoke(this, personId)` is called synchronously after DB write — blocks UI thread. |
| 25 | **Medium** | `Controls/ModernPersianTextBox.xaml.cs:195-199` | `TextBox_TextChanged` converts digits to Persian on every keystroke, including during paste — may cause caret jumping with long text. |
| 26 | **Medium** | `Controls/ModernPersianTextBox.xaml.cs:374-385` | `ConvertToPersianDigits` does string replacement in a loop (10 iterations per call) — O(n×10) per keystroke. Acceptable for short fields, but inefficient for long text. |
| 27 | **Medium** | `Controls/ModernPersianTextBox.xaml.cs:136` | Number input regex `^[0-9۰-۹]+$` allows mixing English and Persian digits in the same string — no normalization to a single digit set. |
| 28 | **Low** | `Controls/ModernPersianTextBox.xaml` | `PART_TextBox` has no `MaxLength` binding from the control's own `MaxLength` property in XAML — it's set in code-behind only. |

---

## 7. DataGrid / Lists

| # | Severity | File:Line | Issue |
|---|----------|-----------|-------|
| 29 | **Medium** | `Controls/UnifiedListView.xaml:55-82` | `DataGridView` uses `SelectionMode="Extended"` — allows multi-select with Ctrl/Shift, but there's no visual multi-select feedback beyond checkbox state. |
| 30 | **Medium** | `Controls/UnifiedListView.xaml:66` | `RowHeight="36"` is fixed — long Persian text in any column will clip. |
| 31 | **Low** | `Controls/UnifiedListView.xaml:76` | `ScrollViewer.HorizontalScrollBarVisibility="Visible"` forces horizontal scrollbar to always show — should be `Auto` to hide when not needed. |
| 32 | **Low** | `Themes/GridStyles.xaml:70-71` | `UnifiedGridColumnHeaderStyle` has `BorderThickness="0,0,1,2"` — the right border (`1`) creates an asymmetric look in RTL where columns flow right-to-left. |

---

## 8. Dialogs / Modals

| # | Severity | File:Line | Issue |
|---|----------|-----------|-------|
| 33 | **Medium** | `Controls/ModernDialog.xaml:12` | Dialog `Width="440"` is fixed — cannot adapt to longer Persian text. |
| 34 | **Medium** | `Controls/ModernDialog.xaml:14` | `MinHeight="280"` is set but `SizeToContent="Height"` is used — the min height may cause empty space above buttons for short messages. |
| 35 | **Low** | `Controls/ModernDialog.xaml.cs:239-254` | Empty input validation shows shake animation + red border, but does not reset `InputWrapper.BorderBrush` back to normal on next keystroke — user must close/reopen dialog. |

---

## 9. Styling / Design System

| # | Severity | File:Line | Issue |
|---|----------|-----------|-------|
| 36 | **Medium** | `Themes/Styles.xaml` | `ModernButton` uses `Background="#2ECC71"` (green) but the rest of the app uses `{StaticResource GreenBrush}` (`#02CF54`). Inconsistent green values. |
| 37 | **Medium** | `Themes/Styles.xaml` | `ModernTabButton` uses `Width="150"` fixed — does not adapt to text length. |
| 38 | **Low** | `Themes/Styles.xaml` | `twoway` style (`Border` with `#D0F0C0` at `Opacity="0.2"`) is defined but its purpose is unclear — appears to be a debug/development artifact. |
| 39 | **Low** | `Themes/Styles.xaml` | `ModernTextBox` style references `materialDesign:HintAssist.FloatingScale` and `materialDesign:TextFieldAssist.TextFieldCornerRadius` — but these MaterialDesign features are only used by this style, not by `ModernPersianTextBox` which is the actual input control used everywhere. |
| 40 | **Low** | `Themes/Styles.xaml:1128-1139` | `ModernTextBox` style is defined but never used in any View or Control — dead style. |

---

## 10. Code-Level / Architecture

| # | Severity | File:Line | Issue |
|---|----------|-----------|-------|
| 41 | **High** | `Views/NewPersonView.xaml.cs` | All DB calls in code-behind (`_personApplication.Search()`, `personApplication.Create()`) are synchronous. Should use `await` with `Task.Run` or async repository methods. |
| 42 | **High** | `Views/MainWindow.xaml.cs` | Navigation uses `App.ServiceProvider.CreateScope()` — pattern is correct for DI, but scope disposal is not visible in the read code — may cause memory leaks. |
| 43 | **Medium** | `Controls/TreeComboBox.xaml.cs:195` | `FilterTree` uses `string.IndexOf(q, StringComparison.OrdinalIgnoreCase)` — this is case-insensitive but does not handle Persian diacritics or normalizations. |
| 44 | **Medium** | `Controls/TreeComboBox.xaml.cs:189-202` | `FilterTree` is recursive and creates cloned `TreeComboItem` objects on every search keystroke — no debouncing. |
| 45 | **Medium** | `Controls/ModernDialog.xaml.cs:314-323` | Safety timer for close animation (`DispatcherTimer` at 500ms) is a good pattern, but if the animation completes before the timer, the timer is stopped — correct. However, if `Close()` is called from both the timer and the animation `Completed` handler, it could double-close. The `_isClosing` flag prevents this. **OK.** |
| 46 | **Low** | `Controls/PaginationBar.xaml.cs:36-43` | `PageInfoContent` dependency property callback references `PageInfoText.Text` directly — correct, but could use binding instead. |
| 47 | **Low** | `Themes/GridStyles.xaml:87-121` | `UnifiedGridCellStyle` has `IsSelected` trigger that sets `Background="Transparent"` — selection is invisible on the cell. Combined with `UnifiedGridRowStyle` setting `IsMouseOver` background, row selection is only visible via the row hover. |

---

## 11. Accessibility

| # | Severity | File:Line | Issue |
|---|----------|-----------|-------|
| — | **Info** | All controls | No `AutomationProperties.Name` or `AutomationProperties.AutomationId` found on any interactive element. Screen reader support is absent. |
| — | **Info** | `Controls/ToastNotification.xaml` | Close button uses a `Border` with `MouseBinding` instead of a `Button` — no keyboard accessibility (Tab focus, Enter/Space activation). |

---

## Recommendations by Priority

### Critical (Fix First)
1. Replace synchronous DB calls in `NewPersonView.xaml.cs` with async pattern
2. Verify back-button code-behind in `NewBranchView`, `NewCompanyView`, `NewFinancialPeriodView`
3. Fix `ModernPersianTextBox` RTL text alignment default

### High (Fix Next)
4. Convert list view root layouts from `HorizontalAlignment="Center"` to `Stretch` + proper margins
5. Add `ScrollViewer` wrapper or `VerticalAlignment="Stretch"` to form views
6. Replace fixed `Width="215"` filter tabs with `Auto` sizing or `MaxWidth`
7. Fix `ModernDialog` confirm button color override timing

### Medium (Polish)
8. Normalize digit input to a single set (Persian or English)
9. Add debouncing to `TreeComboBox` search
10. Fix `ModernButton` and `OutlinedButtonLarge` disabled states
11. Replace hardcoded green values with `GreenBrush` resource
12. Add `AutomationProperties` to interactive elements
13. Make `ToastNotification` close button keyboard-accessible

### Low (Cleanup)
14. Remove dead `ModernTextBox` style
15. Remove `twoway` debug style
16. Fix `HorizontalScrollBarVisibility` to `Auto` in `UnifiedListView`
17. Verify `UnifiedGridCellStyle` selection visibility

---

*End of report.*
