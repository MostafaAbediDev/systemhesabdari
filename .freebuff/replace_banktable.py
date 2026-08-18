# -*- coding: utf-8 -*-
import io
import sys

def replace_block(path, start, end, new_text):
    with io.open(path, 'r', encoding='utf-8') as f:
        content = f.read()
    eol = '\r\n' if '\r\n' in content else '\n'
    lines = content.split('\n')
    # handle trailing newline artifacts
    if lines and lines[-1] == '':
        lines = lines[:-1]
    # sanity checks (1-indexed start/end)
    assert '<Border' in lines[start - 1] or '<!--' in lines[start - 1], \
        "BAD START %d: %r" % (start, lines[start - 1][:80])
    assert lines[end - 1].strip() == '</Border>', \
        "BAD END %d: %r" % (end, lines[end - 1][:80])
    new_lines = new_text.split('\n')
    new_lines = [l for l in new_lines if l != '\u2400']  # noop
    out = lines[:start - 1] + new_lines + lines[end:]
    with io.open(path, 'w', encoding='utf-8', newline='') as f:
        f.write(eol.join(out))
        if content.endswith('\n'):
            f.write('\n')
    print("OK %s replaced lines %d..%d with %d lines" % (path, start, end, len(new_lines)))

new_markup = """                            <!-- جدول حساب‌های بانکی -->
                            <controls:BankAccountsTableControl ItemsSource=\"{Binding BankAccounts}\"
                                                               EditRequested=\"BankAccountsTable_EditRequested\"
                                                               RemoveRequested=\"BankAccountsTable_RemoveRequested\" />"""

edit_markup = """                                <!-- جدول حساب‌های بانکی -->
                                <controls:BankAccountsTableControl ItemsSource=\"{Binding BankAccounts}\"
                                                                   RemoveConfirmation=\"True\"
                                                                   EditRequested=\"BankAccountsTable_EditRequested\"
                                                                   RemoveRequested=\"BankAccountsTable_RemoveRequested\" />"""

replace_block(r'src/FrontEndWPF/Taadol/Taadol/Views/NewPersonView.xaml', 913, 1221, new_markup)
replace_block(r'src/FrontEndWPF/Taadol/Taadol/Views/EditPersonView.xaml', 759, 1066, edit_markup)
