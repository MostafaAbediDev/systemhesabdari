---
name: dotnet-ui-wpf
description: Builds and refines WPF (Windows Presentation Foundation) desktop UIs on .NET 8+. Covers MVVM Toolkit, Fluent theming, XAML styling, DataGrid/list patterns, RTL/localization for Persian (Farsi), accessibility, and migration. Use for all WPF view/UI work in this repo. Do not use for backend API design.
---

# dotnet-ui-wpf (WPF only)

This is the WPF-focused slice of the `dotnet-artisan` `dotnet-ui` skill. It applies to the Taadol accounting front-end (`src/FrontEndWPF`), which is a Persian/Farsi Right-to-Left WPF app.

## Use this skill when
- Working on any `.xaml` view, style, template, or control in `src/FrontEndWPF/Taadol`.
- Building form views (New/Edit person, etc.), list views with DataGrid, filter bars, or theming.
- Handling RTL, Persian numerals, fonts (IRANSans), or localization.
- Deciding on a UI framework, MVVM structure, or XAML patterns.

## Companion references (load on demand)
| Topic | File |
|-------|------|
| WPF modern: Host builder, MVVM Toolkit, Fluent theme, performance | references/wpf-modern.md |
| WPF migration to .NET 8+ | references/wpf-migration.md |
| Accessibility (AutomationPeer, keyboard nav) | references/accessibility.md |
| Localization (.resx, pluralization, RTL) | references/localization.md |
| Framework selection decision tree | references/ui-chooser.md |

## Project constraints to respect (Taadol accounting app)
- **Front-end only.** Never change backend (`PersonManagement.*`, etc.) code.
- WPF, `FlowDirection="RightToLeft"` globally; remember `HorizontalAlignment` is mirrored in RTL.
- Font: `IRANSans` (`IRANSansXFaNum`) for Persian text + numerals.
- All DB access from views must go through `Task.Run` + `App.ServiceProvider.CreateScope()`. Never touch DB synchronously on the UI thread.
- Validate before save: National Code (checksum), mobile, email, Shaba — mirror `Create()` uniqueness checks in `Edit()`.
- Keep filter tabs + search in one responsive row; on narrow width reduce padding, never wrap the row.
- For wide DataGrids use an outer ScrollViewer for horizontal scroll; avoid `Border.Clip` (it cuts the scrollbar). Avoid `Width="*"` on the trailing column if you want overflow.
- Persian numerals: use the `ToPersianNumber(long)` helper present in `PersonListView.xaml.cs`.