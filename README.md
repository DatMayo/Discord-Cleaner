# Discord Cleaner

A comprehensive utility designed to completely remove and reinstall Discord, resolving common installation issues and update loops.

## 🔧 What It Does

Discord Cleaner helps resolve Discord-related issues by performing a complete cleanup and fresh installation:

1. **Process Termination**: Safely terminates all Discord and Discord-Update processes
2. **Data Removal**: Removes Discord application data from both AppData and LocalAppData folders
3. **Fresh Installation**: Downloads and installs the latest Discord version from official servers
4. **Safety Measures**: Includes process filtering to avoid terminating unrelated applications

## ⚠️ Important Warning

**All Discord login credentials and settings will be lost!** You will need to:
- Re-enter your login credentials
- Reconfigure all Discord settings
- Re-join voice channels and servers if needed

## 🎯 When to Use

This tool is particularly useful for resolving:
- Discord update loops
- Corrupted Discord installations  
- Discord failing to start or launch
- Persistent Discord crashes
- Installation corruption issues

## 💻 System Requirements

- **Operating System**: Windows (any version)
- **Framework**: .NET Framework 2.0 or later
- **Architecture**: Any CPU (x86/x64)
- **Permissions**: Administrator rights may be required for process termination

## 🚀 Usage

1. **Download** the latest release from the [Releases](https://github.com/DatMayolein/Discord-Cleaner/releases) page
2. **Close Discord** manually if possible (the tool will force-close it otherwise)
3. **Run** `DiscordCleaner.exe`
4. **Confirm** the operation when prompted (Y/N)
5. **Wait** for the process to complete
6. **Login** to Discord with your credentials

## 🔄 Process Flow

```
Start → User Confirmation → Kill Processes → Remove Data → Download Discord → Install → Complete
```

### Detailed Steps:

1. **User Confirmation**: Prompts for permission to proceed
2. **Process Termination**: 
   - Finds and terminates Discord main processes
   - Safely identifies and terminates Discord update processes
   - Waits 5 seconds for cleanup
3. **Data Removal**:
   - Removes `%AppData%\discord` folder
   - Removes `%LocalAppData%\discord` folder  
4. **Download & Install**:
   - Downloads latest Discord installer from official servers
   - Shows download progress
   - Automatically runs the installer
   - Waits for installation completion

## 🛡️ Safety Features

- **Process Filtering**: Only terminates Discord-related processes, not generic "update" processes
- **Error Handling**: Comprehensive error messages and graceful failure handling
- **Official Downloads**: Uses Discord's official download endpoints
- **Progress Reporting**: Real-time feedback during operations

## 🔧 Technical Details

- **Framework**: .NET Framework 2.0 (for maximum compatibility)
- **Language**: C#
- **Architecture**: Console Application
- **Download Source**: `https://discord.com/api/downloads/distributions/app/installers/latest?channel=stable&platform=win&arch=x64`

## 📁 File Structure

```
Discord-Cleaner/
├── Program.cs              # Main application logic
├── DiscordCleaner.csproj   # Project configuration
├── README.md               # This documentation
├── programIcon.ico         # Application icon
└── Properties/
    └── AssemblyInfo.cs     # Assembly metadata
```

## 🚨 Exit Codes

The application uses specific exit codes for different scenarios:

- `0` - Success
- `1` - User interrupted operation  
- `2` - Could not terminate Discord processes
- `3` - Could not terminate Discord update processes
- `4` - Could not delete Discord AppData folder
- `5` - Could not delete Discord LocalAppData folder

## 🤝 Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.

## 📝 License

This project is released under an open-source license. See the repository for details.

## 🔗 Links

- **GitHub Repository**: [https://github.com/DatMayolein/Discord-Cleaner](https://github.com/DatMayolein/Discord-Cleaner)
- **Icon Source**: [https://bit.ly/2HVZx0B](https://bit.ly/2HVZx0B)
- **Discord Official**: [https://discord.com](https://discord.com)

## 📚 Version History

- **v0.3** (05.10.25): Improved process safety, better error handling, updated Discord download URL
- **v0.2** (07.05.18): Initial public release

---

**Developed by DatMayo** | Last Updated: October 2025
