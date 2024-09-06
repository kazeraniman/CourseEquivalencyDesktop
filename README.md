# CourseEquivalencyDesktop

A desktop application to manage equivalent courses for exchange students. Created for my father and used as an excuse
learn how to develop a Windows desktop application since I have not touched it since Windows Forms. This project uses
Avalonia UI.

## Development Prerequisites

- .Net8.0
- Visual Studio (for packaging; I develop on Rider)
- [Windows App Development Prerequisites](https://learn.microsoft.com/en-us/windows/uwp/debug-test-perf/windows-app-certification-kit#prerequisites)
- Create a template for the credit transfer memo export (not committed as it may be proprietary). Ensure that the
  placeholder dictionary matches your template. The template should be stored as
  `DocumentTemplates/CreditTransferMemoTemplate.docx`.

## Version Updates

To publish a new version:

1. Switch to the main branch with `git checkout main`.
2. Run the script to update the version with `bash Scripts/release.sh <version_number>`.
3. If everything is well, perform the commands printed to finalize the version update.
4. Open the solution in Visual Studio.
5. Right-click the **CourseEquivalencyDesktopPackagingProject** and select **Publish &rarr; Create App Packages...**.
6. Select the option for the Microsoft Store associated with the app and click **Next**.
7. Ensure the options are correct and then click **Create**.
    - **Output location** is set.
    - **Version** matches the version you used in step 2.
    - **Automatically increment** is **Off**.
    - **Generate app bundle** is set to **Always**.
    - **x86 and x64** Architectures are **On** and set to their **Release Solution Configurations**.
    - **Include public symbol files** is **On**.
    - **Generate artifacts** is **On**.
8. After the packages are created, follow the dialog to validate by selecting to run all tests and then not
   automatically submit (as that require Azure).
    - If you can't validate, ensure that you have followed
      the [Windows App Development Prerequisites](https://learn.microsoft.com/en-us/windows/uwp/debug-test-perf/windows-app-certification-kit#prerequisites),
      specifically having installed
      the [Windows App Certification Kit](https://developer.microsoft.com/en-us/windows/downloads/windows-sdk/).
9. Navigate to the [Microsoft Partner Dashboard](https://partner.microsoft.com/en-us/dashboard) and submit the update.
