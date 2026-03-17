# MyCrypt &middot; [![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](./LICENSE) ![.NET](https://img.shields.io/badge/.NET-512BD4?logo=dotnet&logoColor=white) ![C#](https://img.shields.io/badge/C%23-239120?logo=c-sharp&logoColor=white)  ![Static Badge](https://img.shields.io/badge/CLI-8A2BE2)

MyCrypt is a command-line interface (CLI) tool built with .NET for encrypting, decrypting, and validating files in a simple, secure, and efficient way.

## Quick start
1. Clone the repo: `git clone https://github.com/Adler-Targino/MyCrypt.git`
2. From the terminal at the root of the project either build using `dotnet build` or run with `dotnet run`

*(An installer with quick setup will be provided later, once the project has most of it's functionalities implemented, for now, setting up the aplication must be done manually)*

## Features
- File encryption
- File decryption
- File integrity validation using hash

## Usage
Once you properly set up the aplication, you will be able to perform the following operations:
### File encryption
File encryption can be done as easily as running this command on your terminal.

```powershell
mycrypt encrypt .\file.txt
```
Once ran, a random 32 bytes (44 characters) long, random key is displayed at the terminal and your file is encrypted using it.

**Options:**

- `--key` Defines a key which will be used for encryption. (If not present, a random key is created to encrypt your file)
- `-o|--output` Defines the output path of the encrypted file. (By default the output is in the same folder as the input file)
- `-d|--delete` Deletes the original file after encryption.

**Example**
```powershell
mycrypt encrypt .\file.txt --key my-secret-key --output top-secret -d
```
Congratulations! Now your `file.txt` is encrypted into the file `top-secret.myc` and can only be decrypted with the key `my-secret-key`. And there are no traces left of your original file.

<sub>*I can only wonder what secrets you were trying to hide...*</sub>
