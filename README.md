# SkyCore

This project has some setup after you clone the repository. We're using Wwise, but we can't distribute Audiokinetic's Wwise Unity integration code, so the gitignore excludes that code.

After you clone the repository:
1. Open Unity Hub and add the project. Do not open the project until finishing these steps.
2. Download the Audiokinetic Launcher at https://www.audiokinetic.com/en/download/
3. Open the Audiokinetic Launcher.
4. In the Wwise tab, install Wwise version 2021.1.12.7973.2529
5. In the Unity tab, next to the SkyCore Unity project, click the "Integrate Wwise in Project" button.
6. Integration version: 2021.1.12.7973.2529
7. Packages: Unity Integration and Unity Integration Extensions
8. Deployment Platforms: Apple -> macOS, and Microsoft -> Windows
9. Wwise SDK: Leave as default
10. Wwise Project: Click the dropdown arrow on the right, click browse, and select the project file: SkyCore/skyjellies_WwiseProject/skyjellies_WwiseProject.wproj
11. Unity Installation: Leave default (Unity 2021.3.20f1)
12. Integration options: Switch off "Create temporary directory". Leave Advanced options default.
13. Click the "Integrate" button.
14. Once it completes, it will say it failed. Ignore that.
15. In Github Desktop, right click where it says "[number] changed files" and discard all changes.
